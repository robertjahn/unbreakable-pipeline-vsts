using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DynatraceSelfHealingFunction
{
    public static class DynatraceSelfHealingFunction
    {
        [FunctionName("ProcessAlert")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // Get request body
            try
            {
                // read request post body
                dynamic notificationObject = await req.Content.ReadAsAsync<object>();
                log.Info("notification object: " + notificationObject);

                // make sure all values needed are present
                if (notificationObject?.PID == null)
                {
                    log.Error("missing PID");
                    throw new Exception("Missing PID");
                }

                if (notificationObject?.ImpactedEntities == null)
                {
                    log.Error("Missing ImpactedEntites");
                    throw new Exception("Missing ImpactedEntities");
                }

                if (notificationObject?.State == null)
                {
                    log.Error("Missing State");
                    throw new Exception("Missing State");
                }

                // this indicates a Dynatrace Test Message, we just return that everything is ok
                var requestBody = await req.Content.ReadAsStringAsync();
                if (requestBody.Contains("XXXXXXXXXXXXX"))
                {
                    log.Info("From Testing");
                    return req.CreateResponse(HttpStatusCode.OK, "All good from test message");
                }

                // we only do a rollback in case a new problem opens up. any other state, e.g: RESOLVED, MERGED doesnt require any action
                string state = notificationObject?.State;
                if (!state.Equals("OPEN"))
                {
                    log.Info("Nothing to do as problem status is " + state);
                    return req.CreateResponse(HttpStatusCode.OK, "Nothing to do as problem status is " + state);
                }

                // Process the dynatrace event
                // get environmental variables
                var dtApiToken = System.Environment.GetEnvironmentVariable("DT_API_TOKEN", EnvironmentVariableTarget.Process);
                var dtTennantUrl = System.Environment.GetEnvironmentVariable("DT_TENANT_URL", EnvironmentVariableTarget.Process);
                var dtVSTSUrl = System.Environment.GetEnvironmentVariable("DT_VSTSURL", EnvironmentVariableTarget.Process);
                var dtVSTSPAT = System.Environment.GetEnvironmentVariable("DT_VSTSPAT", EnvironmentVariableTarget.Process);
                var dtVSTSReleaseApiUrl = System.Environment.GetEnvironmentVariable("DT_VSTSRELEASEAPIURL", EnvironmentVariableTarget.Process);
                var dtTimeSpanMinutes = System.Environment.GetEnvironmentVariable("DT_VSTSTIMESPANMINUTES", EnvironmentVariableTarget.Process);
                var dtEnvironmentTag = System.Environment.GetEnvironmentVariable("DT_ENVIRONMENTTAG", EnvironmentVariableTarget.Process);
                var messageBuilder = new StringBuilder();

                // create log message
                messageBuilder.AppendLine("Got environmental variables:");
                messageBuilder.AppendLine("DT Api Token: " + dtApiToken);
                messageBuilder.AppendLine("DT Tenant URL: " + dtTennantUrl);
                messageBuilder.AppendLine("DT VSTS Url: " + dtVSTSUrl);
                messageBuilder.AppendLine("DT VSTS PAT: " + dtVSTSPAT);
                messageBuilder.AppendLine("DT Time Span Minutes: " + dtTimeSpanMinutes);
                messageBuilder.AppendLine("DT VSTS Release API: " + dtVSTSReleaseApiUrl);
                log.Info(messageBuilder.ToString());

                var alertProcessor = new DynatraceAlertProcessor
                {
                    Log = log,
                    DTApiToken = dtApiToken,
                    DTTenantUrl = dtTennantUrl,
                    DTTimeSpanMinutes = dtTimeSpanMinutes,
                    VSTSReleaseApiUrl = dtVSTSReleaseApiUrl,
                    DTEnvironmentTag = dtEnvironmentTag,
                    Request = req,
                    NotificationObject = notificationObject,
                    DTVSTSUrl = dtVSTSUrl,
                    DTVSTSPAT = dtVSTSPAT
                };

                log.Info("Created alert processor, starting to process dynatrace alert.");
                var returnMessage = await alertProcessor.ProcessDynatraceAlert();
                log.Info("Finished processing dynatrace alert: " + returnMessage);
                return req.CreateResponse(HttpStatusCode.OK, returnMessage);

            }
            catch (Exception e)
            {
                log.Error("Exception occured: " + e.Message + ":" + e.StackTrace);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
