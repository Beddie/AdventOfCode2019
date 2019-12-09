using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day7 : AdventBase
    {
        public Day7(int day) : base(day)
        {
            Title = "Amplification Circuit";
            //             TestInput = @"3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,
            //-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,
            //53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day7;
            ComputerMemoryInput = PuzzleInput.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            SolutionPart1 = "21860";
            SolutionPart2 = "2645740";
        }

        private List<int> ComputerMemoryInput;
        public enum OpCode
        {
            Adds = 1,
            Multiplie = 2,
            Inputstuff = 3,
            OutputStuff = 4,
            JumpIfTrue = 5,
            JumpIfFalse = 6,
            isLessThen = 7,
            isEqual = 8,
        }

        public class IntCode
        {
            public IntCode(int[] intCode)
            {
                if (intCode[0] != 99)
                {
                    parameter1InstructionValue = intCode[1];
                    if (intCode.Length > 2) parameter2InstructionValue = intCode[2];
                    if (intCode.Length > 3) parameter3InstructionValue = intCode[3];

                    var instructionString = intCode[0].ToString();
                    InstructionLength = instructionString.Length;
                    instructionString = instructionString.PadLeft(5, '0');
                    InstructionID = Convert.ToInt32(new string(instructionString.TakeLast(2).ToArray()));
                    OpCodeInstruction = (OpCode)InstructionID;
                    Paramater1Mode = (ParameterMode)Convert.ToInt32(instructionString.Skip(2).Take(1).FirstOrDefault().ToString());

                    if (intCode.Length > 2) Paramater2Mode = (ParameterMode)Convert.ToInt32(instructionString.Skip(1).Take(1).FirstOrDefault().ToString());
                    if (intCode.Length > 3) Paramater3Mode = (ParameterMode)Convert.ToInt32(instructionString.Take(1).FirstOrDefault().ToString());
                }
                else
                {
                    InstructionID = 99;
                }
            }

            public static int? GetOpCodeLength(OpCode dirstOpcode)
            {
                switch (dirstOpcode)
                {
                    case OpCode.Adds:
                    case OpCode.Multiplie:
                    case OpCode.isLessThen:
                    case OpCode.isEqual:
                        return 4;
                    case OpCode.JumpIfFalse:
                    case OpCode.JumpIfTrue:
                        return 3;
                    case OpCode.Inputstuff:
                    case OpCode.OutputStuff:
                        return 2;
                    default:
                        return null;
                }
            }

            public int InstructionLength;
            public int InstructionID;
            public int Pointer;
            public OpCode OpCodeInstruction;
            public ParameterMode Paramater1Mode;
            public ParameterMode Paramater2Mode;
            public ParameterMode Paramater3Mode;
            public int parameter1InstructionValue;
            public int parameter2InstructionValue;
            public int parameter3InstructionValue;
            public int parameter1Value;
            public int parameter2Value;
            public int parameter3Value;
            public bool Terminate => InstructionID == 99;

            public enum ParameterMode
            {
                Position = 0,
                Immediate = 1,
            }

            public void ExecuteOperation()
            {

                switch (OpCodeInstruction)
                {
                    case OpCode.Adds:
                        parameter3Value = parameter1Value + parameter2Value;
                        break;
                    case OpCode.Multiplie:
                        parameter3Value = parameter1Value * parameter2Value;
                        break;
                    case OpCode.Inputstuff:
                        break;
                    case OpCode.OutputStuff:
                        break;
                    case OpCode.JumpIfTrue:
                        if (parameter1Value != 0)
                        {
                            Pointer = parameter2Value;
                        }
                        break;
                    case OpCode.JumpIfFalse:
                        if (parameter1Value == 0)
                        {
                            Pointer = parameter2Value;
                        }
                        break;
                    case OpCode.isLessThen:
                        if (parameter1Value < parameter2Value)
                        {
                            parameter3Value = 1;
                        }
                        break;
                    case OpCode.isEqual:
                        if (parameter1Value == parameter2Value)
                        {
                            parameter3Value = 1;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public override async Task Part1()
        {
            var combinations = new List<int[]>();
            var validPhaseNumbers = new int[] { 0, 1, 2, 3, 4 };
            var everyNumber = Enumerable.Range(0, 44444);
            var eveveryValidNumber = everyNumber.Where(x => ValidNumber(x)).Select(x => x.ToString().PadLeft(5, '0')).ToList();
            ResultPart1 = GetMaxNumber(eveveryValidNumber);
        }

        private bool ValidNumber(int number)
        {
            var numberstring = number.ToString();
            numberstring = numberstring.PadLeft(5, '0');
            var distinctnumbers = numberstring.Distinct();

            var invalidChars = new List<char>() { '5', '6', '7', '8', '9' };
            return (distinctnumbers.Count() == 5 && !distinctnumbers.Any(x => invalidChars.Contains(x)));
        }

        private bool ValidNumberPart2(int number)
        {
            var numberstring = number.ToString();
            //numberstring = numberstring.PadLeft(5, '0');
            var distinctnumbers = numberstring.Distinct();

            var invalidChars = new List<char>() { '0', '1', '2', '3', '4' };
            return (distinctnumbers.Count() == 5 && !distinctnumbers.Any(x => invalidChars.Contains(x)));
        }

        public override async Task Part2()
        {
            var combinations = new List<int[]>();
            var validPhaseNumbers = new int[] { 5, 6, 7, 8, 9 };
            var everyNumber = Enumerable.Range(55555, 99999 - 55555);
            var eveveryValidNumber = everyNumber.Where(x => ValidNumberPart2(x)).Select(x => x.ToString().PadLeft(5, '0')).ToList();
            ResultPart2 = GetMaxNumber(eveveryValidNumber);
        }

        private string GetMaxNumber(List<string> eveveryValidNumber)
        {
            var maxNumber = 0;

            foreach (var validNumber in eveveryValidNumber)
            {
                var input = (int?)0;
                var computerMemory1 = new CompuetrMemory(ComputerMemoryInput, Convert.ToInt32(validNumber[0].ToString()));
                var computerMemory2 = new CompuetrMemory(ComputerMemoryInput, Convert.ToInt32(validNumber[1].ToString()));
                var computerMemory3 = new CompuetrMemory(ComputerMemoryInput, Convert.ToInt32(validNumber[2].ToString()));
                var computerMemory4 = new CompuetrMemory(ComputerMemoryInput, Convert.ToInt32(validNumber[3].ToString()));
                var computerMemory5 = new CompuetrMemory(ComputerMemoryInput, Convert.ToInt32(validNumber[4].ToString()));
                var isValid = true;
                var currentInput = 0;
                while (isValid)
                {
                    input = computerMemory1.Step(input);
                    input = computerMemory2.Step(input);
                    input = computerMemory3.Step(input);
                    input = computerMemory4.Step(input);
                    input = computerMemory5.Step(input);

                    if (currentInput == input)
                    {
                        isValid = false;
                    }
                    else
                    {
                        currentInput = input.Value;
                    }
                }

                if (input.HasValue && input > maxNumber)
                {
                    maxNumber = input.Value;
                }
                else if (!input.HasValue)
                {
                    break;
                }
            }

           return maxNumber.ToString();
        }

        public class CompuetrMemory
        {
            public CompuetrMemory(List<int> memList, int _phaseValue)
            {
                ComputerMemory = new List<int>(memList);
                SetFirstOpcode();
                PhaseValue = _phaseValue;
            }

            public bool Halt { get; set; }

            public int? Step(int? inputValue)
            {
                if (inputValue.HasValue)
                {
                    ProgramValue = inputValue.Value;
                    Halt = false;
                    while (!Halt)
                    {
                        var currentOpcode = GetOpcode;
                        if (currentOpcode.Terminate) break;
                        SetNextOpCodeLength();
                        SetSKipValue(currentOpcode);

                        currentOpcode.parameter1Value = currentOpcode.Paramater1Mode == IntCode.ParameterMode.Position ? ComputerMemory[currentOpcode.parameter1InstructionValue] : currentOpcode.parameter1InstructionValue;
                        currentOpcode.parameter2Value = currentOpcode.Paramater2Mode == IntCode.ParameterMode.Position ? ComputerMemory[currentOpcode.parameter2InstructionValue] : currentOpcode.parameter2InstructionValue;
                        currentOpcode.ExecuteOperation();
                        switch (currentOpcode.OpCodeInstruction)
                        {
                            case OpCode.Adds:
                            case OpCode.Multiplie:
                                ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                                break;
                            case OpCode.Inputstuff:
                                ComputerMemory[currentOpcode.parameter1InstructionValue] = ProgramValue;
                                break;
                            case OpCode.OutputStuff:
                                ProgramValue = ComputerMemory[currentOpcode.parameter1InstructionValue];
                                Halt = true;
                                break;
                            case OpCode.JumpIfTrue:
                                if (currentOpcode.parameter1Value != 0)
                                {
                                    Pointer = currentOpcode.parameter2Value;
                                }
                                break;
                            case OpCode.JumpIfFalse:
                                if (currentOpcode.parameter1Value == 0)
                                {
                                    Pointer = currentOpcode.parameter2Value;
                                }
                                break;
                            case OpCode.isLessThen:
                                ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                                break;
                            case OpCode.isEqual:
                                ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                                break;
                            default:
                                break;
                        }
                    };
                    return ProgramValue;
                }
                return null;

            }
            public List<int> ComputerMemory { get; set; }
            public int Skip { get; set; }
            public int OpCodeLength { get; set; }
            private int _Pointer { get; set; }
            private bool second { get; set; }
            public int PhaseValue { get; set; }
            private int _ProgramValue { get; set; }
            public int ProgramValue
            {
                get
                {
                    if (!second)
                    {
                        second = true;
                        return PhaseValue;
                    }
                    return _ProgramValue;

                }
                set { _ProgramValue = value; }
            }
            public int Pointer
            {
                get { return _Pointer; }
                set
                {
                    _Pointer = value;
                    Skip = _Pointer;
                    var calcSkip = ComputerMemory.Skip(Skip).Take(1).FirstOrDefault();
                    var opcode = (OpCode)Convert.ToInt32(new string(calcSkip.ToString().TakeLast(2).ToArray()));

                    var length = IntCode.GetOpCodeLength(opcode);
                    if (length.HasValue) OpCodeLength = length.Value;
                }
            }

            private void SetFirstOpcode()
            {
                currentOpCode = (OpCode)Convert.ToInt32(new string(ComputerMemory[0].ToString().TakeLast(2).ToArray()));
                var length = IntCode.GetOpCodeLength(currentOpCode);
                if (length.HasValue) OpCodeLength = length.Value;
            }

            private OpCode currentOpCode { get; set; }
            public IntCode GetOpcode => new IntCode(ComputerMemory.Skip(Skip).Take(OpCodeLength).ToArray());

            public void SetSKipValue(IntCode currentOpcode)
            {
                var length = IntCode.GetOpCodeLength(currentOpcode.OpCodeInstruction);
                if (length.HasValue) Skip += length.Value;
            }

            public int TakeNextOpcodeID => ComputerMemory.Skip(Skip + OpCodeLength).Take(1).FirstOrDefault();

            public OpCode GetNextOpcode => (OpCode)Convert.ToInt32(new string(TakeNextOpcodeID.ToString().TakeLast(2).ToArray()));

            internal void SetNextOpCodeLength()
            {
                var length = IntCode.GetOpCodeLength(GetNextOpcode);
                if (length.HasValue) OpCodeLength = length.Value;
            }

            internal void SetPhaseValue(int phaseValue)
            {
                PhaseValue = phaseValue;
            }
        }

        private int TestInputs(int programValue, ref int phaseValue, bool setPhase = true)
        {
            var computerMemory = new CompuetrMemory(ComputerMemoryInput, phaseValue);
            computerMemory.SetPhaseValue((setPhase) ? phaseValue : programValue);
            while (true)
            {
                var currentOpcode = computerMemory.GetOpcode;
                if (currentOpcode.Terminate) break;
                computerMemory.SetNextOpCodeLength();
                computerMemory.SetSKipValue(currentOpcode);

                currentOpcode.parameter1Value = currentOpcode.Paramater1Mode == IntCode.ParameterMode.Position ? computerMemory.ComputerMemory[currentOpcode.parameter1InstructionValue] : currentOpcode.parameter1InstructionValue;
                try
                {
                    currentOpcode.parameter2Value = currentOpcode.Paramater2Mode == IntCode.ParameterMode.Position ? computerMemory.ComputerMemory[currentOpcode.parameter2InstructionValue] : currentOpcode.parameter2InstructionValue;
                }
                catch (Exception ex)
                {
                    var test = ex.ToString();
                }

                currentOpcode.ExecuteOperation();
                //computerMemory.Pointer = currentOpcode.Pointer;
                if (currentOpcode.OpCodeInstruction == OpCode.Adds || currentOpcode.OpCodeInstruction == OpCode.Multiplie)
                {
                    computerMemory.ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                }
                else
                {
                    if (currentOpcode.OpCodeInstruction == OpCode.Inputstuff)
                    {
                        computerMemory.ComputerMemory[currentOpcode.parameter1InstructionValue] = computerMemory.ProgramValue;
                    }
                    if (currentOpcode.OpCodeInstruction == OpCode.OutputStuff)
                    {
                        computerMemory.ProgramValue = computerMemory.ComputerMemory[currentOpcode.parameter1InstructionValue];
                    }
                    if (currentOpcode.OpCodeInstruction == OpCode.isLessThen)
                    {
                        computerMemory.ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                    }
                    if (currentOpcode.OpCodeInstruction == OpCode.isEqual)
                    {
                        computerMemory.ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                    }
                    if (currentOpcode.OpCodeInstruction == OpCode.JumpIfTrue)
                    {
                        if (currentOpcode.parameter1Value != 0)
                        {
                            computerMemory.Pointer = currentOpcode.parameter2Value;
                            //computerMemory.Skip = currentOpcode.parameter2Value;
                        }
                    }
                    if (currentOpcode.OpCodeInstruction == OpCode.JumpIfFalse)
                    {
                        if (currentOpcode.parameter1Value == 0)
                        {
                            computerMemory.Pointer = currentOpcode.parameter2Value;
                            //computerMemory.Skip = currentOpcode.parameter2Value;
                        }
                    }
                }
            };
            return computerMemory.ProgramValue;
        }
    }
}

