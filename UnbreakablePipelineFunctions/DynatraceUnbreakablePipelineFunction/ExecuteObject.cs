using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System;


namespace DynatraceUnbreakablePipelineFunction
{
    public class ExecuteObject
    {
        //public readonly static string HUBNAME = "Gates";
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
            log.Info("ExecuteObject initialize: Start");

            this.MonspecUrl = monspecUrl;
            this.PipelineInfoUrl = pipelineInfoUrl;
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

            log.Info("ExecuteObject initialize: Complete");
        }

        public string MonspecPullRequestReturnString()
        {
            // read monspec file
            var monspecString = string.Empty;
            using (var client = new WebClient())
            {
                this.Log.Info("MonspecPullRequestReturnString: attempting to read MonspecUrl: " + this.MonspecUrl);
                var monspecByteArray = client.DownloadData(this.MonspecUrl);
                monspecString = System.Text.Encoding.Default.GetString(monspecByteArray);

            }

            // read pipeline info file
            var pipelineInfoString = string.Empty;
            using (var client = new WebClient())
            {
                this.Log.Info("MonspecPullRequestReturnString: attempting to read PipelineInfoUrl: " + this.PipelineInfoUrl);
                var pipelineInfoByteArray = client.DownloadData(this.PipelineInfoUrl);
                pipelineInfoString = Encoding.Default.GetString(pipelineInfoByteArray);
            }

            // make web api call to proxy 
            this.Log.Info("MonspecPullRequestReturnString: Composing Monspec request to ProxyUrl: " + this.ProxyUrl);
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
            this.Log.Info("MonspecPullRequestReturnString: Making Monspec request to ProxyUrl: " + this.ProxyUrl);
            WebResponse res = req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            string returnvalue = sr.ReadToEnd();

            this.Log.Info("MonspecPullRequestReturnString: Monspec request got back: " + returnvalue);

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
            Boolean success = false;
            var logMessage = "ExecuteObject.Execute(): Start";
            this.Log.Info(logMessage);

            String response;
            var gateHelper = new GateHelper(
                this.TaskInstanceId,
                this.HubName,
                this.JobId,
                this.PlanId,
                this.TimelineId,
                this.ProjectId,
                this.VstsUrl,
                this.AuthToken,
                this.Log);

            this.Log.Info("ExecuteObject.Execute(): finished creating new GateHelper");

            try
            {
                // Setup connections and obtain DevOps details
                gateHelper.GetVstsConnection();
                gateHelper.GetTaskClient();
                gateHelper.GetTaskClientPlan();
                gateHelper.GetTaskTimelineRecord();
                logMessage = "ExecuteObject.Execute(): finished getting connections and obtain DevOps task details";
                gateHelper.SendLiveLogMessage(logMessage);
                this.Log.Info(logMessage);

                // query dynatrace with monspec. this is a long running command
                logMessage = "ExecuteObject.Execute(): querying Dynatrace with monspec";
                gateHelper.SendLiveLogMessage(logMessage);
                this.Log.Info(logMessage);
                response = this.MonspecPullRequestReturnString();
                logMessage = "ExecuteObject.Execute(): finished Dynatrace monspec query";
                gateHelper.SendLiveLogMessage(logMessage);
                this.Log.Info(logMessage);

                var pullCompareResponseObj = JsonConvert.DeserializeObject<PullCompareResponse>(response);
                logMessage = "ExecuteObject.Execute(): finished deserializing monspec response";
                gateHelper.SendLiveLogMessage(logMessage);
                this.Log.Info(logMessage);

                // finished with long running work, will now send offline logs and then send task complete event back to vsts
                gateHelper.SendOfflineLog(response);
                this.Log.Info(response);

                // check response and either pass or fail gate
                if (pullCompareResponseObj.totalViolations == 0)
                {
                    this.Log.Info("ExecuteObject.Execute(): There are zero violations, sending succeeded to gate");
                    gateHelper.FinishGate(GateHelper.Result.Succeeded, response);
                    success = true;
                }
                else
                {
                    this.Log.Error("ExecuteObject.Execute(): There is a violation (" + pullCompareResponseObj.totalViolations + "), sending fail to gate: " + response);
                    gateHelper.FinishGate(GateHelper.Result.Failed, response);
                    return;
                }
            }
            catch (Exception e)
            {
                logMessage = "ExecuteObject.Execute() exception: " + e.Message;
                this.Log.Error(logMessage);
                throw;
            }

            if (!success)
            {
                try
                {
                    this.Log.Error("ExecuteObject.Execute(): calling FinishGate for processing exception FAILURE");
                    gateHelper.FinishGate(GateHelper.Result.Failed, logMessage);
                }
                catch (Exception e2)
                {
                    logMessage = "ExecuteObject.Execute(): There was an exception calling gateHelper.FinishGate" + e2.Message + ": " + e2.StackTrace;
                    this.Log.Error(logMessage);
                    throw;
                }
            }
        }
    }
}
