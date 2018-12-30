using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DynatraceUnbreakablePipelineFunction
{
    public class InputData
    {
        [JsonProperty(Required = Required.Always)]
        public string planUrl { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string projectId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string hubName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string planId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string jobId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string timelineId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string taskInstanceId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string authToken { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string monspecUrl { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string pipelineInfoUrl { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string dynatraceTennantUrl { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string dynatraceToken { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string proxyUrl { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string serviceToCompare { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string compareWindow { get; set; }
    }
}
