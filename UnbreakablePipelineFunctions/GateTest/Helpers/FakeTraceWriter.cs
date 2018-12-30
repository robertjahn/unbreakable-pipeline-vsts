using Microsoft.Azure.WebJobs.Host;
using System;
using System.Diagnostics;


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
