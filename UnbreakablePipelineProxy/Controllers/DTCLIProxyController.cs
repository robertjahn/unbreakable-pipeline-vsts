using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace UnbreakableProxy.Controllers
{
    
    public class DTCLIProxyController : Controller
    {
        private static readonly string CLIDIRECTORY = "/home/abel/dtcli";

        [HttpPost]
        [Route("api/DTCLIProxy/MonspecPullRequest")]
        [Produces("application/json")]
        public JsonResult MonspecPullRequest(
            string monspecFile, 
            string pipelineInfoFile, 
            string serviceToCompare, 
            int compareWindow, 
            string dynatraceTennantUrl, 
            string token ) 
        {
            // clean up and save monspecFile and pipelineInfoFile
            var builder = new StringBuilder();
            builder.Append(CLIDIRECTORY);
            builder.Append("/monspec.json");
            string monspecFilePath = builder.ToString();
            if(System.IO.File.Exists(monspecFilePath)) {
                System.IO.File.Delete(monspecFilePath);
            }
            System.IO.File.WriteAllText(monspecFilePath, monspecFile);
            
            // clean up and save pipelineInfoFile
            builder = new StringBuilder();
            builder.Append(CLIDIRECTORY);
            builder.Append("/pipelineinfo.json");
            string pipelineInfoFilePath = builder.ToString();
            if (System.IO.File.Exists(pipelineInfoFilePath)) {
                System.IO.File.Delete(pipelineInfoFilePath);
            }
            System.IO.File.WriteAllText(pipelineInfoFilePath, pipelineInfoFile);

            // create command line arguemnts to dtcli.py
            builder = new StringBuilder();
            builder.Append("dtcli.py monspec pullcompare monspec.json pipelineinfo.json ");
            builder.Append(serviceToCompare);
            builder.Append(" ");
            builder.Append(compareWindow);
            builder.Append(" ");
            builder.Append(dynatraceTennantUrl);
            builder.Append(" ");
            builder.Append(token);
            builder.Append(" ");
            builder.Append(0);
            var parameters = builder.ToString();

            // call dtcli.py
            var startInfo = new ProcessStartInfo();
            startInfo.WorkingDirectory = "/home/abel/dtcli";
            startInfo.FileName = "python3";
            startInfo.Arguments = parameters;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            var p = Process.Start(startInfo);
            
            // read values from standard output
            var output = new StringBuilder();
            while (!p.StandardOutput.EndOfStream) {
                output.AppendLine(p.StandardOutput.ReadLine());
            }
            p.WaitForExit();

            // parse result string into a PullCompareResponse object
            var pullCompareResponse = JsonConvert.DeserializeObject<PullCompareResponse>(output.ToString());
            
            // return as a JsonResult
            return new JsonResult(pullCompareResponse);
        }
    }
}
