using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DynatraceSelfHealingFunction
{
    public class DynatraceAPIHelper
    {
        public static async Task<HttpResponseMessage> Post(string apiUrl, string apiToken, string jsonBody)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Api-Token " + apiToken);
            if (jsonBody == null)
            {
                return await client.GetAsync(apiUrl);
            }

            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            return await client.PostAsync(apiUrl, content);
        }

        public static string ConvertToJSSMilliseconds(DateTime dateObj)
        {
            // javascript Date is represented as millisenconds since 1/1/1970 utc
            // this method calculates the number of milliseconds (dropping the decimal portion)
            // since the unixEpochDate for a c# DateTime object
            var unixEpochDateTime = new DateTime(1970, 1, 1);
            var difference = dateObj.ToUniversalTime() - unixEpochDateTime;
            var differenceDouble = difference.TotalMilliseconds;
            var differenceString = differenceDouble.ToString();
            var splitArray = differenceString.Split(new char[] { '.' });
            var returnString = splitArray[0];

            return returnString;
        }
    }
}
