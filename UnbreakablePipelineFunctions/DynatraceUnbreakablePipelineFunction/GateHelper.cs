using Microsoft.Azure.WebJobs.Host;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace DynatraceUnbreakablePipelineFunction
{
    // TODO - remove this class if not going to use this approach
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
        public TraceWriter Log { get; set; }

        public GateHelper(
            string taskInstanceId,
            string hubName,
            string jobId,
            string planId,
            string timelineId,
            string projectId,
            string vstsUrl,
            string authToken,
            TraceWriter log)
        {
            this.Log = log;

            var logMessage = "GateHelper initialize: Start";
            this.Log.Info(logMessage);

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

            logMessage = "GateHelper initialize: Initialized class variables";
            this.Log.Info(logMessage);

        }
        public void GetVstsConnection() {
            String logMessage;

            // create connection to vsts
            try
            {
                logMessage = "GateHelper.GetVstsConnection(): Attempting to get VssConnection to: " + this.VstsUrl;
                this.Log.Info(logMessage);
                    this.Connection = new VssConnection(new Uri(this.VstsUrl), new VssBasicCredential("", this.AuthToken));
                logMessage = "GetVstsConnection: Got VssConnection to: " + this.VstsUrl;
                this.Log.Info(logMessage);
            }
            catch (Exception e)
            {
                logMessage = "GateHelper.GetVstsConnection(): Failed to connect to URL: " + this.VstsUrl + " using Token:" + this.AuthToken + " : " + e.Message;
                this.Log.Error(logMessage);
                throw;
            }
        }
        public void GetTaskClient()
        {
            String logMessage;

            // get task client - assumes has Connection
            try
            {
                logMessage = "GateHelper.GetTaskClient(): Attempting to get TaskHttpClient to: " + this.VstsUrl;
                this.Log.Info(logMessage);
                this.TaskClient = this.Connection.GetClient<TaskHttpClient>();
                logMessage = "GateHelper.GateHelper() initialize: Got TaskHttpClient to: " + this.VstsUrl;
                this.Log.Info(logMessage);
            }
            catch (Exception e)
            {
                logMessage = "GateHelper.GetTaskClient(): Failed to get TaskHttpClient to: " + this.VstsUrl + ": " + e.Message;
                this.Log.Error(logMessage);
                throw;
            }
        }
        public void GetTaskClientPlan()
        {
            String logMessage;

            // get the plan - assumes has TaskClient
            try
            {
                this.Plan = this.TaskClient.GetPlanAsync(this.ProjectGuid, this.HubName, this.PlanGuid).SyncResult();
                logMessage = "GateHelper.GetTaskClientPlan(): Got TaskClient.GetPlanAsync.SyncResult";
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);
            }
            catch (Exception e)
            {
                logMessage = "GateHelper.GetTaskClientPlan(): Failed to get GetTaskClientPlan: " + e.Message;
                this.Log.Error(logMessage);
                SendLiveLogMessage(logMessage);
                throw;
            }
        }
        public void GetTaskTimelineRecord()
        {
            String logMessage;
            try
            {
                // get httpTaskTimeLineRecord  - assumes has TaskClient

                var timeLineRecords = this.TaskClient.GetRecordsAsync(this.ProjectGuid, this.HubName, this.PlanGuid, this.TimelineGuid).SyncResult();
                this.TimelineRecord = timeLineRecords.Where(record => record.ParentId != null)
                    .First();
                logMessage = "GateHelper.GetTaskTimelineRecord(): Got TimelineRecord.Id: " + this.TimelineRecord.Id;
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);
            }
            catch (Exception e)
            {
                logMessage = "GateHelper.GetTaskTimelineRecord(): Failed to get TaskClient.GetRecordsAsync.SyncResult.First: " + e.Message;
                this.Log.Error(logMessage);
                SendLiveLogMessage(logMessage);
                throw;
            }

        }

        public void SendLiveLogMessage(string message)
        {
            message += "LIVE: " + message;
            var liveFeedList = new List<string> { message };
            if (this.TaskClient != null)
            {
                //this.Log.Info("GateHelper.SendLiveLogMessage(): Start");
                this.TaskClient.AppendTimelineRecordFeedAsync(this.ProjectGuid, this.HubName, this.PlanGuid, this.Plan.Timeline.Id, this.JobGuid, liveFeedList);
                //this.Log.Info("GateHelper.SendLiveLogMessage(): Complete");
            }
            else
            {
                this.Log.Info("GateHelper.SendLiveLogMessage(): skipping due to no TaskClient. Message: " + message);
            }
        }

        public void SendOfflineLog(string message)
        {
            if (this.TaskClient != null)
            {
                var logPath = string.Format(CultureInfo.InvariantCulture, "logs\\{0:D}", this.TimelineRecord.Id);
                var tasklog = new TaskLog(logPath);
                var syncResult = this.TaskClient.CreateLogAsync(this.ProjectGuid, this.HubName, this.PlanGuid, tasklog).SyncResult();
                using (var ms = new MemoryStream())
                {
                    var allBytes = Encoding.UTF8.GetBytes(message);
                    ms.Write(allBytes, 0, allBytes.Length);
                    ms.Position = 0;
                    this.TaskClient.AppendLogContentAsync(this.ProjectGuid, this.HubName, this.PlanGuid, syncResult.Id, ms).SyncResult();
                }
            }
            else
            {
                this.Log.Info("GateHelper.SendOfflineLog(): skipping due to no TaskClient");
            }
        }

        public void FinishGate(Result result, string message)
        {
            String logMessage;

            if (this.TimelineRecord == null)
            {
                logMessage = "GateHelper.FinishGate(): Skipping for TimelineRecord is null";
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);
                return;
            }
            if (this.JobGuid == null)
            {
                logMessage = "GateHelper.FinishGate(): Skipping for JobGuid is null";
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);
                return;
            }
            if (this.HubName == null)
            {
                logMessage = "GateHelper.FinishGate(): Skipping for HubName is null";
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);
                return;
            }

            logMessage = "GateHelper.FinishGate(): Starting for HubName: " + this.HubName;
            this.Log.Info(logMessage);
            SendLiveLogMessage(logMessage);

            var taskResult = result == Result.Succeeded ? TaskResult.Succeeded : TaskResult.Failed;

            var jobId = this.HubName.ToLower().Equals("gates", StringComparison.OrdinalIgnoreCase)
                    ? this.TimelineRecord.Id
                    : this.JobGuid;

            logMessage = "GateHelper.FinishGate(): Using HubName: " + this.HubName + " and jobId: " + jobId + " taskResult: " + taskResult.ToString();
            this.Log.Info(logMessage);
            SendLiveLogMessage(logMessage);

            var taskCompletedEvent = new TaskCompletedEvent();

            try
            {
                logMessage = "GateHelper.FinishGate(): Attempting to create TaskCompletedEvent for TaskInstanceGuid: " + this.TaskInstanceGuid;
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);

                taskCompletedEvent = new TaskCompletedEvent(this.TaskInstanceGuid, Guid.Empty, taskResult);

            }
            catch (Exception e)
            {
                logMessage = "GateHelper.FinishGate(): Exception creating TaskCompletedEvent: " + e.Message;
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);
                throw;
            }

            try
            {
                logMessage = "GateHelper.FinishGate(): Attempting to call TaskClient.RaisePlanEventAsync() method with ProjectGuid: " + this.ProjectGuid + " and PlanGuid: " + this.PlanGuid;
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);

                this.TaskClient.RaisePlanEventAsync(this.ProjectGuid, this.HubName, this.PlanGuid, taskCompletedEvent).SyncResult();

                logMessage = "GateHelper.FinishGate(): Complete";
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);
            }
            catch (Exception e)
            {
                logMessage = "GateHelper.FinishGate(): Exception calling TaskClient.RaisePlanEventAsync: " + e.Message;
                this.Log.Info(logMessage);
                SendLiveLogMessage(logMessage);
                throw;
            }   
        }
    }
}
