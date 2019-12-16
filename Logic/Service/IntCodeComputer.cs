using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Logic.Service
{
    public class IntCodeComputer
    {
        public IntCodeComputer(List<long> computerMemory, long startValue)
        {
            LocalComputerMemory = new ComputerMemory(0, computerMemory, startValue, true);
        }

        public long Start()
        {
            while (true)
            {
                //Get opcode from pointer
                var opcode = GetOpCodeFromCurrentPointer();
                opcode.Run(LocalComputerMemory);
                if (opcode.Exit)
                {
                    return LocalComputerMemory.ProgramValue;
                }
            }
        }

        private IOpcode GetOpCodeFromCurrentPointer()
        {
            var opcodeValue = Convert.ToUInt32(new string(LocalComputerMemory.PointerValue().ToString().TakeLast(2).ToArray()));
            IOpcode opcode = null;
            switch ((OpCode)opcodeValue)
            {
                case OpCode.Adds:
                    opcode = new Add();
                    break;
                case OpCode.Multiplie:
                    opcode = new Multiply();
                    break;
                case OpCode.Inputstuff:
                    opcode = new Input();
                    break;
                case OpCode.OutputStuff:
                    opcode = new Output();
                    break;
                case OpCode.JumpIfTrue:
                    opcode = new JumpIfTrue();
                    break;
                case OpCode.JumpIfFalse:
                    opcode = new JumpIfFalse();
                    break;
                case OpCode.isLessThen:
                    opcode = new LessThan();
                    break;
                case OpCode.isEqual:
                    opcode = new Equal();
                    break;
                case OpCode.RelativeBase:
                    opcode = new RelativeBase();
                    break;
                case OpCode.Exit:
                    opcode = new Exit();
                    break;
                default:
                    break;
            }
            return opcode;
        }

        public ComputerMemory LocalComputerMemory { get; set; }

        public interface IOpcode
        {
            public bool Exit { get; set; }
            public void Run(ComputerMemory computerMemory);
        }

        public enum ParameterMode
        {
            Position = 0,
            Immediate = 1,
            Relative = 2
        }

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
            RelativeBase = 9,
            Exit = 99
        }

        private abstract class Opcode
        {
            public int Length { get; set; }
            public long[] InstructionParameters { get; set; }
            public long[] ValueParameters { get; set; }
            public long Value { get; set; }
            public bool Exit { get; set; }
            public ParameterMode WriteMode { get; set; }

            public void SetParameterArrays(int length)
            {
                ValueParameters = new long[length];
                InstructionParameters = new long[length];
            }

            public long RelativeBaseWhenWriteModeIsReliativeBase(ComputerMemory computerMemory)
            {
                return WriteMode == ParameterMode.Relative ? computerMemory.RelativeBase : 0;
            }

            public void SetValueParameters(ComputerMemory computerMemory)
            {
                var instructionValues = computerMemory.Memory.Skip((int)computerMemory.GetPointer()).Take(Length).ToArray();
                var instruction = new string(instructionValues[0].ToString().PadLeft(5, '0').Take(3).Reverse().ToArray());
                //Debug.WriteLine(string.Join(',', instructionValues));
                for (int param = 1; param < Length; param++)
                {
                    InstructionParameters[param] = Convert.ToInt32(instruction.Skip(param - 1).Take(1).FirstOrDefault().ToString());
                }

                var i = 1;
                while (i < instructionValues.Count() - 1)
                {
                    switch ((ParameterMode)InstructionParameters[i])
                    {
                        case ParameterMode.Position:
                            ValueParameters[i] = computerMemory.Memory[(int)instructionValues[i]];
                            break;
                        case ParameterMode.Immediate:
                            ValueParameters[i] = instructionValues[i];
                            break;
                        case ParameterMode.Relative:
                            ValueParameters[i] = computerMemory.Memory[(int)(instructionValues[i] + computerMemory.RelativeBase)];
                            break;
                        default:
                            break;
                    }
                    i++;
                }
                WriteMode = (ParameterMode)InstructionParameters[i];
                ValueParameters[i] = instructionValues[i];
            }
        }

        public class ComputerMemory
        {
            public ComputerMemory(int pointer, List<long> memory, long startValue, bool expand = false)
            {
                this.pointer = pointer;
                Memory = memory;
                ProgramValue = startValue;
                if (expand)
                {
                    var addRange = Enumerable.Range(0, 9999999).Select(x => Convert.ToInt64(0)).ToList();
                    Memory.AddRange(addRange);
                }
            }
            public long PointerValue()
            {
                return Memory[(int)pointer];
            }

            private long relativeBase;

            public long RelativeBase
            {
                get { return relativeBase; }
                set { relativeBase = value; }
            }

            public long GetPointer()
            {
                return pointer;
            }
            private long pointer { get; set; }
            private long programValue;

            public long ProgramValue
            {
                get { return programValue; }
                set { programValue = value; }
            }

            public List<long> Memory { get; set; }

            internal void SetNewPointer(int length)
            {
                pointer += length;
            }

            internal void SetPointer(long v)
            {
                pointer = v;
            }
        }

        public const int PARAMETER1 = 1;
        public const int PARAMETER2 = 2;

        private class Exit : Opcode, IOpcode
        {
            public void Run(ComputerMemory computerMemory)
            {
                Exit = true;
            }
        }

        private class Add : Opcode, IOpcode
        {
            public Add()
            {
                Length = 4;
                SetParameterArrays(Length);
            }
            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                Value = ValueParameters[PARAMETER2] + ValueParameters[PARAMETER1];
                computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)] = Value;
                computerMemory.SetNewPointer(Length);
            }
        }

        private class Multiply : Opcode, IOpcode
        {
            public Multiply()
            {
                Length = 4;
                SetParameterArrays(Length);
            }

            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                Value = ValueParameters[PARAMETER2] * ValueParameters[PARAMETER1];
                computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)] = Value;
                computerMemory.SetNewPointer(Length);
            }
        }

        private class Input : Opcode, IOpcode
        {
            public Input()
            {
                Length = 2;
                SetParameterArrays(Length);
            }

            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)] = computerMemory.ProgramValue;
                computerMemory.SetNewPointer(Length);
            }
        }

        private class Output : Opcode, IOpcode
        {
            public Output()
            {
                Length = 2;
                SetParameterArrays(Length);
            }
            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                switch (WriteMode)
                {
                    case ParameterMode.Position:
                    case ParameterMode.Relative:
                        computerMemory.ProgramValue = computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)];
                        break;
                    case ParameterMode.Immediate:
                        computerMemory.ProgramValue = ValueParameters[Length - 1];
                        break;
                }
                computerMemory.SetNewPointer(Length);
            }
        }


        private class JumpIfFalse : Opcode, IOpcode
        {
            public JumpIfFalse()
            {
                Length = 3;
                SetParameterArrays(Length);
            }
            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                if (ValueParameters[1] == (long)0)
                {
                    switch (WriteMode)
                    {
                        case ParameterMode.Position:
                        case ParameterMode.Relative:
                            computerMemory.SetPointer(computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)]);
                            break;
                        case ParameterMode.Immediate:
                            computerMemory.SetPointer(ValueParameters[Length - 1]);
                            break;
                    }
                }
                else
                {
                    computerMemory.SetNewPointer(Length);
                }
            }
        }

        private class JumpIfTrue : Opcode, IOpcode
        {
            public JumpIfTrue()
            {
                Length = 3;
                SetParameterArrays(Length);
            }
            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                if (ValueParameters[1] != 0)
                {
                    switch (WriteMode)
                    {
                        case ParameterMode.Position:
                        case ParameterMode.Relative:
                            computerMemory.SetPointer(computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)]);
                            break;
                        case ParameterMode.Immediate:
                            computerMemory.SetPointer(ValueParameters[Length - 1]);
                            break;
                    }
                }
                else
                {
                    computerMemory.SetNewPointer(Length);
                }
            }
        }

        private class LessThan : Opcode, IOpcode
        {
            public LessThan()
            {
                Length = 4;
                SetParameterArrays(Length);
            }
            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                if (ValueParameters[1] < ValueParameters[2])
                {
                    computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)] = 1;
                }
                else
                {
                    computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)] = 0;
                }
                computerMemory.SetNewPointer(Length);
            }
        }
        private class Equal : Opcode, IOpcode
        {
            public Equal()
            {
                Length = 4;
                SetParameterArrays(Length);
            }
            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                if (ValueParameters[1] == ValueParameters[2])
                {
                    computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)] = 1;
                }
                else
                {
                    computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)] = 0;
                }
                computerMemory.SetNewPointer(Length);
            }
        }

        private class RelativeBase : Opcode, IOpcode
        {
            public RelativeBase()
            {
                Length = 2;
                SetParameterArrays(Length);
            }
            public void Run(ComputerMemory computerMemory)
            {
                SetValueParameters(computerMemory);
                switch (WriteMode)
                {
                    case ParameterMode.Position:
                    case ParameterMode.Relative:
                        computerMemory.RelativeBase += computerMemory.Memory[(int)ValueParameters[Length - 1] + (int)RelativeBaseWhenWriteModeIsReliativeBase(computerMemory)];
                        break;
                    case ParameterMode.Immediate:
                        computerMemory.RelativeBase += (int)ValueParameters[Length - 1];
                        break;
                }
                computerMemory.SetNewPointer(Length);
            }
        }
    }
}