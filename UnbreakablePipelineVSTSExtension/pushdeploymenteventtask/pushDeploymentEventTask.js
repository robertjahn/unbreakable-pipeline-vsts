var path = require('path');
var tl = require('vso-task-lib');
var request = require('request');

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
echo.arg("retrieved all parameters..." + "\n");

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

echo.arg("created message payload..." + "\n");

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

echo.arg("sending deployment request event to dynatrace..." + "\n");
request.post(options, function(error, response, body) {
    echo.arg("returned from post..." + "\n");
    echo.arg("response code: " + response.statusCode + "\n");
    echo.arg("returned: " + response + "\n");
    echo.arg("Error: " + error + "\n");
});
echo.arg("done sending deployment request event to dyntrace..." + "\n")

echo.exec({ failOnStdErr: false})
.then(function(code) {
    tl.exit(code);
})
.fail(function(err) {
    console.error(err.message);
    tl.debug('taskRunner fail');
    tl.exit(1);
})
