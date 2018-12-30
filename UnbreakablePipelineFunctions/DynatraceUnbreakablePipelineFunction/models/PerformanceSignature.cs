﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceUnbreakablePipelineFunction
{
    public class PerformanceSignature
    {
        public string validate { get; set; }
        public double result_compare { get; set; }
        public int violation { get; set; }
        public string timeseries { get; set; }
        public double threshold { get; set; }
        public string aggregate { get; set; }
        public double result { get; set; }
        public string smartscape { get; set; }
    }
}
