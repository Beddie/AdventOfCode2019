using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day99 : AdventBase
    {
        public Day99(int day) : base(day)
        {
            Title = "";
            TestInput = "";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day1;
            Day2Integers = PuzzleInput.Split(',').Cast<int>();
            SolutionPart1 = "";
            SolutionPart2 = "";
        }

        private IEnumerable<int> Day2Integers;

        public override async Task Part1()
        {
            ResultPart1 = string.Empty;
        }

        public override async Task Part2() => ResultPart2 = "";

    }
}

