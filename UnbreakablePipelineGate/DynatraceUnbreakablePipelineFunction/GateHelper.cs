using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceUnbreakablePipelineFunction
{

    public class GateHelper
    {
        public enum Result { Succeeded, Failed }

        public string AuthToken { get; set; }
        public string VstsUrl { get; set; }
        public string HubName { get; set; }

        public Guid ProjectGuid { get; set; }
        public Guid PlanGuid { get; set; }
        public Guid JobGuid { get; set; }
        public Guid TimelineGuid { get; set; }
        public Guid TaskInstanceGuid { get; set; }

        public VssConnection Connection { get; set; }

        public TaskHttpClient TaskClient { get; set; }
        public TaskOrchestrationPlan Plan { get; set; }
        public TimelineRecord TimelineRecord { get; set; }



        public GateHelper(string taskInstanceId, string hubName, string jobId, string planId, string timelineId, string projectId, string vstsUrl, string authToken)
        {
            // create guuids
            this.JobGuid = new Guid(jobId);
            this.PlanGuid = new Guid(planId);
            this.TimelineGuid = new Guid(timelineId);
            this.ProjectGuid = new Guid(projectId);
            this.TaskInstanceGuid = new Guid(taskInstanceId);

            // save token and vsts url
            this.VstsUrl = vstsUrl;
            this.AuthToken = authToken;
            this.HubName = hubName;

            // create connection to vsts
            try
            {
                this.Connection = new VssConnection(new Uri(this.VstsUrl), new VssBasicCredential("", authToken));

            }
            catch (Exception e)
            {
                System.Console.WriteLine("ok");
            }

            // get task client
            try
            {
                this.TaskClient = this.Connection.GetClient<TaskHttpClient>();

            }
            catch (Exception e)
            {
                System.Console.WriteLine("ok");
            }
            // get the plan
            this.Plan = this.TaskClient.GetPlanAsync(this.ProjectGuid, this.HubName, this.PlanGuid).SyncResult();

            // get httpTaskTimeLineRecord
            var timeLineRecords = this.TaskClient.GetRecordsAsync(this.ProjectGuid, this.HubName, this.PlanGuid, this.TimelineGuid).SyncResult();
            this.TimelineRecord = timeLineRecords.Where(record => record.ParentId != null)
                .First();

        }

        public void SendLiveLogMessage(string message)
        {
            var liveFeedList = new List<string> { message };
            this.TaskClient.AppendTimelineRecordFeedAsync(this.ProjectGuid, this.HubName, this.PlanGuid, this.Plan.Timeline.Id, this.JobGuid, liveFeedList);
        }

        public void SendOfflineLog(string message)
        {
            var logPath = string.Format(CultureInfo.InvariantCulture, "logs\\{0:D}", this.TimelineRecord.Id);
            var tasklog = new TaskLog(logPath);
            var log = this.TaskClient.CreateLogAsync(this.ProjectGuid, this.HubName, this.PlanGuid, tasklog).SyncResult();
            using (var ms = new MemoryStream())
            {
                var allBytes = Encoding.UTF8.GetBytes(message);
                ms.Write(allBytes, 0, allBytes.Length);
                ms.Position = 0;
                this.TaskClient.AppendLogContentAsync(this.ProjectGuid, this.HubName, this.PlanGuid, log.Id, ms).SyncResult();
            }
        }

        public void FinishGate(Result result, string message)
        {
            var taskResult = result == Result.Succeeded ? TaskResult.Succeeded : TaskResult.Failed;

            var jobId = this.HubName.ToLower().Equals("gates", StringComparison.OrdinalIgnoreCase)
                    ? this.TimelineRecord.Id
                    : this.JobGuid;
            var taskCompletedEvent = new TaskCompletedEvent(this.TaskInstanceGuid, Guid.Empty, taskResult);
            this.TaskClient.RaisePlanEventAsync(this.ProjectGuid, this.HubName, this.PlanGuid, taskCompletedEvent).SyncResult();
        }
    }
}
