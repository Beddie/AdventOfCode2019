using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day9 : AdventBase
    {
        public Day9(int day) : base(day)
        {
            Title = "Sensor Boost";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day9;
            ComputerMemoryInput = PuzzleInput.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            SolutionPart1 = "3497884671";
            SolutionPart2 = "46470";
        }

        private List<long> ComputerMemoryInput;

        public override async Task Part1()
        {
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 1);
            intComputer.Start();
            ResultPart1 = intComputer.LocalComputerMemory.ProgramValue.ToString();

        }
        public override async Task TestPart1()
        {
            TestInputOutput.Add(new InputOutput() { InputValue = "21", PuzzleString = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99", OutputValue = "21" });
            TestInputOutput.Add(new InputOutput() { InputValue = "33", PuzzleString = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99", OutputValue = "33" });
            TestInputOutput.Add(new InputOutput() { InputValue = "4", PuzzleString = "1102,34915192,34915192,7,4,7,99,0", OutputValue = "0" });
            TestInputOutput.Add(new InputOutput() { InputValue = "4", PuzzleString = "104,1125899906842624,99", OutputValue = "1125899906842624" });

            foreach (var tito in TestInputOutput)
            {
                var puzzleinput = tito.PuzzleString.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                var intComputer = new Service.IntCodeComputer(puzzleinput, Convert.ToInt32(tito.InputValue));
                intComputer.Start();
                if (tito.OutputValue == intComputer.LocalComputerMemory.ProgramValue.ToString())
                {
                    Debug.WriteLine($"INPUT:{tito.InputValue} - {tito.PuzzleString} - OUTPUT:{tito.OutputValue} = OK");
                }
                else
                {
                    Debug.WriteLine($"INPUT:{tito.InputValue} - {tito.PuzzleString} - OUTPUT:{tito.OutputValue} = NOK");
                    puzzleinput = tito.PuzzleString.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                    var intComputer2 = new Service.IntCodeComputer(puzzleinput, Convert.ToInt32(tito.InputValue));
                    intComputer2.Start();
                }
                intComputer = null;
                puzzleinput = null;
            }
        }

        public override async Task Part2()
        {
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 2);
            intComputer.Start();
            ResultPart2 = intComputer.LocalComputerMemory.ProgramValue.ToString();
        }
    }
}

