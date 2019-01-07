var path = require('path');
var tl = require('azure-pipelines-task-lib/task');
var request = require('request');

// show the task version information
var echo = new tl.ToolRunner(tl.which('echo', true));

// getting all parameter variables
var dtToken = tl.getInput('dtToken', true);
var dtUrl = tl.getInput('dtTenantUrl', true);
var entityType = tl.getInput('entityType', true);
var tagContext = tl.getInput('tagContext', true);
var tagName = tl.getInput('tagName', true);
var tagValue = tl.getInput('tagValue', true);
var param_deploymentName = tl.getInput('deploymentName', true);
var param_deploymentVersion = tl.getInput('deploymentVersion', true);
var param_deploymentProject = tl.getInput('deploymentProject', true);
var ciLink = tl.getInput('ciLink', true);
var jenkinsUrl = tl.getInput('jenkinsUrl', true);
var buildUrl = tl.getInput('buildUrl', true);
var gitCommit = tl.getInput('gitCommit', true);

console.info("Retrieved all parameters.");

// creating message payload
var messagePayload = {
    eventType: "CUSTOM_DEPLOYMENT",
    attachRules: {
        tagRule: [
            {
                meTypes: [
                    entityType
                ],
                tags: [
                    {
                        context: tagContext,
                        key: tagName,
                        value: tagValue
                    }
                ]
            }
            
        ]
    },
    deploymentName: param_deploymentName,
    deploymentVersion: param_deploymentVersion,
    deploymentProject: param_deploymentProject,
    source: "VSTS",
    ciBackLink: ciLink,
    customProperties: {
        JenkinsUrl: jenkinsUrl,
        BuildUrl: buildUrl,
        GitCommit: gitCommit
    }
};
var messagePayloadString = JSON.stringify(messagePayload);

console.info("Created message payload.");

// creating dynatrace post deployment event
var deploymentEventUrl = dtUrl + "/api/v1/events"
var options = {
    url: deploymentEventUrl,
    headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Api-Token ' + dtToken
    },
    body: messagePayloadString
}

function callback(error, response, body) {
    console.info("returned from request.");
    console.info("response code: " + response.statusCode);
    if (error) {
        console.info("Got Error statusCode: " + error.statusCode);
        console.info("Got Error message: " + error.message);
    }
    else {
        console.info("Returned body: " + response.body);
    }
};

console.info("Sending deployment request event to Dynatrace...");
console.info("deploymentEventUrl: " + deploymentEventUrl);
console.info("dtToken: " + dtToken);
console.info("messagePayloadString: " + messagePayloadString);
request.post(options, callback);
console.info("Done sending deployment request event to dyntrace.");

echo.exec({ failOnStdErr: false })
.then(function (code) {
    console.info("TaskResult: Succeeded");
    tl.setResult(tl.TaskResult.Succeeded, code);
})
.fail(function(err) {
    console.error("TaskResult: Error:" + err.message);
    tl.setResult(tl.TaskResult.Failed, err.message);
})
