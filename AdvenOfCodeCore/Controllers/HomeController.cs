using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http;
using Logic.Model;
using Logic.Service;

namespace AdvenOfCodeCore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Index", RenderDayService.GetOverview());
        }

        public ActionResult RenderDayPart()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public async Task<IActionResult> Details(int id, int part)
        {
            var overviewmodel = RenderDayService.GetOverview();

            var AoCDay = overviewmodel.Where(c => c.ID == id).FirstOrDefault();
            if (AoCDay != null)
            {
                var partResult = string.Empty;
                AoCDay.PartsToRender = new List<EnumParts> { (EnumParts)part };
                await AoCDay.RenderParts();
                partResult = AoCDay.GetResult((EnumParts)part);
                if (!string.IsNullOrWhiteSpace(partResult)) ViewBag.puzzleString = $"{partResult}";
            }

            return View("Index", overviewmodel);
        }

        public async Task<IActionResult> AllDetails()
        {
            var sw = new Stopwatch();
            var overviewmodel = RenderDayService.GetOverview();
            var sb = new StringBuilder();

            sw.Start();
            var startTime = sw.ElapsedMilliseconds;
            foreach (var day in overviewmodel)
            {
                await day.Part1();
                var day1Time = sw.ElapsedMilliseconds - startTime;
                await day.Part2();
                var day2Time = sw.ElapsedMilliseconds - startTime - day1Time;
                sb.AppendLine($"{day.Name} part1 answer: {day.ResultPart1} ({day1Time} ms), part2 answer: {day.ResultPart2} ({day2Time} ms))");
                startTime = sw.ElapsedMilliseconds;
            }
            sw.Stop();
            sb.AppendLine($"Total executionTime {sw.ElapsedMilliseconds} ms");
            ViewBag.puzzleString = sb.ToString();

            return View("Index", overviewmodel);
        }

        //TODO make private leaderboard JSON with cookie WORK :)
        public async Task<ActionResult> GetLeaderBoard()
        {
            var overviewmodel = RenderDayService.GetOverview();
            var leaderboard = await GetJSON();
            ViewBag.leaderboard = leaderboard;
            return View("Index", overviewmodel);
        }

        private async Task<string> GetJSON()
        {
            using (var httpClient = new HttpClient())
            {
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://adventofcode.com/2018/leaderboard/private/view/205926.json");
                httpRequestMessage.Headers.Add("Cookie", "{\"key\":\"2018 - 11 - 30499\",\"target\":1544504398}");
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
                var httpContent = httpResponseMessage.EnsureSuccessStatusCode();
                string result = await httpContent.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}
