using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceUnbreakablePipelineFunction
{
    public class InputData
    {
        public string planUrl { get; set; }
        public string projectId { get; set; }
        public string hubName { get; set; }
        public string planId { get; set; }
        public string jobId { get; set; }
        public string timelineId { get; set; }
        public string taskInstanceId { get; set; }
        public string authToken { get; set; }
        public string monspecUrl { get; set; }
        public string pipelineInfoUrl { get; set; }
        public string dynatraceTennantUrl { get; set; }
        public string dynatraceToken { get; set; }
        public string proxyUrl { get; set; }
        public string serviceToCompare { get; set; }
        public string compareWindow { get; set; }
    }
}
