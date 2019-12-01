using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day1 : AdventBase
    {
        public Day1(int day) : base(day)
        {
            Title = "The Tyranny of the Rocket Equation";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day1;
            Day1Modules = PuzzleInput.Split('\n').Select(x => Convert.ToDouble(x));
            SolutionPart1 = "3325342";
            SolutionPart2 = "4985158";
        }

        private IEnumerable<double> Day1Modules;

        public override async Task Part1()
        {
            var totalFuel = 0d;
            foreach (var module in Day1Modules)
            {
                totalFuel += CalcFuelPart1(module);
            }
            ResultPart1 = totalFuel.ToString();
        }

        private double CalcFuelPart1(double input) => Math.Round(input / 3d, 0, MidpointRounding.ToZero) - 2;
        public override async Task Part2() => ResultPart2 = Day1Modules.Select(module => CalcFuelPart2(module)).Sum().ToString();

        private double CalcFuelPart2(double input)
        {
            var totalfuelfuel = 0d;
            var fuelfuel = CalcFuelPart1(input);
            do
            {
                totalfuelfuel += fuelfuel;
                fuelfuel = CalcFuelPart1(fuelfuel);
            } while (fuelfuel > 0);
            return totalfuelfuel;
        }
    }
}

