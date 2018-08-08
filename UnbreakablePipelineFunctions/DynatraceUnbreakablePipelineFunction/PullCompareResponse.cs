using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceUnbreakablePipelineFunction
{
    public class PullCompareResponse
    {
        public List<PerformanceSignature> performanceSignature { get; set; }
        public int totalViolations { get; set; }
        public string comment { get; set; }
    }
}
