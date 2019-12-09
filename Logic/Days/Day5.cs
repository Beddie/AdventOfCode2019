﻿using Logic.Model;
using Logic.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logic.Days
{
    public class Day5 : AdventBase
    {
        public Day5(int day) : base(day)
        {
            Title = "Sunny with a Chance of Asteroids";
            // TestInput = @"3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99";
            PuzzleInput = !string.IsNullOrEmpty(TestInput) ? TestInput : Resources.Day5;
            ComputerMemoryInput = PuzzleInput.Split(',').Select(x => Convert.ToInt32(x)).ToList();
            SolutionPart1 = "7566643";
            SolutionPart2 = "9265694";
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
                if (intCode.Length > 3)  Paramater3Mode = (ParameterMode)Convert.ToInt32(instructionString.Take(1).FirstOrDefault().ToString());
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

            ResultPart1 = TestInputs(1).ToString();
        }

        public override async Task Part2()
        {
            ResultPart2 = TestInputs(5).ToString();
        }

        public class CompuetrMemory
        {
            public CompuetrMemory(List<int> memList)
            {
                ComputerMemory = new List<int>(memList);
                SetFirstOpcode();
            }
            public List<int> ComputerMemory { get; set; }
            public int Skip { get; set; }
            public int OpCodeLength { get; set; }
            private int _Pointer { get; set; }
            public int Pointer
            {
                get { return _Pointer; }
                set
                {
                    _Pointer = value;
                    var calcSkip = ComputerMemory.Skip(Skip).Take(1).FirstOrDefault();
                    var opcode = (OpCode)Convert.ToInt32(new string(calcSkip.ToString().TakeLast(2).ToArray()));

                    var length = IntCode.GetOpCodeLength(opcode);
                    if (length.HasValue) OpCodeLength = length.Value;

                    if (_Pointer > 0)
                    {
                        Skip = _Pointer;
                    }
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

            public int TakeNextOpcodeID =>  ComputerMemory.Skip(Skip + OpCodeLength).Take(1).FirstOrDefault();

            public OpCode GetNextOpcode => (OpCode)Convert.ToInt32(new string(TakeNextOpcodeID.ToString().TakeLast(2).ToArray()));

            internal void SetNextOpCodeLength()
            {
                var length = IntCode.GetOpCodeLength(GetNextOpcode);
                if (length.HasValue) OpCodeLength = length.Value;
            }
        }


        private int TestInputs(int programValue)
        {
            var computerMemory = new CompuetrMemory(ComputerMemoryInput);

            while (true)
            {
                var currentOpcode = computerMemory.GetOpcode;
                if (currentOpcode.Terminate) break;
                computerMemory.SetNextOpCodeLength();
                computerMemory.SetSKipValue(currentOpcode);

                currentOpcode.parameter1Value = currentOpcode.Paramater1Mode == IntCode.ParameterMode.Position ? computerMemory.ComputerMemory[currentOpcode.parameter1InstructionValue] : currentOpcode.parameter1InstructionValue;
                currentOpcode.parameter2Value = currentOpcode.Paramater2Mode == IntCode.ParameterMode.Position ? computerMemory.ComputerMemory[currentOpcode.parameter2InstructionValue] : currentOpcode.parameter2InstructionValue;
                currentOpcode.ExecuteOperation();
                computerMemory.Pointer = currentOpcode.Pointer;
                if (currentOpcode.OpCodeInstruction == OpCode.Adds || currentOpcode.OpCodeInstruction == OpCode.Multiplie)
                {
                    computerMemory.ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                }
                else
                {
                    if (currentOpcode.OpCodeInstruction == OpCode.Inputstuff)
                    {
                        computerMemory.ComputerMemory[currentOpcode.parameter1InstructionValue] = programValue;
                    }
                    if (currentOpcode.OpCodeInstruction == OpCode.OutputStuff)
                    {
                        programValue = computerMemory.ComputerMemory[currentOpcode.parameter1InstructionValue];
                    }
                    if (currentOpcode.OpCodeInstruction == OpCode.isLessThen)
                    {
                        computerMemory.ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                    }
                    if (currentOpcode.OpCodeInstruction == OpCode.isEqual)
                    {
                        computerMemory.ComputerMemory[currentOpcode.parameter3InstructionValue] = currentOpcode.parameter3Value;
                    }
                }
            };
            return programValue;
        }
    }
}
