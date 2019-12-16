using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day11: AdventBase
    {
        public Day11(int day) : base(day)
        {
            Title = "Space Police";
            //TestInput = "109,19,204,-34";
           // TestInput = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day11;
            ComputerMemoryInput = PuzzleInput.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            SolutionPart1 = "";
            SolutionPart2 = "";
        }

        private List<long> ComputerMemoryInput;

        public override async Task Part1()
        {
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 1);
            intComputer.Start();
            ResultPart1 = intComputer.LocalComputerMemory.ProgramValue.ToString();
        }
        public override async Task Part2()
        {
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 1);
            intComputer.Start();
            ResultPart2 = intComputer.LocalComputerMemory.ProgramValue.ToString();

        }
    }
}

