NOTICE:
 
The components in this demo have some known issues and limitations listed below.  As this is a community project please be aware that there is not Dynatrace product support for this and please read the disclaimers below about the state and purpose of this code.
* Extensions
  * Uses an out of date DevOps node module and can result in “null” response when run on custom build agents
  * Limited to a single Dynatrace Tag and specific version the Dynatrace API
  * Limited debug messages
* Function Apps
  * Built on function app 1.x framework.  2.x is the recommended version now
  * Requires visual studio enterprise to run the mock tests as to have the “Fakes” module
  * Quality Gate “callback” feature is broken
  * Limited debug messages
  * Selfhealing function –This function has assumptions for what is put into the deployment event, Dynatrace and DevOps API versions, and variables passed from the DevOps Pipeline
* Web Proxy app
  * No README for building and deploying the application to a VM


# Dynatrace Unbreakable Pipeline for VSTS

Original Developer: Abel Wang (Microsoft)
Maintainer: Maintainer: None.  
Point of contact for suggestions/questions/ways to improve: Dynatrace Innovation Lab (Main contact Rob Jahn - rob.jahn@dynatrace.com)

Special thanks goes to Abel Wang and Donovan Brown who started implementing this plugin. More details on Abels blog about [The Dynatrace Unbreakable Pipeline in VSTS and Azure](https://abelsquidhead.com/index.php/2018/08/03/the-dynatrace-unbreakable-pipeline-in-vsts-and-azure-bam/)

*Disclaimer*
The Dynatrace Unbreakable Pipeline for VSTS plugin is currently in Early Access.
Early Access releases provide early-stage insight into new features and functionality of the Dynatrace Platform. They enable you to provide feedback that can significantly impact our direction and implementation.

While Early Access releases aren't ready to be used to build production solutions, they're at a stage where you can test and tinker with an implementation. As we receive feedback and iterate on a project, we anticipate breaking changes without advanced warning, so Early Access releases should not be used in a user-facing manner or applied to production environments.

# Dynatrace Unbreakable Pipeline for VSTS

The Dynatrace Unbreakable Pipeline for VSTS consists of the following use case:

- monitor your app in all the environments using Dynatrace
- use monspec files to define what you want to monitor and your performance thresholds
- use Dynatrace Unbreakable Pipeline Release Gate to automate the approval between environments based on performance data captured while running continuous performance tests after a release.
- self healing after deploying into production via rollback to previous release if problems detected with Dynatrace

## Prerequisites

### Dynatrace Unbreakable Pipeline VSTS Extension
To implement the Unbreakable Pipeline, you will need the Dynatrace Unbreakable Pipeline VSTS extension. This extension consists of two things

- Dynatrace Unbreakable Pipeline Release Gate
- Dynatrace Push Deployment Event Task

You can find the code for the extension [here](https://github.com/dynatrace-innovationlab/unbreakable-pipeline-vsts/tree/master/UnbreakablePipelineVSTSExtension).

### Dynatrace Unbreakable Pipeline Gate - Azure Function

The release gate uses an [Azure function](https://azure.microsoft.com/en-us/services/functions/) for a serverless on-demand processing of performance data captured while running performance tests during a release. The code for this function is [here](https://github.com/dynatrace-innovationlab/unbreakable-pipeline-vsts/tree/master/UnbreakablePipelineGate/DynatraceUnbreakablePipelineFunction).

### Dynatrace Unbreakable Pipeline Proxy

The release gate also uses a proxy which consists of a VM with python installed and the Dynatrace CLI. The proxy code uses the Dynatrace CLI to pull monspec info from Dynatrace.

The code for the proxy is [here](https://github.com/dynatrace-innovationlab/unbreakable-pipeline-vsts/tree/master/UnbreakablePipelineProxy).

Need to make this into a container. Would be super cool to have this deployed in [Azure Container Instance](https://azure.microsoft.com/en-us/services/container-instances/) and have the gate launch this.

### Dynatrace Self Healing Function

Self healing web hook uses an Azure function for a serverless on-demand processing and rollback of releases. The code for the self healing azure function is [here](https://github.com/dynatrace-innovationlab/unbreakable-pipeline-vsts/tree/master/UnbreakablePipelineGate
).

Input into the self healing webhook/function is in the form:

```json
{
    "State":"{State}",
    "ProblemID":"{ProblemID}",
    "ProblemTitle":"{ProblemTitle}",
    "PID": "{PID}",
    "ImpactedEntities": {ImpactedEntities}
}
```


## Setup
Currently setup is all done manually. Would be cool to use Donovan's Yo Team to scaffold out the entire pipeline for a project.
