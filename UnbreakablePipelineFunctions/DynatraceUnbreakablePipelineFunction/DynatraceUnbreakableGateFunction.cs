using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
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

            // code to work around parsing issues
            TypeDescriptor.AddAttributes(typeof(IdentityDescriptor), new TypeConverterAttribute(typeof(IdentityDescriptorConverter).FullName));
            TypeDescriptor.AddAttributes(typeof(SubjectDescriptor), new TypeConverterAttribute(typeof(SubjectDescriptorConverter).FullName));
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            try
            {
                var contentString = await req.Content.ReadAsStringAsync();
                var input = JsonConvert.DeserializeObject<InputData>(contentString);
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
                    log);
                log.Info("DynatraceUnbreakableGateFunction: Parsed info from body of post and stored in executionObject");

                /*
                // async process gate
                var executionThread = new Thread(new ThreadStart(executionObject.Execute));
                log.Info("DynatraceUnbreakableGateFunction: Created Thread for processing");

                executionThread.Name = "Execution Thread";
                executionThread.Start();
                log.Info("DynatraceUnbreakableGateFunction: Started Thread for processing");
                */

                executionObject.Execute();

                // return back return object
                return HttpRequestProxy.CreateResponse(req, HttpStatusCode.OK, "Started Async processing of monspec...", "application/json");
            }
            catch (Exception e)
            {
                log.Error("DynatraceUnbreakableGateFunction: Raising Exception: " + e.Message);
                return HttpRequestProxy.CreateResponse(req, HttpStatusCode.BadRequest, "Exception: " + e.Message, "application/json");
            }
        }



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
