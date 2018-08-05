using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GateTest
{
    public static class Class1
    {
        public async static Task<string> GetName()
        {
            System.Console.WriteLine("Something goes here");
            int a = 1 + 9;

            var httpClient = new HttpClient();
            var name = await httpClient.GetStringAsync("http://msdn.microsoft.com");
            
            return name;
        }
    }
}
