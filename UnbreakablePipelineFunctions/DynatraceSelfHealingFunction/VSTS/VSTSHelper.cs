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

        public VSTSHelper()
        {

        }

        public int ReleaseProblemDetected(string project, int releaseId, string environment)
        {
            Log.Info("ReleaseProblemDetected(): project: " + project + " releaseId: " + releaseId + " environment: " + environment);

            // from release Id, get release definition id
            Log.Info("Getting release");
            var release = GetRelease(project, releaseId);
            var releaseDefinitionId = release.releaseDefinition.id;
            Log.Info("Got release, release defintion id: " + releaseDefinitionId);

            // get all releases for the release definition
            Log.Info("Getting all releases using release defintion: " + releaseDefinitionId);
            var releases = ListReleases(project, releaseDefinitionId);

            // get previous successful release
            Log.Info("geting previous release");
            var previousSuccessfulRelease = GetPreviousSuccessfulRelease(project, releases.value, releaseId, environment);

            // redploy to environment
            var environmentId = previousSuccessfulRelease.environments.First(env => env.name == environment).id;
            var redployResponseObj = Redeploy(project, previousSuccessfulRelease.id, environmentId);

            Log.Info("Finished triggering release id: " + previousSuccessfulRelease.id, " to environment: " + environment);

            return previousSuccessfulRelease.id;

        }

        private ApiObjects.Releases.GetRelease.GetReleaseRootObject GetPreviousSuccessfulRelease(
            string project,
            ApiObjects.Releases.List.Release[] releases,
            int releaseId,
            string environment)
        {
            Log.Info("GetReleaseRootObject() starting");
            GetReleaseRootObject returnObj = null;
            var releasesReverseOrdered = releases.OrderByDescending(x => x.id);
            Log.Info("iterating all releases in revers order");
            foreach (var release in releasesReverseOrdered)
            {
                Log.Info("release: " + release.id);

                // only look at previous releases
                if (release.id < releaseId)
                {
                    Log.Info("This is a previous release, processing.");
                    // get the full release object so we can see if it successfuly deployed in the environment
                    var theRelease = GetRelease(project, release.id);
                    // check if it deployed successfully in the environment
                    if (theRelease.environments.Any(env => (env.name == environment) && (env.status.ToLower() == "succeeded")))
                    {
                        Log.Info("In this release, found environment: " + environment + " and status succeeded");
                        returnObj = theRelease;
                        break;
                    }
                    else
                    {
                        Log.Info("Release did not have successful environment: " + JsonConvert.SerializeObject(theRelease));
                    }
                }
                else
                {
                    Log.Info("This is not a previous release, ignoring.");
                }
            }
            return returnObj;
        }


        private RedeployRootObject Redeploy(string project, int releaseId, int environmentId)
        {
            var apiUrl = GetVstsReleaseApiUrl() + "/" + project + "/_apis/Release/releases/" + releaseId + "/environments/" + environmentId;
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
            var refererUrl = VSTSUrl + "/" + project + "/_releaseProgress?releaseId=" + releaseId + "&_a=release-pipeline-progress";
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



        private GetReleaseRootObject GetRelease(string project, int releaseId)
        {
            var apiUrl = GetVstsReleaseApiUrl() + "/" + project + "/_apis/release/releases/" + releaseId + "?api-version=4.1-preview.6";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
            request.UserAgent = "DynatraceSelfHealingFunction";
            request.Headers.Set(HttpRequestHeader.Authorization, GetHeaderAuthorization());
            var response = (HttpWebResponse)request.GetResponse();

            var reader = new StreamReader(response.GetResponseStream());
            var body = reader.ReadToEnd();

            var returnObj = JsonConvert.DeserializeObject<GetReleaseRootObject>(body);
            return returnObj;
        }

        private ListReleasesRootobject ListReleases(string project, int releaseDefinitionId)
        {
            //var theUrl = "https://msvstsdemo-a.vsrm.visualstudio.com/AbelUnbreakablePipelineDemo/_apis/release/releases?api-version=4.1-preview.6&definitionId=1";
            var apiUrl = GetVstsReleaseApiUrl() + "/" + project + "/_apis/release/releases?api-version=4.1-preview.6&definitionId=" + releaseDefinitionId;
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



        private string GetHeaderAuthorization()
        {
            var plainText = "abel:" + this.VSTSPAT;
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return "Basic " + System.Convert.ToBase64String(plainTextBytes);

        }

        private string GetVstsReleaseApiUrl()
        {
            var splitStringArray = this.VSTSUrl.Split(new char[] { '.' });
            StringBuilder returnStringBuilder = new StringBuilder();


            returnStringBuilder.Append(splitStringArray[0]);
            returnStringBuilder.Append(".vsrm");

            for (int i = 1; i < splitStringArray.Length; i++)
            {
                returnStringBuilder.Append(".");
                returnStringBuilder.Append(splitStringArray[i]);
            }

            return returnStringBuilder.ToString();

        }







    }
}
