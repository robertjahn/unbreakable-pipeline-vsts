using DynatraceSelfHealingFunction.VSTS;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceSelfHealingFunction
{
    public class DynatraceAlertProcessor
    {
        public string DTTimeSpanMinutes { get; set; }

        public TraceWriter Log { get; set; }

        public string DTApiToken { get; set; }
        public string DTTenantUrl { get; set; }
        public string DTVSTSUrl { get; set; }
        public string DTVSTSPAT { get; set; }
        public string VSTSReleaseApiUrl { get; set; }
        public string DTEnvironmentTag { get; set; }

        public HttpRequestMessage Request { get; set; }
        public dynamic NotificationObject { get; set; }


        public async Task<string> ProcessDynatraceAlert()
        {
            Log.Info("In ProcessDynatraceAlert(), notificationObject.ImpactedEnties: " + this.NotificationObject?.ImpactedEntities);
            var DTTimeSpanMS = 60 * 1000 * Int32.Parse(this.DTTimeSpanMinutes);
            FixedEvent[] fixedEvents = await fixMostRecentDeploymentsOnEntities(NotificationObject?.ImpactedEntities, DTTimeSpanMS);

            Log.Info("fixed events, updating problem ticket");
            // we have our information and can now iterate and update the problem ticket
            foreach (var fixedEvent in fixedEvents)
            {
                Log.Info("fixedEvent: " + fixedEvent);
                // create comment body
                var commentBody = new FixedEventComment
                {
                    comment = "Triggered release " + fixedEvent.OrigEvent?.deploymentName.Value + " " + fixedEvent.RollbackReleaseId + " in project " + fixedEvent.OrigEvent?.deploymentProject.Value,
                    user = "Dynatrace Self healing Function Remediation Action",
                    context = "Azure Function"
                };
                Log.Info("Created FixedEventComment object: " + JsonConvert.SerializeObject(commentBody));

                // post comment body to dynatrace problem comments
                var fullUrl = this.DTTenantUrl + "/api/v1/problem/details/" + this.NotificationObject?.PID.Value + "/comments";
                Log.Info("full url: " + fullUrl);
                var response = await DynatraceAPIHelper.Post(fullUrl, this.DTApiToken, JsonConvert.SerializeObject(commentBody));
                var statusCode = response.StatusCode;
                var data = await response.Content.ReadAsStringAsync();
                this.Log.Info("Push comment to Dynatrace: " + fullUrl + " " + statusCode + "-" + data);
            }


            return "Executed Handler successfully!";
        }

        private async Task<FixedEvent[]> fixMostRecentDeploymentsOnEntities(dynamic entities, int timespan)
        {
            Log.Info("Starting fixMostRecentDeploymentsOnEntities");
            if (entities.Count == 0)
            {
                this.Log.Info("No entites passed to getRecentDeploymentsOnEntities");
                return new FixedEvent[0];
            }

            var returnArray = new List<FixedEvent>();

            dynamic[] mostRecentEvents = await getMostRecentDeploymentOnEntity(entities, timespan);
            this.Log.Info("Found Most Recent Deployment Events: " + mostRecentEvents.Length);

            var vstsHelper = new VSTSHelper
            {
                VSTSUrl = this.DTVSTSUrl,
                VSTSPAT = this.DTVSTSPAT,
                VSTSReleaseApiUrl = this.VSTSReleaseApiUrl,
                Log = this.Log
            };

            this.Log.Info("Iterating through deployment events and rolling back");
            foreach (var deployEvent in mostRecentEvents)
            {
                this.Log.Info("Problem event: " + deployEvent);
                var releaseId = int.Parse(deployEvent?.deploymentVersion.Value);
                var vstsTeamProject = deployEvent?.deploymentProject.Value;

                // need to find the environment value within the tag array
                var environment = getEnvironmentTagValue(deployEvent);

                // I want to use a custom event. Need to experiment
                //var releaseName = problemEvent?.customProperties?.VSTSReleaseName.Value;
                //var releaseId = problemEvent?.customProperties?.ReleaseId.Value;
                //var vstsTeamProject = problemEvent?.customProperties?.VSTSTeamProject.Value;
                //var environment = problemEvent?.customProperties?.VSTSEnvironment.Value;

                this.Log.Info("Rolling back release from VSTS");
                var rollbackId = vstsHelper.ReleaseProblemDetected(vstsTeamProject, releaseId, environment);
                returnArray.Add(new FixedEvent
                {
                    RollbackReleaseId = rollbackId,
                    OrigEvent = deployEvent
                });
            }

            this.Log.Info("Done with fixMostRecentDeploymentsOnEntities");
            return returnArray.ToArray();
        }

        private string getEnvironmentTagValue(dynamic deployEvent)
        {
            this.Log.Info("getEnvironmentTagValue: Iterating through deployment tags, looking for: " + this.DTEnvironmentTag);
            string environmentTagValue = "NOT_FOUND";
            // loop through the tags and look for the environment one
            // look for the tag's value in for the key stored in DTEnvironmentTag
            foreach (var problemTag in deployEvent?.tags)
            {
                if (problemTag.key.Value == this.DTEnvironmentTag)
                {
                    this.Log.Info("getEnvironmentTagValue: Loop found key: " + problemTag.key.Value + " value: " + problemTag.value.Value);
                    environmentTagValue = problemTag.value.Value;
                }
            }
            this.Log.Info("getEnvironmentTagValue: Returning environmentTagValue: " + environmentTagValue);
            return environmentTagValue;
        }

        /**
         * Returns the most recent CUSTOM_DEPLOYMENT event on these passed entites where the event.source == VSTS
         *
         */
        private async Task<dynamic[]> getMostRecentDeploymentOnEntity(dynamic entities, int timespan)
        {
            this.Log.Info("Starting getMostRecentDeploymentOnEntity");
            List<dynamic> resultEvents = new List<dynamic>();

            this.Log.Info("Iterating through all entities");
            foreach (var entity in entities)
            {
                this.Log.Info("Entity: " + entity);
                // build out dynatrace query to get most recent deployment event for each entity
                var dtEventUrl = this.DTTenantUrl + "/api/v1/events";
                var to = DateTime.Now;
                var from = to.Subtract(new TimeSpan(0, 0, 0, 0, timespan));
                var queryString = "?entitydId=" + entity?.entity + "&eventType=CUSTOM_DEPLOYMENT";
                if (timespan > 0)
                {
                    queryString += "&to=" + DynatraceAPIHelper.ConvertToJSSMilliseconds(to) + "&from=" + DynatraceAPIHelper.ConvertToJSSMilliseconds(from);
                }

                // execute query to dynatrace
                this.Log.Info("Executing query to Dynatrace: " + queryString);
                HttpResponseMessage response = await DynatraceAPIHelper.Post(dtEventUrl + queryString, this.DTApiToken, null);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    this.Log.Info("Got valid response from dynatrace");
                    // get list of events from response
                    dynamic data = await response.Content.ReadAsAsync<object>();
                    this.Log.Info("response data: " + data);
                    var events = data?.events;

                    // test code, should read this from the response
                    //var dataString = "{\n    \"nextEventStartTms\": null,\n    \"nextEventId\": null,\n    \"nextCursor\": null,\n    \"from\": 1532067180000,\n    \"to\": 1532110380000,\n    \"totalEventCount\": 1,\n    \"events\": [\n	\n      {\n        \"eventId\": 7587387097431610000,        \"startTime\": 1532086141275,\n        \"endTime\": 153208614127,\n \n       \"entityId\": \"SERVICE - 8AC3A0761537D748\",\n        \"entityName\": \"SampleNodeJsService\",\n        \"severityLevel\": null,\n        \"impactLevel\": \"SERVICE\",\n        \"eventType\": \"CUSTOM_DEPLOYMENT\",\n        \"eventStatus\": \"CLOSED\",\n        \"tags\": [\n          {\n            \"context\": \"CONTEXTLESS\",\n            \"key\": \"DeploymentGroup\",\n            \"value\": \"Production\"\n          }\n        ],\n        \"id\": \"7587387097431610671_1532086141275\",\n        \"customProperties\": {\n          \"CodeDeploy.DeploymentGroup\": \"Production\",\n          \"PipelineStage\": \"Production\",\n          \"PipelineName\": \"SampleDevOpsPipeline\",\n    \"CodeDeploy.DeploymentId\": \"d - 4P826EJDU\",\n          \"CodeDeploy.Application\": \"SampleDevOpsApp\",\n         \"PipelineAction\": \"PushDynatraceDeploymentEvent\"\n        },\n        \"deploymentProject\": \"SampleDevOpsPipeline\",\n        \"remediationAction\": \"https://42g9h6vd6i.execute-api.us-east-1.amazonaws.com/v1/HandleDynatraceProblem\",\n        \"deploymentVersion\": \"78\",        \"deploymentName\": \"CodePipeline Deploying in Production\",\n        \"source\": \"VSTS\"\n      }\n    ]\n  }";
                    //dynamic data = JsonConvert.DeserializeObject(dataString);
                    //var events = data?.events;

                    // only look at most recent one that came from VSTS, once you find the first one, break out of for loop
                    this.Log.Info("Iterating through all events, find first one from VSTS");
                    foreach (var problemEvent in events)
                    {
                        this.Log.Info("ProblemEvent: " + problemEvent);
                        if (problemEvent?.source.Value.Equals("VSTS"))
                        {
                            // only push it if the same deployment id is not already on the list
                            var exists = resultEvents.Any(item => item?.deploymentVersion == problemEvent?.deploymentVersion);
                            if (!exists)
                            {
                                resultEvents.Add(problemEvent);
                            }
                            break;
                        }
                    }
                }

            }

            this.Log.Info("Finisehd with getMostRecentDeploymentOnEntity, resultEvents: " + resultEvents);
            return resultEvents.ToArray();
        }

    }
}