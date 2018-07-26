using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;


namespace DynatraceUnbreakablePipelineFunction
{
    class ExecuteObject
    {
        public readonly static string HUBNAME = "Gates";
        //public readonly static string HUBNAME = "Release";

        public string MonspecUrl { get; set; }
        public string PipelineInfoUrl { get; set; }
        public string DynatraceTennantUrl { get; set; }
        public string DynatraceToken { get; set; }
        public string ProxyUrl { get; set; }
        public string ServiceToCompare { get; set; }
        public string CompareWindow { get; set; }

        public string TaskInstanceId { get; set; }
        public string HubName { get; set; }
        public string JobId { get; set; }
        public string PlanId { get; set; }
        public string TimelineId { get; set; }
        public string ProjectId { get; set; }
        public string VstsUrl { get; set; }
        public string AuthToken { get; set; }

        public TraceWriter Log { get; set; }

        public ExecuteObject() { }

        public ExecuteObject(
            string hubName,
            string jobId,
            string planId,
            string timelineId,
            string projectId,
            string vstsUrl,
            string taskInstanceId,
            string authToken,
            string monspecUrl,
            string pipelineInfoUrl,
            string compareWindow,
            string dynatraceTennantUrl,
            string dynatraceToken,
            string proxyUrl,
            string serviceToCompare,
            TraceWriter log)
        {
            this.MonspecUrl = monspecUrl;
            this.PipelineInfoUrl = pipelineInfoUrl; ;
            this.DynatraceTennantUrl = dynatraceTennantUrl;
            this.DynatraceToken = dynatraceToken;
            this.ProxyUrl = proxyUrl;
            this.ServiceToCompare = serviceToCompare;
            this.CompareWindow = compareWindow;

            this.TaskInstanceId = taskInstanceId;
            this.JobId = jobId;
            this.PlanId = planId;
            this.TimelineId = timelineId;
            this.ProjectId = projectId;
            this.VstsUrl = vstsUrl;
            this.AuthToken = authToken;
            this.HubName = hubName;

            this.Log = log;
        }

        public string MonspecPullRequestReturnString()
        {
            // read monspec file
            var monspecString = string.Empty;
            using (var client = new WebClient())
            {
                var monspecByteArray = client.DownloadData(this.MonspecUrl);
                monspecString = System.Text.Encoding.Default.GetString(monspecByteArray);

            }

            // read pipeline info file
            var pipelineInfoString = string.Empty;
            using (var client = new WebClient())
            {
                var pipelineInfoByteArray = client.DownloadData(this.PipelineInfoUrl);
                pipelineInfoString = Encoding.Default.GetString(pipelineInfoByteArray);
            }

            // make web api call to proxy 
            var req = WebRequest.Create(this.ProxyUrl);
            var postDataBuilder = new StringBuilder();
            postDataBuilder.Append("serviceToCompare=");
            postDataBuilder.Append(this.ServiceToCompare);
            postDataBuilder.Append("&compareWindow=");
            postDataBuilder.Append(this.CompareWindow);
            postDataBuilder.Append("&dynatraceTennantUrl=");
            postDataBuilder.Append(this.DynatraceTennantUrl);
            postDataBuilder.Append("&token=");
            postDataBuilder.Append(this.DynatraceToken);
            postDataBuilder.Append("&monspecFile=");
            postDataBuilder.Append(monspecString);
            postDataBuilder.Append("&pipelineInfoFile=");
            postDataBuilder.Append(pipelineInfoString);
            string postData = postDataBuilder.ToString();

            byte[] send = Encoding.Default.GetBytes(postData);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = send.Length;

            Stream sout = req.GetRequestStream();
            sout.Write(send, 0, send.Length);
            sout.Flush();
            sout.Close();

            // read response back as string
            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string returnvalue = sr.ReadToEnd();

            // parse and turn into object
            return returnvalue;
        }

        public PullCompareResponse MonspecPullRequest()
        {
            // query dynatrace with monspec
            string returnvalue = this.MonspecPullRequestReturnString();

            // parse and turn into object
            var pullCompareResponse = JsonConvert.DeserializeObject<PullCompareResponse>(returnvalue);
            return pullCompareResponse;
        }

        // long running method called by seperate thread
        public void Execute()
        {
            var gateHelper = new GateHelper(
                this.TaskInstanceId,
                this.HubName,
                this.JobId,
                this.PlanId,
                this.TimelineId,
                this.ProjectId,
                this.VstsUrl,
                this.AuthToken);


            // query dynatrace with monspec. this is a long running command
            // sending live log message
            gateHelper.SendLiveLogMessage("querying Dynatrace with monspec...");
            var response = this.MonspecPullRequestReturnString();
            var pullCompareResponseObj = JsonConvert.DeserializeObject<PullCompareResponse>(response);
            gateHelper.SendLiveLogMessage("finished querying Dynatrace");

            // finished with long running work, will now send offline logs and then send task complete event back to vsts
            gateHelper.SendOfflineLog(response);

            // check response and either pass or fail gate
            if (pullCompareResponseObj.totalViolations == 0)
            {
                this.Log.Info("There are zero violations, sending succeeded to gate");
                gateHelper.FinishGate(GateHelper.Result.Succeeded, response);
            }
            else
            {
                this.Log.Error("There is a violation (" + pullCompareResponseObj.totalViolations +"), sending fail to gate: " + response);
                //gateHelper.FinishGate(GateHelper.Result.Succeeded, response);
                gateHelper.FinishGate(GateHelper.Result.Failed, response);
            }
        }
    }
}
