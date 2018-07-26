using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceUnbreakablePipelineFunction
{
    public class UnbreakableGateResponse
    {
        public int totalViolations { get; set; }
        public PullCompareResponse pullCompareResponse { get; set; }
    }
}
