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
namespace DynatraceUnbreakablePipelineFunction.Proxies
{
    public static class HttpRequestProxy
    {

        public static HttpResponseMessage CreateResponse(HttpRequestMessage req, HttpStatusCode code, string msg, string responseType)
        {
            return req.CreateResponse(code, msg, responseType);
        }
    }
}
