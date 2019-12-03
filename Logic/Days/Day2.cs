using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day2 : AdventBase
    {
        public Day2(int day) : base(day)
        {
            Title = "1202 Program Alarm";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day2;
            ComputerMemory = PuzzleInput.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            SolutionPart1 = "4484226";
            SolutionPart2 = "5696";
        }

        private List<int> ComputerMemory;
        private const int opCodeLength = 4;

        private struct IntCode
        {
            public int InstructionID;
            public int fromPosition1;
            public int fromPosition2;
            public int toPosition;
            public int valuefromPosition1;
            public int valuefromPosition2;
            public int toPositionResult;

            public bool Terminate => InstructionID == 99;

            private enum OpCode
            {
                Adds = 1,
                Multiplie = 2,
            }

            public void ExecuteOperation() {
                switch (InstructionID)
                {
                    case (int)OpCode.Adds:
                        toPositionResult = valuefromPosition1 + valuefromPosition2;
                        break;
                    case (int)OpCode.Multiplie:
                        toPositionResult = valuefromPosition1 * valuefromPosition2;
                        break;
                    default:
                        break;
                }
            }
        }

        public override async Task Part1()
        {
            ResultPart1 = TestInputs(12, 2).ToString();
        }

        public override async Task Part2()
        {
            var nouns = Enumerable.Range(0, 99);
            var verbs = Enumerable.Range(0, 99);

            Parallel.ForEach(nouns, (noun, loopstate1) =>
            {
                Parallel.ForEach(verbs, (verb, loopstate2) =>
                {
                    if (19690720 == TestInputs(noun, verb))
                    {
                        ResultPart2 = $"{100 * noun + verb}";
                        loopstate1.Stop();
                        loopstate2.Stop();
                    }
                });
            });
        }

        private int TestInputs(int noun, int verb)
        {
            List<int> computerMemory = new List<int>(ComputerMemory);
            computerMemory[1] = noun;
            computerMemory[2] = verb;

            var skip = 0;
            while (true)
            {
                var intCode = computerMemory.Skip(skip).Take(opCodeLength).ToArray();
                var currentOpcode = new IntCode() { InstructionID = intCode[0], fromPosition1 = intCode[1], fromPosition2 = intCode[2], toPosition = intCode[3] };
                if (currentOpcode.Terminate) break;
                currentOpcode.valuefromPosition1 = computerMemory[currentOpcode.fromPosition1];
                currentOpcode.valuefromPosition2 = computerMemory[currentOpcode.fromPosition2];
                currentOpcode.ExecuteOperation();
                computerMemory[currentOpcode.toPosition] = currentOpcode.toPositionResult;
                skip += opCodeLength;
            };
            return computerMemory[0];
        }
    }
}

