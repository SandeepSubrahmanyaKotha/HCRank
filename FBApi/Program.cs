using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;

namespace FBApi
{
    public class Mresponse
    {
        public int page { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public int total_pages { get; set; }
        public List<MInfo> data { get; set; }
    }
    class Program
    {
        public static async Task Main(string[] args)
        {
           //await Part1();
           await Part2();
        }
        public static async Task  inp()
        {
          // await Program.Part2("UEFA Champions League", "2011");
        }

        public static async Task Part1()
        {
            Mresponse mr = null;
            HttpClient httpClient = new HttpClient();
            
            HttpResponseMessage resp = await httpClient.GetAsync(@"https://jsonmock.hackerrank.com/api/football_matches?year=2015");
            
            string hresp = await resp.Content.ReadAsStringAsync();
            mr = Newtonsoft.Json.JsonConvert.DeserializeObject<Mresponse>(hresp);
            int matchesdrawn = 0, totalpages = mr.total_pages;
            for (int i = 1; i <= 10; i++)
            {
                resp = await httpClient.GetAsync($"https://jsonmock.hackerrank.com/api/football_matches?year=2011&page={i}");
                //resp = await httpClient.GetAsync($"https://jsonmock.hackerrank.com/api/football_matches?year=2011&team1goals={i}&team2goals={i}");
                mr = JsonConvert.DeserializeObject<Mresponse>(await resp.Content.ReadAsStringAsync());
                // matchesdrawn = matchesdrawn + mr.data.Count;
                foreach (MInfo minfo in mr.data)
                    matchesdrawn = minfo.team1goals == minfo.team2goals ? matchesdrawn + 1 : matchesdrawn;
            }
            Console.WriteLine(matchesdrawn); Console.ReadLine();
            //Console.WriteLine(mr.page + "\n" + mr.total_pages + "\n" + mr.data.Count); Console.ReadLine();
            //File.WriteAllText(@"c:\temp\hresp.txt", hresp);
            //mr = await resp.Content.ReadAsAsync<Mresponse>();
            //httpClient.BaseAddress = @"https://jsonmock.hackerrank.com/api/football_matches?year=2011";
        }

        public static async Task Part2()
        {            
            HttpClient httpClient = new HttpClient();
            
            HttpResponseMessage resp = await httpClient.GetAsync($"https://jsonmock.hackerrank.com/api/football_competitions?year=2011&name=UEFA Champions League");

            string hresp = await resp.Content.ReadAsStringAsync(), v = "";
            int total = 0;
            
            foreach (Match m in Regex.Matches(hresp, @".winner.:.(?<winner>\w+)."))
            {
                v = m.Groups["winner"].Value;                
            }
            
            for(int i=1; i<=10;i++)
            {
                HttpResponseMessage resp1 = await httpClient.GetAsync($"https://jsonmock.hackerrank.com/api/football_matches?competition=UEFA Champions League&year=2011&team1={v}&team1goals={i}");
                string s1 = await resp1.Content.ReadAsStringAsync();

                Regex rgx = new Regex(@".team1goals.:.(?<word>\d).");

                foreach(Match m in rgx.Matches(s1))
                {
                    Console.WriteLine(m.Groups["word"].Value);
                    total = total + Convert.ToInt32(m.Groups["word"].Value);
                    Console.WriteLine(total);
                }

                HttpResponseMessage resp2 = await httpClient.GetAsync($"https://jsonmock.hackerrank.com/api/football_matches?competition=UEFA Champions League&year=2011&team2={v}&team2goals={i}");
                string s2 = await resp2.Content.ReadAsStringAsync();
                rgx = new Regex(@".team2goals.:.(?<word>\d).");

                foreach (Match m in rgx.Matches(s2))
                {
                    Console.WriteLine(m.Groups["word"].Value);
                    total = total + Convert.ToInt32(m.Groups["word"].Value);
                    Console.WriteLine(total);
                }

                //total = total + m1.total + m2.total;
            }

            Console.WriteLine(total); 
            Console.ReadLine();
        }
    }

  

    public class MInfo
    {
        public string competition { get; set; }
        public int year { get; set; }
        public string round { get; set; }
        public string team1 { get; set; }
        public string team2 { get; set; }
        public int team1goals { get; set; }
        public int team2goals { get; set; }
    }
}
