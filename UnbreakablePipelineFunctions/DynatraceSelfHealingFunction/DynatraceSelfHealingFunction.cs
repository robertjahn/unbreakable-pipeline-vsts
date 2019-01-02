using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
            log.Info("ProcessAlert: Starting...");

            // this is needed for testing outside a pipeline
            if (req.GetConfiguration() == null)
            {
                var configuration = new HttpConfiguration();
                req.SetConfiguration(configuration);
            }

            // Get request body
            try
            {
                // read request post body
                dynamic notificationObject = await req.Content.ReadAsAsync<object>();
                log.Info("ProcessAlert: notification object: " + notificationObject);

                // make sure all values needed are present
                if (notificationObject?.PID == null)
                {
                    log.Error("ProcessAlert: missing PID in notification object");
                    throw new Exception("ProcessAlert: Missing PID in notification object");
                }

                if (notificationObject?.ImpactedEntities == null)
                {
                    log.Error("ProcessAlert: Missing ImpactedEntites in notification object");
                    throw new Exception("ProcessAlert: Missing ImpactedEntities in notification object");
                }

                if (notificationObject?.State == null)
                {
                    log.Error("ProcessAlert: Missing State in notification object");
                    throw new Exception("ProcessAlert: Missing State in notification object");
                }

                // this indicates a Dynatrace Test Message, we just return that everything is ok
                var requestBody = await req.Content.ReadAsStringAsync();
                if (requestBody.Contains("XXXXXXXXXXXXX"))
                {
                    log.Info("ProcessAlert: Found testing message");
                    return req.CreateResponse(HttpStatusCode.OK, "All good from test message");
                }

                // we only do a rollback in case a new problem opens up. any other state, e.g: RESOLVED, MERGED doesnt require any action
                string state = notificationObject?.State;
                if (!state.Equals("OPEN"))
                {
                    log.Info("ProcessAlert: Nothing to do as problem status is " + state);
                    return req.CreateResponse(HttpStatusCode.OK, "Nothing to do as problem status is not OPEN. Current status is: " + state);
                }

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
                messageBuilder.AppendLine("ProcessAlert: Function application setting variables:");
                messageBuilder.AppendLine("ProcessAlert: DT Api Token: " + dtApiToken);
                messageBuilder.AppendLine("ProcessAlert: DT Tenant URL: " + dtTennantUrl);
                messageBuilder.AppendLine("ProcessAlert: DT VSTS Url: " + dtVSTSUrl);
                messageBuilder.AppendLine("ProcessAlert: DT VSTS PAT: " + dtVSTSPAT);
                messageBuilder.AppendLine("ProcessAlert: DT Time Span Minutes: " + dtTimeSpanMinutes);
                messageBuilder.AppendLine("ProcessAlert: DT VSTS Release API: " + dtVSTSReleaseApiUrl);
                log.Info(messageBuilder.ToString());

                if ((dtApiToken == null) || (dtApiToken == "")) { throw new Exception("DT_API_TOKEN environmental variables not defined"); }
                if ((dtTennantUrl == null) || (dtTennantUrl == "")) { throw new Exception("DT_TENANT_URL environmental variables not defined"); }
                if ((dtVSTSUrl == null) || (dtVSTSUrl == "")) { throw new Exception("DT_VSTSURL environmental variables not defined"); }
                if ((dtVSTSPAT == null) || (dtVSTSPAT == "")) { throw new Exception("DT_VSTSPAT environmental variables not defined"); }
                if ((dtVSTSReleaseApiUrl == null) || (dtVSTSReleaseApiUrl == "")) { throw new Exception("DT_VSTSRELEASEAPIURL environmental variables not defined"); }
                if ((dtTimeSpanMinutes == null) || (dtTimeSpanMinutes == "")) { throw new Exception("DT_VSTSTIMESPANMINUTES environmental variables not defined"); }
                if ((dtEnvironmentTag == null) || (dtEnvironmentTag == "")) { throw new Exception("DT_ENVIRONMENTTAG environmental variables not defined"); }

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

                log.Info("ProcessAlert: Created alert processor, starting to process dynatrace alert.");
                var returnMessage = await alertProcessor.ProcessDynatraceAlert();
                log.Info("ProcessAlert: Finished processing dynatrace alert: " + returnMessage);
                return req.CreateResponse(HttpStatusCode.OK, returnMessage);

            }
            catch (Exception e)
            {
                log.Error("ProcessAlert: Exception occured: " + e.Message + ":" + e.StackTrace);
                return req.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
