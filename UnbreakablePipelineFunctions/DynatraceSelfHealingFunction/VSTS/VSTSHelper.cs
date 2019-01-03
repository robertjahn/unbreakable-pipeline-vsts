using System;
using Newtonsoft.Json;
using DynatraceSelfHealingFunction.VSTS.ApiObjects.Releases.GetRelease;
using DynatraceSelfHealingFunction.VSTS.ApiObjects.Releases.List;
using DynatraceSelfHealingFunction.VSTS.ApiObjects.Releases.Redeploy;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Azure.WebJobs.Host;

namespace DynatraceSelfHealingFunction.VSTS
{
    public class VSTSHelper
    {
        public string VSTSUrl { get; set; }
        public string VSTSPAT { get; set; }
        public TraceWriter Log { get; set; }
        public string VSTSReleaseApiUrl { get; set; }

        public VSTSHelper()
        {

        }

        public int ReleaseProblemDetected(string project, int releaseId, string environment)
        {
            Log.Info("ReleaseProblemDetected: project: " + project + " releaseId: " + releaseId + " environment: " + environment);

            // from release Id, get release definition id
            Log.Info("ReleaseProblemDetected: calling GetRelease() with releaseId: " + releaseId);
            var release = GetRelease(project, releaseId);
            var releaseDefinitionId = release.releaseDefinition.id;
            Log.Info("ReleaseProblemDetected: Got release defintion id: " + releaseDefinitionId);

            // get all releases for the release definition
            Log.Info("ReleaseProblemDetected: Getting all releases using release defintion: " + releaseDefinitionId);
            var releases = ListReleases(project, releaseDefinitionId);

            // get previous successful release
            Log.Info("ReleaseProblemDetected: calling GetPreviousSuccessfulRelease()");
            var previousSuccessfulRelease = GetPreviousSuccessfulRelease(project, releases.value, releaseId, environment);

            // redploy to environment
            var environmentId = previousSuccessfulRelease.environments.First(env => env.name == environment).id;
            Log.Info("ReleaseProblemDetected: calling Redeploy() with previousSuccessfulRelease.id: " + previousSuccessfulRelease.id);
            var redployResponseObj = Redeploy(project, previousSuccessfulRelease.id, environmentId);
            Log.Info("ReleaseProblemDetected: Redeploy() returned: " + redployResponseObj);

            return previousSuccessfulRelease.id;

        }

        private ApiObjects.Releases.GetRelease.GetReleaseRootObject GetPreviousSuccessfulRelease(
            string project,
            ApiObjects.Releases.List.Release[] releases,
            int releaseId,
            string environment)
        {
            Log.Info("GetPreviousSuccessfulRelease: starting");
            GetReleaseRootObject returnObj = null;
            var releasesReverseOrdered = releases.OrderByDescending(x => x.id);
            Log.Info("GetPreviousSuccessfulRelease: iterating all releases in reverse order");
            foreach (var release in releasesReverseOrdered)
            {
                Log.Info("GetPreviousSuccessfulRelease: processing release: " + release.id);
                // only look at previous releases
                if (release.id < releaseId)
                {
                    // get the full release object so we can see if it successfuly deployed in the environment
                    Log.Info("GetPreviousSuccessfulRelease: This is a previous release, processing release.id: " + release.id);
                    var theRelease = GetRelease(project, release.id);
                    Log.Info("GetPreviousSuccessfulRelease: This is a previous release, found theRelease: " + theRelease);

                    // check if it deployed successfully in the environment
                    if (theRelease.environments.Any(env => (env.name == environment) && (env.status.ToLower() == "succeeded")))
                    {
                        Log.Info("GetPreviousSuccessfulRelease: In this release, found environment: " + environment + " and status succeeded");
                        returnObj = theRelease;
                        break;
                    }
                    else
                    {
                        throw new Exception("GetPreviousSuccessfulRelease: Aborting. Release did not have successful environment: " + JsonConvert.SerializeObject(theRelease));
                    }
                }
                else
                {
                    Log.Info("GetPreviousSuccessfulRelease: Skipping. release.id: " + release.id + " not less than releaseId:" + releaseId);
                }
            }
            return returnObj;
        }


        private RedeployRootObject Redeploy(string project, int releaseId, int environmentId)
        {
            var apiUrl = this.VSTSReleaseApiUrl + "/" + project + "/_apis/Release/releases/" + releaseId + "/environments/" + environmentId;
            Log.Info("RedeployRootObject: apiUrl: " + apiUrl);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.KeepAlive = true;
                request.Headers.Add("X-TFS-FedAuthRedirect", @"Suppress");
                request.Headers.Add("Origin", @"https://msvstsdemo-a.visualstudio.com");
                request.Headers.Add("X-VSS-ReauthenticationAction", @"Suppress");
                request.Headers.Set(HttpRequestHeader.Authorization, GetHeaderAuthorization());
                request.ContentType = "application/json";
                request.Accept = "application/json;api-version=5.0-preview.6;excludeUrls=true";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36";
                request.Headers.Add("X-TFS-Session", @"0a0daa44-1443-4b15-886a-a45e1ffc1899");
                request.Headers.Add("DNT", @"1");
                var refererUrl = this.VSTSUrl + "/" + project + "/_releaseProgress?releaseId=" + releaseId + "&_a=release-pipeline-progress";
                request.Referer = refererUrl;
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");

                request.Method = "PATCH";

                string body = @"{""status"":2,""scheduledDeploymentTime"":null,""comment"":""Self healing dynatrace redeploy""}";
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                var response = (HttpWebResponse)request.GetResponse();

                var responseStream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                var reader = new StreamReader(responseStream);
                var responseBody = reader.ReadToEnd();

                var returnObj = JsonConvert.DeserializeObject<RedeployRootObject>(responseBody);
                return returnObj;
            }
            catch (Exception e)
            {
                throw new Exception("Error calling " + apiUrl + " exception: " + e.Message);
            }

        }

        private GetReleaseRootObject GetRelease(string project, int releaseId)
        {
            var apiUrl = this.VSTSReleaseApiUrl + "/" + project + "/_apis/release/releases/" + releaseId + "?api-version=4.1-preview.6";
            Log.Info("GetRelease: calling apiUrl: " + apiUrl);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.UserAgent = "DynatraceSelfHealingFunction";
                request.Headers.Set(HttpRequestHeader.Authorization, GetHeaderAuthorization());
                var response = (HttpWebResponse)request.GetResponse();

                var reader = new StreamReader(response.GetResponseStream());
                var body = reader.ReadToEnd();

                Log.Info("GetRelease: before serialization");
                Log.Info("GetRelease: body: " + body);

                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;

                var returnObj = JsonConvert.DeserializeObject<GetReleaseRootObject>(body, settings);
                Log.Info("GetRelease: after serialization");
                return returnObj;
            }
            catch (Exception e)
            {
                throw new Exception("Error calling " + apiUrl + " exception: " + e.Message);
            }
        }

        private ListReleasesRootobject ListReleases(string project, int releaseDefinitionId)
        {
            //var theUrl = "https://msvstsdemo-a.vsrm.visualstudio.com/AbelUnbreakablePipelineDemo/_apis/release/releases?api-version=4.1-preview.6&definitionId=1";
            var apiUrl = this.VSTSReleaseApiUrl + "/" + project + "/_apis/release/releases?api-version=4.1-preview.6&definitionId=" + releaseDefinitionId;
            Log.Info("ListReleases: calling apiUrl: " + apiUrl);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.UserAgent = "DynatraceSelfHealingFunction";
                request.Headers.Set(HttpRequestHeader.Authorization, GetHeaderAuthorization());
                var response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var body = reader.ReadToEnd();
                    var returnObj = JsonConvert.DeserializeObject<ListReleasesRootobject>(body);
                    return returnObj;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error calling " + apiUrl + " exception: " + e.Message);
            }

        }

        private string GetHeaderAuthorization()
        {
            var plainText = "abel:" + this.VSTSPAT;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return "Basic " + System.Convert.ToBase64String(plainTextBytes);

        }
    }
}
