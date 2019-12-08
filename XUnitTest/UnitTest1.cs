using Logic.Model;
using Logic.Service;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest
{
    public class UnitTest1
    {
        private const int DAY = 8;
        private const int PERFORMANCECOUNT = 30;

        [Fact]
        public async Task DayPart1()
        {
            var day = RenderDayService.GetDay(DAY, EnumParts.Part1);
            await day.RenderParts();
            day.PrintResultToDebugWindow();
        }

        [Fact]
        public async Task DayPart2()
        {
            var day = RenderDayService.GetDay(DAY, EnumParts.Part2);
            await day.RenderParts();
            day.PrintResultToDebugWindow();
        }

        [Fact]
        public async Task AllDayResult()
        {
            var day = RenderDayService.GetDay(DAY);
            await day.RenderParts();
            day.PrintResultToDebugWindow();
        }

        [Fact]
        public async Task TestDaySolutionPerformance()
        {
            var averageRunTime = new HashSet<long>();
            bool isValid = true;
            for (int i = 0; i < PERFORMANCECOUNT; i++)
            {
                var day = RenderDayService.GetDay(DAY);
                await day.RenderParts();
                averageRunTime.Add(day.ElapsedTime);
                if (!day.IsValid)
                {
                    isValid = false;
                    Debug.WriteLine($"Invalid");
                    break;
                }
                day.PrintResultToDebugWindow();
            }

            Debug.WriteLine($"IsCorrect?={isValid}. Average complete in {averageRunTime.Average()} ms");
        }
    }
}
