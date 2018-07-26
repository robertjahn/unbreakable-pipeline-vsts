using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceSelfHealingFunction.VSTS.ApiObjects.Releases.GetRelease
{

    public class GetReleaseRootObject
    {
        public int id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public DateTime createdOn { get; set; }
        public DateTime modifiedOn { get; set; }
        public Modifiedby modifiedBy { get; set; }
        public Createdby createdBy { get; set; }
        public Environment[] environments { get; set; }
        public Variables variables { get; set; }
        public object[] variableGroups { get; set; }
        public Artifact[] artifacts { get; set; }
        public Releasedefinition releaseDefinition { get; set; }
        public string description { get; set; }
        public string reason { get; set; }
        public string releaseNameFormat { get; set; }
        public bool keepForever { get; set; }
        public int definitionSnapshotRevision { get; set; }
        public string logsContainerUrl { get; set; }
        public string url { get; set; }
        public _Links3 _links { get; set; }
        public object[] tags { get; set; }
        public object triggeringArtifactAlias { get; set; }
        public Projectreference projectReference { get; set; }
        public Properties properties { get; set; }
    }

    public class Modifiedby
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

    public class Createdby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links1 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links1
    {
        public Avatar1 avatar { get; set; }
    }

    public class Avatar1
    {
        public string href { get; set; }
    }

    public class Variables
    {
        public DynatraceApplicationname DynatraceApplicationName { get; set; }
        public DynatraceDeploymentid DynatraceDeploymentId { get; set; }
        public DynatracePipelineinfourl DynatracePipelineInfoUrl { get; set; }
        public DynatraceProxy DynatraceProxy { get; set; }
        public DynatraceServicetocompare DynatraceServiceToCompare { get; set; }
        public DynatraceToken DynatraceToken { get; set; }
        public DynatraceUnbreakablegatefunctionkey DynatraceUnbreakableGateFunctionKey { get; set; }
        public DynatraceUrl DynatraceUrl { get; set; }
        public Testval TestVal { get; set; }
    }

    public class DynatraceApplicationname
    {
        public string value { get; set; }
    }

    public class DynatraceDeploymentid
    {
        public string value { get; set; }
    }

    public class DynatracePipelineinfourl
    {
        public string value { get; set; }
    }

    public class DynatraceProxy
    {
        public string value { get; set; }
    }

    public class DynatraceServicetocompare
    {
        public string value { get; set; }
    }

    public class DynatraceToken
    {
        public string value { get; set; }
    }

    public class DynatraceUnbreakablegatefunctionkey
    {
        public string value { get; set; }
    }

    public class DynatraceUrl
    {
        public string value { get; set; }
    }

    public class Testval
    {
        public string value { get; set; }
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
        public Self self { get; set; }
        public Web web { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Web
    {
        public string href { get; set; }
    }

    public class _Links3
    {
        public Self1 self { get; set; }
        public Web1 web { get; set; }
    }

    public class Self1
    {
        public string href { get; set; }
    }

    public class Web1
    {
        public string href { get; set; }
    }

    public class Projectreference
    {
        public string id { get; set; }
        public object name { get; set; }
    }

    public class Properties
    {
        public Downloadbuildartifactsusingtask DownloadBuildArtifactsUsingTask { get; set; }
        public Releasecreationsource ReleaseCreationSource { get; set; }
    }

    public class Downloadbuildartifactsusingtask
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Releasecreationsource
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Environment
    {
        public int id { get; set; }
        public int releaseId { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public Variables1 variables { get; set; }
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
        public Releasedefinition1 releaseDefinition { get; set; }
        public Releasecreatedby releaseCreatedBy { get; set; }
        public string triggerReason { get; set; }
        public float timeToDeploy { get; set; }
        public Processparameters processParameters { get; set; }
        public Predeploymentgatessnapshot preDeploymentGatesSnapshot { get; set; }
        public Postdeploymentgatessnapshot postDeploymentGatesSnapshot { get; set; }
    }

    public class Variables1
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
        public Approver approver { get; set; }
    }

    public class Approver
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links4 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links4
    {
        public Avatar2 avatar { get; set; }
    }

    public class Avatar2
    {
        public string href { get; set; }
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
        public _Links5 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links5
    {
        public Avatar3 avatar { get; set; }
    }

    public class Avatar3
    {
        public string href { get; set; }
    }

    public class Release
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links6 _links { get; set; }
    }

    public class _Links6
    {
        public Web2 web { get; set; }
        public Self2 self { get; set; }
    }

    public class Web2
    {
        public string href { get; set; }
    }

    public class Self2
    {
        public string href { get; set; }
    }

    public class Releasedefinition1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public _Links7 _links { get; set; }
    }

    public class _Links7
    {
        public Web3 web { get; set; }
        public Self3 self { get; set; }
    }

    public class Web3
    {
        public string href { get; set; }
    }

    public class Self3
    {
        public string href { get; set; }
    }

    public class Releasecreatedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links8 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links8
    {
        public Avatar4 avatar { get; set; }
    }

    public class Avatar4
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
        public string function { get; set; }
        public string key { get; set; }
        public string queryParameters { get; set; }
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
        public Releasedefinition2 releaseDefinition { get; set; }
        public Releaseenvironment releaseEnvironment { get; set; }
        public string url { get; set; }
        public Approver1 approver { get; set; }
        public Approvedby approvedBy { get; set; }
    }

    public class Release1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links9 _links { get; set; }
    }

    public class _Links9
    {
    }

    public class Releasedefinition2
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public _Links10 _links { get; set; }
    }

    public class _Links10
    {
    }

    public class Releaseenvironment
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links11 _links { get; set; }
    }

    public class _Links11
    {
    }

    public class Approver1
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links12 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links12
    {
        public Avatar5 avatar { get; set; }
    }

    public class Avatar5
    {
        public string href { get; set; }
    }

    public class Approvedby
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links13 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links13
    {
        public Avatar6 avatar { get; set; }
    }

    public class Avatar6
    {
        public string href { get; set; }
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
        public Releasedefinition3 releaseDefinition { get; set; }
        public Releaseenvironment1 releaseEnvironment { get; set; }
        public string url { get; set; }
    }

    public class Release2
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links14 _links { get; set; }
    }

    public class _Links14
    {
    }

    public class Releasedefinition3
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public object projectReference { get; set; }
        public string url { get; set; }
        public _Links15 _links { get; set; }
    }

    public class _Links15
    {
    }

    public class Releaseenvironment1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public _Links16 _links { get; set; }
    }

    public class _Links16
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
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string descriptor { get; set; }
        public string url { get; set; }
        public _Links17 _links { get; set; }
        public string imageUrl { get; set; }
    }

    public class _Links17
    {
        public Avatar7 avatar { get; set; }
    }

    public class Avatar7
    {
        public string href { get; set; }
    }

    public class Requestedfor
    {
        public string displayName { get; set; }
        public string url { get; set; }
        public _Links18 _links { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string imageUrl { get; set; }
        public string descriptor { get; set; }
    }

    public class _Links18
    {
        public Avatar8 avatar { get; set; }
    }

    public class Avatar8
    {
        public string href { get; set; }
    }

    public class Lastmodifiedby
    {
        public string displayName { get; set; }
        public string id { get; set; }
        public string uniqueName { get; set; }
        public string descriptor { get; set; }
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
        public int rank { get; set; }
        public Issue[] issues { get; set; }
    }

    public class Issue
    {
        public string issueType { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
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

    public class Artifact
    {
        public string sourceId { get; set; }
        public string type { get; set; }
        public string alias { get; set; }
        public Definitionreference definitionReference { get; set; }
        public bool isPrimary { get; set; }
        public bool isRetained { get; set; }
    }

    public class Definitionreference
    {
        public Artifactsourcedefinitionurl artifactSourceDefinitionUrl { get; set; }
        public Definition definition { get; set; }
        public Istriggeringartifact IsTriggeringArtifact { get; set; }
        public Project project { get; set; }
        public Pullrequestmergecommitid pullRequestMergeCommitId { get; set; }
        public Version version { get; set; }
        public Artifactsourceversionurl artifactSourceVersionUrl { get; set; }
        public Defaultversiontype defaultVersionType { get; set; }
        public Branch branch { get; set; }
    }

    public class Artifactsourcedefinitionurl
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Definition
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Istriggeringartifact
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Pullrequestmergecommitid
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Version
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Artifactsourceversionurl
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Defaultversiontype
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Branch
    {
        public string id { get; set; }
        public string name { get; set; }
    }

}
