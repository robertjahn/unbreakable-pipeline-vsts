using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceSelfHealingFunction.VSTS.ApiObjects.Releases.Redeploy
{

    public class RedeployRootObject
    {
        public int id { get; set; }
        public int releaseId { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public Variables variables { get; set; }
        public object[] variableGroups { get; set; }
        public Predeployapproval[] preDeployApprovals { get; set; }
        public Postdeployapproval[] postDeployApprovals { get; set; }
        public Preapprovalssnapshot preApprovalsSnapshot { get; set; }
        public Postapprovalssnapshot postApprovalsSnapshot { get; set; }
        public Deploystep[] deploySteps { get; set; }
        public int rank { get; set; }
        public int definitionEnvironmentId { get; set; }
        public Environmentoptions environmentOptions { get; set; }
        public object[] demands { get; set; }
        public Condition[] conditions { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public object[] workflowTasks { get; set; }
        public Deployphasessnapshot[] deployPhasesSnapshot { get; set; }
        public Owner owner { get; set; }
        public object[] schedules { get; set; }
        public Release release { get; set; }
        public Releasedefinition releaseDefinition { get; set; }
        public Releasecreatedby releaseCreatedBy { get; set; }
        public string triggerReason { get; set; }
        public float timeToDeploy { get; set; }
        public Processparameters processParameters { get; set; }
        public Predeploymentgatessnapshot preDeploymentGatesSnapshot { get; set; }
        public Postdeploymentgatessnapshot postDeploymentGatesSnapshot { get; set; }
    }

    public class Variables
    {
        public DynatraceDeploymentgroupname DynatraceDeploymentGroupName { get; set; }
        public DynatraceMonspecurl DynatraceMonspecUrl { get; set; }
    }

    public class DynatraceDeploymentgroupname
    {
        public string value { get; set; }
    }

    public class DynatraceMonspecurl
    {
        public string value { get; set; }
    }

    public class Preapprovalssnapshot
    {
        public Approval[] approvals { get; set; }
        public Approvaloptions approvalOptions { get; set; }
    }

    public class Approvaloptions
    {
        public object requiredApproverCount { get; set; }
        public bool releaseCreatorCanBeApprover { get; set; }
        public bool autoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }
        public bool enforceIdentityRevalidation { get; set; }
        public int timeoutInMinutes { get; set; }
        public string executionOrder { get; set; }
    }

    public class Approval
    {
        public int rank { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int id { get; set; }
    }

    public class Postapprovalssnapshot
    {
        public Approval1[] approvals { get; set; }
        public Approvaloptions1 approvalOptions { get; set; }
    }

    public class Approvaloptions1
    {
        public object requiredApproverCount { get; set; }
        public bool releaseCreatorCanBeApprover { get; set; }
        public bool autoTriggeredAndPreviousEnvironmentApprovedCanBeSkipped { get; set; }
        public bool enforceIdentityRevalidation { get; set; }
        public int timeoutInMinutes { get; set; }
        public string executionOrder { get; set; }
    }

    public class Approval1
    {
        public int rank { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int id { get; set; }
    }

    public class Environmentoptions
    {
        public string emailNotificationType { get; set; }
        public string emailRecipients { get; set; }
        public bool skipArtifactsDownload { get; set; }
        public int timeoutInMinutes { get; set; }
        public bool enableAccessToken { get; set; }
        public bool publishDeploymentStatus { get; set; }
        public bool badgeEnabled { get; set; }
        public bool autoLinkWorkItems { get; set; }
        public bool pullRequestDeploymentEnabled { get; set; }
    }

    public class Owner
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links
    {
        public Avatar avatar { get; set; }
    }

    public class Avatar
    {
        public string href { get; set; }
    }

    public class Release
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links1 _links { get; set; }
    }

    public class _Links1
    {
        public Web web { get; set; }
        public Self self { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Releasedefinition
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public _Links2 _links { get; set; }
    }

    public class _Links2
    {
        public Web1 web { get; set; }
        public Self1 self { get; set; }
    }

    public class Web1
    {
        public string href { get; set; }
    }

    public class Self1
    {
        public string href { get; set; }
    }

    public class Releasecreatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links3 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links3
    {
        public Avatar1 avatar { get; set; }
    }

    public class Avatar1
    {
        public string href { get; set; }
    }

    public class Processparameters
    {
    }

    public class Predeploymentgatessnapshot
    {
        public int id { get; set; }
        public object gatesOptions { get; set; }
        public object[] gates { get; set; }
    }

    public class Postdeploymentgatessnapshot
    {
        public int id { get; set; }
        public Gatesoptions gatesOptions { get; set; }
        public Gate[] gates { get; set; }
    }

    public class Gatesoptions
    {
        public bool isEnabled { get; set; }
        public int timeout { get; set; }
        public int samplingInterval { get; set; }
        public int stabilizationTime { get; set; }
        public int minimumSuccessDuration { get; set; }
    }

    public class Gate
    {
        public Task[] tasks { get; set; }
    }

    public class Task
    {
        public string taskId { get; set; }
        public string version { get; set; }
        public string name { get; set; }
        public bool enabled { get; set; }
        public bool alwaysRun { get; set; }
        public bool continueOnError { get; set; }
        public int timeoutInMinutes { get; set; }
        public string definitionType { get; set; }
        public Overrideinputs overrideInputs { get; set; }
        public string condition { get; set; }
        public Inputs inputs { get; set; }
    }

    public class Overrideinputs
    {
    }

    public class Inputs
    {
        public string connectedServiceName { get; set; }
        public string method { get; set; }
        public string headers { get; set; }
        public string body { get; set; }
        public string urlSuffix { get; set; }
        public string waitForCompletion { get; set; }
        public string successCriteria { get; set; }
    }

    public class Predeployapproval
    {
        public int id { get; set; }
        public int revision { get; set; }
        public string approvalType { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int trialNumber { get; set; }
        public int attempt { get; set; }
        public int rank { get; set; }
        public Release1 release { get; set; }
        public Releasedefinition1 releaseDefinition { get; set; }
        public Releaseenvironment releaseEnvironment { get; set; }
        public string url { get; set; }
    }

    public class Release1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links4 _links { get; set; }
    }

    public class _Links4
    {
    }

    public class Releasedefinition1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public _Links5 _links { get; set; }
    }

    public class _Links5
    {
    }

    public class Releaseenvironment
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links6 _links { get; set; }
    }

    public class _Links6
    {
    }

    public class Postdeployapproval
    {
        public int id { get; set; }
        public int revision { get; set; }
        public string approvalType { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public string status { get; set; }
        public string comments { get; set; }
        public bool isAutomated { get; set; }
        public bool isNotificationOn { get; set; }
        public int trialNumber { get; set; }
        public int attempt { get; set; }
        public int rank { get; set; }
        public Release2 release { get; set; }
        public Releasedefinition2 releaseDefinition { get; set; }
        public Releaseenvironment1 releaseEnvironment { get; set; }
        public string url { get; set; }
    }

    public class Release2
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links7 _links { get; set; }
    }

    public class _Links7
    {
    }

    public class Releasedefinition2
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public _Links8 _links { get; set; }
    }

    public class _Links8
    {
    }

    public class Releaseenvironment1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links9 _links { get; set; }
    }

    public class _Links9
    {
    }

    public class Deploystep
    {
        public int id { get; set; }
        public int deploymentId { get; set; }
        public int attempt { get; set; }
        public string reason { get; set; }
        public string status { get; set; }
        public string operationStatus { get; set; }
        public Releasedeployphas[] releaseDeployPhases { get; set; }
        public Requestedby requestedBy { get; set; }
        public Requestedfor requestedFor { get; set; }
        public DateTime queuedOn { get; set; }
        public Lastmodifiedby lastModifiedBy { get; set; }
        public DateTime lastModifiedOn { get; set; }
        public bool hasStarted { get; set; }
        public object[] tasks { get; set; }
        public string runPlanId { get; set; }
        public Postdeploymentgates postDeploymentGates { get; set; }
        public object[] issues { get; set; }
    }

    public class Requestedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links10 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links10
    {
        public Avatar2 avatar { get; set; }
    }

    public class Avatar2
    {
        public string href { get; set; }
    }

    public class Requestedfor
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links11 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links11
    {
        public Avatar3 avatar { get; set; }
    }

    public class Avatar3
    {
        public string href { get; set; }
    }

    public class Lastmodifiedby
    {
        public string displayName { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string descriptor { get; set; }
        public string url { get; set; }
        public _Links12 _links { get; set; }
        public string imageUrl { get; set; }
    }

    public class _Links12
    {
        public Avatar4 avatar { get; set; }
    }

    public class Avatar4
    {
        public string href { get; set; }
    }

    public class Postdeploymentgates
    {
        public int id { get; set; }
        public string status { get; set; }
        public string runPlanId { get; set; }
        public Deploymentjob[] deploymentJobs { get; set; }
        public DateTime startedOn { get; set; }
        public DateTime lastModifiedOn { get; set; }
        public DateTime stabilizationCompletedOn { get; set; }
        public DateTime succeedingSince { get; set; }
    }

    public class Deploymentjob
    {
        public Job job { get; set; }
        public Task1[] tasks { get; set; }
    }

    public class Job
    {
        public int id { get; set; }
        public string timelineRecordId { get; set; }
        public string name { get; set; }
        public DateTime dateStarted { get; set; }
        public DateTime dateEnded { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public string status { get; set; }
        public object rank { get; set; }
        public object[] issues { get; set; }
    }

    public class Task1
    {
        public int id { get; set; }
        public string timelineRecordId { get; set; }
        public string name { get; set; }
        public DateTime dateStarted { get; set; }
        public DateTime dateEnded { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public string status { get; set; }
        public object rank { get; set; }
        public object[] issues { get; set; }
        public Task2 task { get; set; }
        public string logUrl { get; set; }
    }

    public class Task2
    {
        public string id { get; set; }
        public string name { get; set; }
        public string version { get; set; }
    }

    public class Releasedeployphas
    {
        public int id { get; set; }
        public string phaseId { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
        public string phaseType { get; set; }
        public string status { get; set; }
        public string runPlanId { get; set; }
        public Deploymentjob1[] deploymentJobs { get; set; }
        public object[] manualInterventions { get; set; }
    }

    public class Deploymentjob1
    {
        public Job1 job { get; set; }
        public Task3[] tasks { get; set; }
    }

    public class Job1
    {
        public int id { get; set; }
        public string timelineRecordId { get; set; }
        public string name { get; set; }
        public DateTime dateStarted { get; set; }
        public DateTime dateEnded { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public string status { get; set; }
        public int rank { get; set; }
        public object[] issues { get; set; }
        public string agentName { get; set; }
        public string logUrl { get; set; }
    }

    public class Task3
    {
        public int id { get; set; }
        public string timelineRecordId { get; set; }
        public string name { get; set; }
        public DateTime dateStarted { get; set; }
        public DateTime dateEnded { get; set; }
        public DateTime startTime { get; set; }
        public DateTime finishTime { get; set; }
        public string status { get; set; }
        public int rank { get; set; }
        public object[] issues { get; set; }
        public string agentName { get; set; }
        public string logUrl { get; set; }
        public Task4 task { get; set; }
    }

    public class Task4
    {
        public string id { get; set; }
        public string name { get; set; }
        public string version { get; set; }
    }

    public class Condition
    {
        public bool result { get; set; }
        public string name { get; set; }
        public string conditionType { get; set; }
        public string value { get; set; }
    }

    public class Deployphasessnapshot
    {
        public Deploymentinput deploymentInput { get; set; }
        public int rank { get; set; }
        public string phaseType { get; set; }
        public string name { get; set; }
        public Workflowtask[] workflowTasks { get; set; }
    }

    public class Deploymentinput
    {
        public Parallelexecution parallelExecution { get; set; }
        public bool skipArtifactsDownload { get; set; }
        public Artifactsdownloadinput artifactsDownloadInput { get; set; }
        public int queueId { get; set; }
        public object[] demands { get; set; }
        public bool enableAccessToken { get; set; }
        public int timeoutInMinutes { get; set; }
        public int jobCancelTimeoutInMinutes { get; set; }
        public string condition { get; set; }
        public Overrideinputs1 overrideInputs { get; set; }
    }

    public class Parallelexecution
    {
        public string parallelExecutionType { get; set; }
    }

    public class Artifactsdownloadinput
    {
        public Downloadinput[] downloadInputs { get; set; }
    }

    public class Downloadinput
    {
        public object[] artifactItems { get; set; }
        public string alias { get; set; }
        public string artifactType { get; set; }
        public string artifactDownloadMode { get; set; }
    }

    public class Overrideinputs1
    {
    }

    public class Workflowtask
    {
        public string taskId { get; set; }
        public string version { get; set; }
        public string name { get; set; }
        public string refName { get; set; }
        public bool enabled { get; set; }
        public bool alwaysRun { get; set; }
        public bool continueOnError { get; set; }
        public int timeoutInMinutes { get; set; }
        public string definitionType { get; set; }
        public Overrideinputs2 overrideInputs { get; set; }
        public string condition { get; set; }
        public Inputs1 inputs { get; set; }
    }

    public class Overrideinputs2
    {
    }

    public class Inputs1
    {
        public string targetType { get; set; }
        public string filePath { get; set; }
        public string arguments { get; set; }
        public string script { get; set; }
        public string workingDirectory { get; set; }
        public string failOnStderr { get; set; }
    }

}
