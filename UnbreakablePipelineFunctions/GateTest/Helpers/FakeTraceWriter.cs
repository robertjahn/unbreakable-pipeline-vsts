using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateTest.Helpers
{
    public class FakeTraceWriter : TraceWriter
    {
        public FakeTraceWriter(TraceLevel level) : base(level)
        {
        }

        public override void Trace(TraceEvent traceEvent)
        {
            throw new NotImplementedException();
        }
    }
}
