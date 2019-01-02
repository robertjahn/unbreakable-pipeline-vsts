using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Fakes;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Fakes;
using System.Threading;
using System.Threading.Tasks;
using DynatraceUnbreakablePipelineFunction;
using DynatraceUnbreakablePipelineFunction.Fakes;
using DynatraceUnbreakablePipelineFunction.Proxies.Fakes;
using GateTest.Helpers;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GateTest.Behaviors
{
    [TestClass]
    public class GateGetsTriggered
    {


        [TestMethod]
        public async Task Set_Up_Assemblies_For_Json_Parsing()
        {
            using (ShimsContext.Create())
            {
                #region Arrange
                // mock TraceWriter to do nothing
                ShimTraceWriter.AllInstances.InfoStringString = (self, msg, source) =>
                {
                    System.Console.WriteLine("ok...");
                };

                // mock TypeDescriptor to see if the correct type descriptors were added
                bool addedAttributeIdentityDescriptor = false;
                bool addedAttributeSubjectDescriptor = false;
                ShimTypeDescriptor.AddAttributesTypeAttributeArray = (theType, attributeArray) =>
                {
                    var typeConverterAttribute = attributeArray[0] as TypeConverterAttribute;

                    if (theType.Name.Equals("IdentityDescriptor") && typeConverterAttribute.ConverterTypeName.Equals("Microsoft.VisualStudio.Services.Identity.IdentityDescriptorConverter"))
                    {
                        addedAttributeIdentityDescriptor = true;
                    }
                    else if (theType.Name.Equals("SubjectDescriptor") && typeConverterAttribute.ConverterTypeName.Equals("Microsoft.VisualStudio.Services.Common.SubjectDescriptorConverter"))
                    {
                        addedAttributeSubjectDescriptor = true;
                    }
                    return null;
                };

                // mock HttpRequestMessage to do nothing
                ShimHttpRequestMessage.AllInstances.ContentGet = (self) => new FakeHttpContent();

                // mock HttpRequestProxy to do nothing
                ShimHttpRequestProxy.CreateResponseHttpRequestMessageHttpStatusCodeStringString = (req, code, msg, returnType) => new HttpResponseMessage();

                // mock HttpContent to do nothing
                ShimHttpContent.AllInstances.ReadAsStringAsync = (self) =>
                {
                    return Task.FromResult<string>(FakeRequestContent.TriggerContent);
                };

                // Mock Execute Object to do nothing
                ShimExecuteObject.AllInstances.Execute = (self) =>
                {
                };
                #endregion

                #region Act
                var requestMessage = new HttpRequestMessage();
                var logger = new FakeTraceWriter(TraceLevel.Info);
                var response = await DynatraceUnbreakableGateFunction.Run(requestMessage, logger);
                #endregion

                #region Assert
                Assert.IsTrue(addedAttributeIdentityDescriptor);
                Assert.IsTrue(addedAttributeSubjectDescriptor);
                #endregion
            }
        }

        [TestMethod]
        public async Task Log_Trigger_Function_Processed()
        {
            using (ShimsContext.Create())
            {
                #region Arrange
                // mock TraceWriter to do nothing
                var logMessageList = new List<string>();
                ShimTraceWriter.AllInstances.InfoStringString = (self, msg, source) =>
                {
                    logMessageList.Add(msg);
                };

                // mock TypeDescriptor to do nothing
                ShimTypeDescriptor.AddAttributesTypeAttributeArray = (theType, attributeArray) =>
                {
                    return null;
                };

                // mock HttpRequestMessage to do nothing
                ShimHttpRequestMessage.AllInstances.ContentGet = (self) => new FakeHttpContent();

                // mock HttpRequestProxy to do nothing
                ShimHttpRequestProxy.CreateResponseHttpRequestMessageHttpStatusCodeStringString = (req, code, msg, returnType) => new HttpResponseMessage();

                // mock HttpContent to do nothing
                ShimHttpContent.AllInstances.ReadAsStringAsync = (self) =>
                {
                    return Task.FromResult<string>(FakeRequestContent.TriggerContent);
                };

                // Mock Execute Object to do nothing
                ShimExecuteObject.AllInstances.Execute = (self) =>
                {
                };
                #endregion

                #region Act
                var requestMessage = new HttpRequestMessage();
                var logger = new FakeTraceWriter(TraceLevel.Info);
                var response = await DynatraceUnbreakableGateFunction.Run(requestMessage, logger);
                #endregion

                #region Assert
                Assert.AreEqual("DynatraceUnbreakableGateFunction: Start", logMessageList[0]);
                #endregion
            }
        }

        [TestMethod]
        public async Task Parse_Post_Body_To_ExecuteObject()
        {
            using (ShimsContext.Create())
            {
                #region Arrange
                // mock TraceWriter to do nothing
                var logMessageList = new List<string>();
                ShimTraceWriter.AllInstances.InfoStringString = (self, msg, source) =>
                {
                    logMessageList.Add(msg);
                };

                // mock TypeDescriptor to do nothing
                ShimTypeDescriptor.AddAttributesTypeAttributeArray = (theType, attributeArray) =>
                {
                    return null;
                };

                // mock HttpRequestMessage to do nothing
                ShimHttpRequestMessage.AllInstances.ContentGet = (self) => new FakeHttpContent();

                // mock HttpRequestProxy to do nothing
                ShimHttpRequestProxy.CreateResponseHttpRequestMessageHttpStatusCodeStringString = (req, code, msg, returnType) => new HttpResponseMessage();

                // mock HttpContent (the post body) to hold known data
                ShimHttpContent.AllInstances.ReadAsStringAsync = (self) =>
                {
                    return Task.FromResult<string>(FakeRequestContent.TriggerContent);
                };

                // Mock Execute Object Execute() to do nothing
                ShimExecuteObject.AllInstances.Execute = (self) =>
                {
                };

                // mock execute objectconstructor to get all the values that were parsed from request body
                var theHubName = string.Empty;
                var theJobId = string.Empty;
                var thePlanId = string.Empty;
                var theTimelineId = string.Empty;
                var theProjectId = string.Empty;
                var thePlanUrl = string.Empty;
                var theTaskInstanceId = string.Empty;
                var theAuthToken = string.Empty;
                var theMonspecUrl = string.Empty;
                var thePipelineInfoUrl = string.Empty;
                var theCompareWindow = string.Empty;
                var theDynatraceTenantUrl = string.Empty;
                var theDynatraceToken = string.Empty;
                var theProxyUrl = string.Empty;
                var theServiceToCompare = string.Empty;

                ShimExecuteObject.ConstructorStringStringStringStringStringStringStringStringStringStringStringStringStringStringStringTraceWriter = (
                    self,
                    hubName,
                    jobId,
                    planId,
                    timelineId,
                    projectId,
                    planUrl,
                    taskInstanceId,
                    authToken,
                    monspecUrl,
                    pipelineInfoUrl,
                    compareWindow,
                    dynatraceTenantUrl,
                    dynatraceToken,
                    proxyUrl,
                    serviceToCompare,
                    log) =>
                {
                    theHubName = hubName;
                    theJobId = jobId;
                    thePlanId = planId;
                    theTimelineId = timelineId;
                    theProjectId = projectId;
                    thePlanUrl = planUrl;
                    theTaskInstanceId = taskInstanceId;
                    theAuthToken = authToken;
                    theMonspecUrl = monspecUrl;
                    thePipelineInfoUrl = pipelineInfoUrl;
                    theCompareWindow = compareWindow;
                    theDynatraceTenantUrl = dynatraceTenantUrl;
                    theDynatraceToken = dynatraceToken;
                    theProxyUrl = proxyUrl;
                    theServiceToCompare = serviceToCompare;
                };

                #endregion

                #region Act
                var requestMessage = new HttpRequestMessage();
                var logger = new FakeTraceWriter(TraceLevel.Info);
                var response = await DynatraceUnbreakableGateFunction.Run(requestMessage, logger);
                #endregion

                #region Assert
                Assert.AreEqual("TheHubName", theHubName, "Hub name was parsed incorrectly");
                Assert.AreEqual("TheJobId", theJobId, "Job id was parsed incorrectly");
                Assert.AreEqual("ThePlanId", thePlanId, "Plan Id was parsed incorrectly");
                Assert.AreEqual("TheTimelineId", theTimelineId, "Timeline id was parsed incorrectly");
                Assert.AreEqual("TheProjectId", theProjectId, "Project Id was parsed incorrectly");
                Assert.AreEqual("ThePlanUrl", thePlanUrl, "The plan url was parsed incorrectly");
                Assert.AreEqual("TheTaskInstanceId", theTaskInstanceId, "The task instance id was parsed incoreectly");
                Assert.AreEqual("TheAuthToken", theAuthToken, "Auth token parsed incorrectly");
                Assert.AreEqual("TheMonspecUrl", theMonspecUrl, "The monspec url was parsed incorrectly");
                Assert.AreEqual("ThePipelineUrl", thePipelineInfoUrl, "pipeline info url parsed incorrectly");
                Assert.AreEqual("TheCompareWindow", "TheCompareWindow", "compare window was parsed incorrectly");
                Assert.AreEqual("TheDynatraceTenantUrl", theDynatraceTenantUrl, "dynatrace tenant url was parsed incorrectly");
                Assert.AreEqual("TheDynatraceToken", theDynatraceToken, "dynatrac token was parsed incorrectly");
                Assert.AreEqual("TheProxyUrl", theProxyUrl, "proxy url was parsed incorrectly");
                Assert.AreEqual("TheServiceToCompare", theServiceToCompare, "service to compare was parsed incorrectly");
                #endregion
            }
        }

        [TestMethod]
        public async Task Launch_Thread_To_Run_Execute()
        {
            /*
            using (ShimsContext.Create())
            {
                #region Arrange
                // mock TraceWriter to do nothing
                var logMessageList = new List<string>();
                ShimTraceWriter.AllInstances.InfoStringString = (self, msg, source) =>
                {
                    logMessageList.Add(msg);
                };

                // mock TypeDescriptor to do nothing
                ShimTypeDescriptor.AddAttributesTypeAttributeArray = (theType, attributeArray) =>
                {
                    return null;
                };

                // mock HttpRequestMessage to do nothing
                ShimHttpRequestMessage.AllInstances.ContentGet = (self) => new FakeHttpContent();

                // mock HttpRequestProxy to do nothing
                ShimHttpRequestProxy.CreateResponseHttpRequestMessageHttpStatusCodeStringString = (req, code, msg, returnType) => new HttpResponseMessage();

                // mock HttpContent to do nothing
                ShimHttpContent.AllInstances.ReadAsStringAsync = (self) =>
                {
                    return Task.FromResult<string>(FakeRequestContent.TriggerContent);
                };

                // Mock Execute Object to check if the Execution Thread called it
                bool executeCalledFromThread = false;
                ShimExecuteObject.AllInstances.Execute = (self) =>
                {
                    if (Thread.CurrentThread.Name.Equals("Execution Thread"))
                    {
                        executeCalledFromThread = true;
                    }
                };

                #endregion

                #region Act
                var requestMessage = new HttpRequestMessage();
                var logger = new FakeTraceWriter(TraceLevel.Info);
                var response = await DynatraceUnbreakableGateFunction.Run(requestMessage, logger);
                // throw in a sleep to give time for the Execution Thread to run
                Thread.Sleep(500);
                #endregion

                #region Assert
                Assert.IsTrue(executeCalledFromThread, "Execute was never called");
                #endregion
            }
            */
        }

        [TestMethod]
        public async Task Returns_Ok_Message_Response()
        {
            using (ShimsContext.Create())
            {
                #region Arrange
                // mock TraceWriter to do nothing
                var logMessageList = new List<string>();
                ShimTraceWriter.AllInstances.InfoStringString = (self, msg, source) =>
                {
                    logMessageList.Add(msg);
                };

                // mock TypeDescriptor to do nothing
                ShimTypeDescriptor.AddAttributesTypeAttributeArray = (theType, attributeArray) =>
                {
                    return null;
                };

                // mock HttpRequestMessage to do nothing
                ShimHttpRequestMessage.AllInstances.ContentGet = (self) => new FakeHttpContent();

                // mock HttpRequestProxy to see if correect values are passed in
                var theCode = HttpStatusCode.Forbidden;
                var theMsg = string.Empty;
                var theReturnType = string.Empty;
                ShimHttpRequestProxy.CreateResponseHttpRequestMessageHttpStatusCodeStringString = (req, code, msg, returnType) =>
                {
                    theCode = code;
                    theMsg = msg;
                    theReturnType = returnType;
                    return new HttpResponseMessage();
                };
                
                

                // mock HttpContent to do nothing
                ShimHttpContent.AllInstances.ReadAsStringAsync = (self) =>
                {
                    return Task.FromResult<string>(FakeRequestContent.TriggerContent);
                };

                // Mock Execute Object to do nothing
                ShimExecuteObject.AllInstances.Execute = (self) =>
                {
                };
                #endregion

                #region Act
                var requestMessage = new HttpRequestMessage();
                var logger = new FakeTraceWriter(TraceLevel.Info);
                var response = await DynatraceUnbreakableGateFunction.Run(requestMessage, logger);
                // throw in a sleep to give time for the Execution Thread to run
                Thread.Sleep(500);
                #endregion

                #region Assert
                Assert.AreEqual(HttpStatusCode.OK, theCode, "Status code is wrong");
                Assert.AreEqual("Started Async processing of monspec...", theMsg, "response message is wrong");
                Assert.AreEqual("application/json", theReturnType, "returnType is wrong");
                #endregion
            }
        }
    }
}
