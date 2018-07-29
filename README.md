# Dynatrace Unbreakable Pipeline for VSTS

The Dynatrace Unbreakable Pipeline for VSTS consists of the following use case:

- monitor your app in all the environments using Dynatrace
- use monspec files to define what you want to monitor and your performance thresholds
- use Dynatrace Unbreakable Pipeline Release Gate to automate the approval between environments based on performance data captured while running continuous performance tests after a release.
- self healing after deploying into production via rollback to previous release if problems detected with Dynatrace

## Prerequisites

### Dynatrace Unbreakable Pipeline VSTS Extension
To implement the Unbreakable Pipeline, you will need the Dynatrace Unbreakable Pipeline VSTS extension. This extension consists of two things

- Dynatrace Unbreakable Pipeline Gate
- Dynatrace Push Deployment Event Task

You can find the code for the extension [here](http://).

### Dynatrace Unbreakable Pipeline Gate - Azure Function

This gate uses an [Azure function](https://azure.microsoft.com/en-us/services/functions/) for a serverless on-demand processing of performance data captured while running performance tests during a release. The code for this function is [here](https://github.com/dynatrace-innovationlab/unbreakable-pipeline-vsts/tree/master/UnbreakablePipelineGate/DynatraceUnbreakablePipelineFunction).

### Dynatrace Unbreakable Pipeline Proxy

This gate also uses a proxy which consists of a VM with python installed and the Dynatrace CLI. The proxy code uses the Dynatrace CLI to pull monspec info from Dynatrace.

The code for the proxy is [here](https://github.com/dynatrace-innovationlab/unbreakable-pipeline-vsts/tree/master/UnbreakablePipelineProxy).

Need to make this into a container. Would be super cool to have this deployed in [Azure Container Instance](https://azure.microsoft.com/en-us/services/container-instances/) and have the gate launch this.

