using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day9 : AdventBase
    {
        public Day9(int day) : base(day)
        {
            Title = "Sensor Boost";
            //TestInput = "109,19,204,-34";
            TestInput = "109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day9;
            ComputerMemoryInput = PuzzleInput.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            SolutionPart1 = "";
            SolutionPart2 = "";
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
            AdjustRelativeBase = 9
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
                    case OpCode.AdjustRelativeBase:
                        return 2;
                    default:
                        return null;
                }
            }

            public int InstructionLength;
            public int InstructionID;
            public int Pointer;
            public int RelativeBaseOffset;
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
                RelativePosition = 2,
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
                    case OpCode.AdjustRelativeBase:
                        RelativeBaseOffset += parameter1Value;
                        break;
                    default:
                        break;
                }
            }
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

                        switch (currentOpcode.Paramater1Mode)
                        {
                            case IntCode.ParameterMode.Position:
                                currentOpcode.parameter1Value = ComputerMemory[currentOpcode.parameter1InstructionValue];
                                break;
                            case IntCode.ParameterMode.Immediate:
                                currentOpcode.parameter1Value = currentOpcode.parameter1InstructionValue;
                                break;
                            case IntCode.ParameterMode.RelativePosition:
                                currentOpcode.parameter1Value = ComputerMemory[currentOpcode.parameter1InstructionValue + RelativeBaseOffset];
                                break;
                            default:
                                break;
                        }
                        switch (currentOpcode.Paramater2Mode)
                        {
                            case IntCode.ParameterMode.Position:
                                currentOpcode.parameter2Value = ComputerMemory[currentOpcode.parameter2InstructionValue];
                                break;
                            case IntCode.ParameterMode.Immediate:
                                currentOpcode.parameter2Value = currentOpcode.parameter2InstructionValue;
                                break;
                            case IntCode.ParameterMode.RelativePosition:
                                currentOpcode.parameter2Value = ComputerMemory[currentOpcode.parameter2InstructionValue + RelativeBaseOffset];
                                break;
                            default:
                                break;
                        }
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
                                if (currentOpcode.parameter1InstructionValue >= 0)
                                {
                                    ProgramValue = ComputerMemory[currentOpcode.parameter1InstructionValue];
                                }
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
                            case OpCode.AdjustRelativeBase:
                                RelativeBaseOffset += currentOpcode.parameter1Value;
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
            public int RelativeBaseOffset { get; set; } = 0;
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


        public override async Task Part1()
        {
            var computerMemory = new CompuetrMemory(ComputerMemoryInput, Convert.ToInt32(0));
            var isValid = true;
            var input = (int?)0;
            var currentInput = input.Value;
            while (isValid)
            {
                input = computerMemory.Step(input);

                if (currentInput == input)
                {
                    isValid = false;
                }
                else
                {
                    currentInput = input.Value;
                }
            }

            ResultPart1 = input.ToString();
            
        }
        public override async Task Part2() => ResultPart2 = "";

    }
}

