using System;
using DynatraceUnbreakablePipelineFunction;
using DynatraceUnbreakablePipelineFunction.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GateTest.Behaviors
{
    [TestClass]
    public class ExecutionObjectGetsCalled
    {
        [TestMethod]
        public void Gate_Helper_Created_With_Correct_Values()
        {
            //using(ShimsContext.Create())
            //{
            //    #region Arrange
            //    var theTaskInstanceId = string.Empty;
            //    var theHubName = string.Empty;
            //    var theJobId = string.Empty;
            //    var thePlanId = string.Empty;
            //    var theTimelineId = string.Empty;
            //    var theProjectId = string.Empty;
            //    var theVstsUrl = string.Empty;
            //    var theAuthToken = string.Empty;
            //    var gateHelperConstructorCalled = false;
            //    ShimGateHelper.ConstructorStringStringStringStringStringStringStringString = (
            //            self,
            //            taskInstanceId,
            //            hubName,
            //            jobId,
            //            planId,
            //            timelineId,
            //            projectId,
            //            vstsUrl,
            //            authToken
            //        ) =>
            //    {
            //        gateHelperConstructorCalled = true;
            //        theTaskInstanceId = taskInstanceId;
            //        theHubName = hubName;
            //        theJobId = jobId;
            //        thePlanId = planId;
            //        theTimelineId = timelineId;
            //        theProjectId = projectId;
            //        theVstsUrl = vstsUrl;
            //        theAuthToken = authToken;
            //    };

            //    #endregion

            //    #region Act
            //    var hubName = "hubname";
            //    var jobId = "jobid";
            //    var planId = "planid";
            //    var timelineId = "timelineid";
            //    var projectId = "projectid";
            //    var vstsUrl = "vstsurl";
            //    var authToken = "authtoken";

            //    var executionObj = new ExecuteObject(
            //        hubName,
            //        jobId,
            //        planId,
            //        timelineId,
            //        projectId,
            //        vstsUrl,
            //        null,
            //        authToken,
            //        null,
            //        null,
            //        null,
            //        null,
            //        null,
            //        null,
            //        null,
            //        null
            //    );

            //    executionObj.Execute();
            //    #endregion

            //    #region Assert
            //    #endregion
            //}
        }

        [TestMethod]
        public void Send_Querying_Dynatrace_Live_Log_Message()
        {
        }

        [TestMethod]
        public void Query_Dynatrace_With_Monspec()
        {

        }

        [TestMethod]
        public void Send_Finished_Querying_Monspec_Live_Log_Message()
        {

        }

        [TestMethod] 
        public void Response_Has_Violations_Fail_Gate()
        {

        }

        [TestMethod]
        public void Response_Has_No_Violations_Pass_Gate()
        {

        }
    }
}
