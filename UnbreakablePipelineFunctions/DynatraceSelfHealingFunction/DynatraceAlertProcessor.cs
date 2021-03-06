﻿using DynatraceSelfHealingFunction.VSTS;
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
            var DTTimeSpanMS = 60 * 1000 * Int32.Parse(this.DTTimeSpanMinutes);
            Log.Info("ProcessDynatraceAlert: notificationObject.ImpactedEnties: " + this.NotificationObject?.ImpactedEntities);

            FixedEvent[] fixedEvents = await fixMostRecentDeploymentsOnEntities(NotificationObject?.ImpactedEntities, DTTimeSpanMS);

            Log.Info("ProcessDynatraceAlert: fixed events, updating problem ticket");

            // we have our information and can now iterate and update the problem ticket
            if (fixedEvents.Length > 0)
            {
                foreach (var fixedEvent in fixedEvents)
                {
                    Log.Info("ProcessDynatraceAlert: fixedEvent: " + fixedEvent);
                    // create comment body
                    var commentBody = new FixedEventComment
                    {
                        comment = "Triggered release deploymentName: " + fixedEvent.OrigEvent?.deploymentName.Value + " RollbackReleaseId: " + fixedEvent.RollbackReleaseId + " in project " + fixedEvent.OrigEvent?.deploymentProject.Value,
                        user = "Dynatrace Self healing Function Remediation Action",
                        context = "Azure Function"
                    };
                    Log.Info("ProcessDynatraceAlert: Created FixedEventComment object: " + JsonConvert.SerializeObject(commentBody));

                    // post comment body to dynatrace problem comments
                    var fullUrl = this.DTTenantUrl + "/api/v1/problem/details/" + this.NotificationObject?.PID.Value + "/comments";
                    Log.Info("full url: " + fullUrl);
                    var response = await DynatraceAPIHelper.Post(fullUrl, this.DTApiToken, JsonConvert.SerializeObject(commentBody));
                    var statusCode = response.StatusCode;
                    var data = await response.Content.ReadAsStringAsync();
                    this.Log.Info("ProcessDynatraceAlert: Push comment to Dynatrace: " + fullUrl + " " + statusCode + "-" + data);
                    if ((int)statusCode != 200)
                    {
                        throw new Exception("ProcessDynatraceAlert: Error udpating Dynatrace deployment entity: " + data);
                    }
                }
            }
            else
            {
                throw new Exception("ProcessDynatraceAlert: There are no recent Dynatrace deployment entities to update");
            }

            return "Executed Handler successfully!";
        }

        private async Task<FixedEvent[]> fixMostRecentDeploymentsOnEntities(dynamic entities, int timespan)
        {
            Log.Info("fixMostRecentDeploymentsOnEntities: Start");

            if (entities.Count == 0)
            {
                this.Log.Info("fixMostRecentDeploymentsOnEntities: No entites passed, aborting");
                return new FixedEvent[0];
            }

            var returnArray = new List<FixedEvent>();

            dynamic[] mostRecentEvents = await getMostRecentDeploymentOnEntity(entities, timespan);
            this.Log.Info("fixMostRecentDeploymentsOnEntities: # of Most Recent Deployment Events: " + mostRecentEvents.Length);

            if (mostRecentEvents.Length > 0)
            {
                var vstsHelper = new VSTSHelper
                {
                    VSTSUrl = this.DTVSTSUrl,
                    VSTSPAT = this.DTVSTSPAT,
                    VSTSReleaseApiUrl = this.VSTSReleaseApiUrl,
                    Log = this.Log
                };

                this.Log.Info("fixMostRecentDeploymentsOnEntities: Iterating through deployment events and rolling back");
                foreach (var deployEvent in mostRecentEvents)
                {
                    this.Log.Info("fixMostRecentDeploymentsOnEntities: processing Problem event: " + deployEvent);
                    var releaseId = int.Parse(deployEvent?.deploymentVersion.Value);
                    var vstsTeamProject = deployEvent?.deploymentProject.Value;

                    // find the environment value within the array of tags
                    var environment = getEnvironmentTagValue(deployEvent);

                    this.Log.Info("fixMostRecentDeploymentsOnEntities: Rolling back release from VSTS");
                    this.Log.Info("fixMostRecentDeploymentsOnEntities: releaseId: " + releaseId);
                    this.Log.Info("fixMostRecentDeploymentsOnEntities: vstsTeamProject: " + vstsTeamProject);
                    this.Log.Info("fixMostRecentDeploymentsOnEntities: environment: " + environment);
                    var rollbackId = vstsHelper.ReleaseProblemDetected(vstsTeamProject, releaseId, environment);
                    returnArray.Add(new FixedEvent
                    {
                        RollbackReleaseId = rollbackId,
                        OrigEvent = deployEvent
                    });
                }
                this.Log.Info("fixMostRecentDeploymentsOnEntities: Done");
                return returnArray.ToArray();
            }
            else
            {
                throw new Exception("fixMostRecentDeploymentsOnEntities: Missing PID");
            }
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
                    break;
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
            this.Log.Info("getMostRecentDeploymentOnEntity: Start");
            List<dynamic> resultEvents = new List<dynamic>();

            this.Log.Info("getMostRecentDeploymentOnEntity: Iterating through " + entities.Count + " entities");
            foreach (var entity in entities)
            {
                this.Log.Info("getMostRecentDeploymentOnEntity: processing Entity: " + entity);
                // build out dynatrace query to get most recent deployment event for each entity
                var dtEventUrl = this.DTTenantUrl + "/api/v1/events";
                var to = DateTime.Now;
                var from = to.Subtract(new TimeSpan(0, 0, 0, 0, timespan));
                var queryString = "?entityId=" + entity?.entity + "&eventType=CUSTOM_DEPLOYMENT";
                if (timespan > 0)
                {
                    queryString += "&to=" + DynatraceAPIHelper.ConvertToJSSMilliseconds(to) + "&from=" + DynatraceAPIHelper.ConvertToJSSMilliseconds(from);
                }

                // execute query to dynatrace
                this.Log.Info("getMostRecentDeploymentOnEntity: Executing query to Dynatrace: " + dtEventUrl + queryString);
                HttpResponseMessage response = await DynatraceAPIHelper.Post(dtEventUrl + queryString, this.DTApiToken, null);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    this.Log.Info("getMostRecentDeploymentOnEntity: Got valid response from dynatrace");
                    // get list of events from response
                    dynamic data = await response.Content.ReadAsAsync<object>();
                    this.Log.Info("getMostRecentDeploymentOnEntity: dynatrace response data: " + data);
                    var events = data?.events;

                    // test code, should read this from the response
                    //var dataString = "{\n    \"nextEventStartTms\": null,\n    \"nextEventId\": null,\n    \"nextCursor\": null,\n    \"from\": 1532067180000,\n    \"to\": 1532110380000,\n    \"totalEventCount\": 1,\n    \"events\": [\n	\n      {\n        \"eventId\": 7587387097431610000,        \"startTime\": 1532086141275,\n        \"endTime\": 153208614127,\n \n       \"entityId\": \"SERVICE - 8AC3A0761537D748\",\n        \"entityName\": \"SampleNodeJsService\",\n        \"severityLevel\": null,\n        \"impactLevel\": \"SERVICE\",\n        \"eventType\": \"CUSTOM_DEPLOYMENT\",\n        \"eventStatus\": \"CLOSED\",\n        \"tags\": [\n          {\n            \"context\": \"CONTEXTLESS\",\n            \"key\": \"DeploymentGroup\",\n            \"value\": \"Production\"\n          }\n        ],\n        \"id\": \"7587387097431610671_1532086141275\",\n        \"customProperties\": {\n          \"CodeDeploy.DeploymentGroup\": \"Production\",\n          \"PipelineStage\": \"Production\",\n          \"PipelineName\": \"SampleDevOpsPipeline\",\n    \"CodeDeploy.DeploymentId\": \"d - 4P826EJDU\",\n          \"CodeDeploy.Application\": \"SampleDevOpsApp\",\n         \"PipelineAction\": \"PushDynatraceDeploymentEvent\"\n        },\n        \"deploymentProject\": \"SampleDevOpsPipeline\",\n        \"remediationAction\": \"https://42g9h6vd6i.execute-api.us-east-1.amazonaws.com/v1/HandleDynatraceProblem\",\n        \"deploymentVersion\": \"78\",        \"deploymentName\": \"CodePipeline Deploying in Production\",\n        \"source\": \"VSTS\"\n      }\n    ]\n  }";
                    //dynamic data = JsonConvert.DeserializeObject(dataString);
                    //var events = data?.events;

                    // only look at most recent one that came from VSTS, once you find the first one, break out of for loop
                    if (events.Count > 0)
                    {
                        this.Log.Info("getMostRecentDeploymentOnEntity: Iterating through " + events.Count + " events, find first one from VSTS");
                        Boolean foundEvent = false;
                        foreach (var problemEvent in events)
                        {
                            this.Log.Info("getMostRecentDeploymentOnEntity: ProblemEvent: " + problemEvent);
                            if (problemEvent?.source.Value.Equals("VSTS"))
                            {
                                // only push it if the same deployment id is not already on the list
                                var exists = resultEvents.Any(item => item?.deploymentVersion == problemEvent?.deploymentVersion);
                                if (!exists)
                                {
                                    resultEvents.Add(problemEvent);
                                    foundEvent = true;
                                }
                                break;
                            }
                        }
                        if (!foundEvent)
                        {
                            throw new Exception("getMostRecentDeploymentOnEntity: Found no events where problemEvent=VSTS and deploymentVersion of problem and deployments match");
                        }
                    }
                    else
                    {
                        throw new Exception("getMostRecentDeploymentOnEntity: Found no dynatrace deployment events");
                    }
                }
                else
                {
                    throw new Exception("getMostRecentDeploymentOnEntity: Error getting Dynatrace events. StatusCode: " + response.StatusCode);
                }
            }

            this.Log.Info("getMostRecentDeploymentOnEntity: finished. resultEvents: " + resultEvents);
            return resultEvents.ToArray();
        }

    }
}