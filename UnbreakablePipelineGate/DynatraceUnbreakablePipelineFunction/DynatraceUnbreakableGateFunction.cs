using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a dynatrace unbreakable gate.");

            // code to work around parsing issues
            TypeDescriptor.AddAttributes(typeof(IdentityDescriptor), new TypeConverterAttribute(typeof(IdentityDescriptorConverter).FullName));
            TypeDescriptor.AddAttributes(typeof(SubjectDescriptor), new TypeConverterAttribute(typeof(SubjectDescriptorConverter).FullName));
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            // read info from body of post
            var contentString = await req.Content.ReadAsStringAsync();
            var input = JsonConvert.DeserializeObject<InputData>(contentString);

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
            
            // async process gate
            var executionThread = new Thread(new ThreadStart(executionObject.Execute));
            executionThread.Start();

            // return back return object
            return req.CreateResponse(HttpStatusCode.OK, "processing monspec...", "application/json");

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
