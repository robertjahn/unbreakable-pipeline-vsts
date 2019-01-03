using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateTest.Helpers
{
    public static class FakeRequestContent
    {
        public static string TriggerContent = "{ \"Content-Type\":\"application/json\", \"PlanUrl\": \"ThePlanUrl\", \"ProjectId\": \"TheProjectId\", \"HubName\": \"TheHubName\", \"PlanId\": \"ThePlanId\", \"JobId\": \"TheJobId\", \"TimelineId\": \"TheTimelineId\", \"TaskInstanceId\": \"TheTaskInstanceId\", \"AuthToken\": \"TheAuthToken\", \"monspecUrl\": \"TheMonspecUrl\", \"pipelineInfoUrl\": \"ThePipelineUrl\", \"dynatraceTennantUrl\": \"TheDynatraceTenantUrl\", \"dynatraceToken\": \"TheDynatraceToken\", \"proxyUrl\": \"TheProxyUrl\", \"serviceToCompare\":\"TheServiceToCompare\", \"compareWindow\": \"5\", \"compareShift\": \"5\", \"compareType\": \"pullcompare\" }";
    }
}
