using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DynatraceUnbreakablePipelineFunction.Proxies;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;

namespace DynatraceUnbreakablePipelineFunction
{
    public static class DynatraceUnbreakableGateFunction
    {
        [FunctionName("ProcessUnbreakableGate")]
        public async static Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("DynatraceUnbreakableGateFunction: Start");

            // TODO - determine if/why these are here
            // code to work around parsing issues
            TypeDescriptor.AddAttributes(typeof(IdentityDescriptor), new TypeConverterAttribute(typeof(IdentityDescriptorConverter).FullName));
            TypeDescriptor.AddAttributes(typeof(SubjectDescriptor), new TypeConverterAttribute(typeof(SubjectDescriptorConverter).FullName));
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // response is a JSON object with status and comment
            CompareResponse jsonResponse = new CompareResponse();
            try
            {
                var contentString = await req.Content.ReadAsStringAsync();
                var input = JsonConvert.DeserializeObject<InputData>(contentString);

                // TODO - remove all VSTS variables and arguments related to async if not going to use this approach
                log.Info("DynatraceUnbreakableGateFunction: Read info from body of post");
                var executionObject = new ExecuteObject(
                    input.hubName,
                    input.jobId,
                    input.planId,
                    input.timelineId,
                    input.projectId,
                    input.planUrl,
                    input.taskInstanceId,
                    input.authToken,
                    input.monspecUrl,
                    input.pipelineInfoUrl,
                    input.compareWindow,
                    input.dynatraceTennantUrl,
                    input.dynatraceToken,
                    input.proxyUrl,
                    input.serviceToCompare,
                    input.compareShift,
                    input.compareType,
                    log);
                log.Info("DynatraceUnbreakableGateFunction: Parsed info from body of post and stored in executionObject");

                var compareReturnString = executionObject.Compare();
                if (compareReturnString == "")
                {
                    jsonResponse.status = "success";
                    jsonResponse.comment = "";
                }
                else
                {
                    jsonResponse.status = "failure";
                    jsonResponse.comment = compareReturnString;
                }
                log.Info("DynatraceUnbreakableGateFunction: response: " + JsonConvert.SerializeObject(jsonResponse));
                return HttpRequestProxy.CreateResponse(req, HttpStatusCode.OK, JsonConvert.SerializeObject(jsonResponse), "application/json");

            }
            catch (Exception e)
            {
                jsonResponse.status = "exception";
                jsonResponse.comment = "Exception: " + e.Message;
                log.Error("DynatraceUnbreakableGateFunction: response: " + JsonConvert.SerializeObject(jsonResponse));
                return HttpRequestProxy.CreateResponse(req, HttpStatusCode.InternalServerError, JsonConvert.SerializeObject(jsonResponse), "application/json");
            }
        }


        // TODO - determine if this needed
        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Microsoft.VisualStudio.Services.WebApi"))
            {
                return typeof(IdentityDescriptor).Assembly;
            }
            return null;
        }
    }
}
