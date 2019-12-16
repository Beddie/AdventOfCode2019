using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day5 : AdventBase
    {
        public Day5(int day) : base(day)
        {
            Title = "Sunny with a Chance of Asteroids";
           
//             TestInput = @"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
//1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
//999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99";
//            TestInput = @"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
//1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
//999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day5;
            ComputerMemoryInput = PuzzleInput.Split(',').Select(x => Convert.ToInt64(x)).ToList();
            SolutionPart1 = "7566643";
            SolutionPart2 = "9265694";
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
            TestInputOutput.Add(new InputOutput() { InputValue = "2", PuzzleString = "3,9,8,9,10,9,4,9,99,-1,8", OutputValue = "0" });
            TestInputOutput.Add(new InputOutput() { InputValue = "5", PuzzleString = "3,9,8,9,10,9,4,9,99,-1,8", OutputValue = "0" });
            TestInputOutput.Add(new InputOutput() { InputValue = "8", PuzzleString = "3,9,8,9,10,9,4,9,99,-1,8", OutputValue = "1" });
            TestInputOutput.Add(new InputOutput() { InputValue = "3", PuzzleString = "3,9,7,9,10,9,4,9,99,-1,8", OutputValue = "1" });
            TestInputOutput.Add(new InputOutput() { InputValue = "18", PuzzleString = "3,9,7,9,10,9,4,9,99,-1,8", OutputValue = "0" });
            TestInputOutput.Add(new InputOutput() { InputValue = "10", PuzzleString = "3,3,1108,-1,8,3,4,3,99", OutputValue = "0" });
            TestInputOutput.Add(new InputOutput() { InputValue = "8", PuzzleString = "3,3,1108,-1,8,3,4,3,99", OutputValue = "1" });
            TestInputOutput.Add(new InputOutput() { InputValue = "2", PuzzleString = "3,3,1107,-1,8,3,4,3,99", OutputValue = "1" });
            TestInputOutput.Add(new InputOutput() { InputValue = "8", PuzzleString = "3,3,1107,-1,8,3,4,3,99", OutputValue = "0" });
            TestInputOutput.Add(new InputOutput() { InputValue = "9", PuzzleString = "3,3,1107,-1,8,3,4,3,99", OutputValue = "0" });


            TestInputOutput.Add(new InputOutput() { InputValue = "0", PuzzleString = "3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9", OutputValue = "0" });
            TestInputOutput.Add(new InputOutput() { InputValue = "9", PuzzleString = "3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9", OutputValue = "1" });
            TestInputOutput.Add(new InputOutput() { InputValue = "33", PuzzleString = "3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9", OutputValue = "1" });
            TestInputOutput.Add(new InputOutput() { InputValue = "0", PuzzleString = "3,3,1105,-1,9,1101,0,0,12,4,12,99,1", OutputValue = "0" });
            TestInputOutput.Add(new InputOutput() { InputValue = "9", PuzzleString = "3,3,1105,-1,9,1101,0,0,12,4,12,99,1", OutputValue = "1" });
            TestInputOutput.Add(new InputOutput() { InputValue = "33", PuzzleString = "3,3,1105,-1,9,1101,0,0,12,4,12,99,1", OutputValue = "1" });
            
            
            TestInputOutput.Add(new InputOutput() { InputValue = "2", PuzzleString = @"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", OutputValue = "999" });
            TestInputOutput.Add(new InputOutput() { InputValue = "8", PuzzleString = @"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", OutputValue = "1000" });
            TestInputOutput.Add(new InputOutput() { InputValue = "9", PuzzleString = @"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", OutputValue = "1001" });
            TestInputOutput.Add(new InputOutput() { InputValue = "33", PuzzleString = @"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", OutputValue = "1001" });
            TestInputOutput.Add(new InputOutput() { InputValue = "0", PuzzleString = @"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,
1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,
999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", OutputValue = "999" });

            foreach (var tito in TestInputOutput)
            {
                var puzzleinput = tito.PuzzleString.Split(',').Select(x => Convert.ToInt64(x)).ToList();
                var intComputer = new Service.IntCodeComputer(puzzleinput, Convert.ToInt32(tito.InputValue));
                intComputer.Start();
                if (tito.OutputValue == intComputer.LocalComputerMemory.ProgramValue.ToString())
                {
                    Debug.WriteLine($"INPUT:{tito.InputValue} - {tito.PuzzleString} - OUTPUT:{tito.OutputValue} = OK");
                }
                else {
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
            var intComputer = new Service.IntCodeComputer(ComputerMemoryInput, 5);
            intComputer.Start();
             ResultPart2 = intComputer.LocalComputerMemory.ProgramValue.ToString();
        }
            
    }
}

