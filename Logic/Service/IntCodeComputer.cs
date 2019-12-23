using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Service
{
    public class Packet
    {
        public long? DestinationAddress { get; set; }
        public long? X { get; set; }
        public long? Y { get; set; }

        public void Fill(long value)
        {
            if (!DestinationAddress.HasValue)
            {
                DestinationAddress = value;
            }
            else if (!X.HasValue)
            {
                X = value;
            }
            else if (!Y.HasValue)
            {
                Y = value;
            }
        }

        public long? TryGetInput()
        {
            long? returnvalue = null;
            if (X.HasValue)
            {
                returnvalue = X;
                X = null;
            }
            else if (Y.HasValue)
            {
                returnvalue = Y;
                Y = null;
            }
            return returnvalue;
        }


        public bool IsReadyForShipment()
        {
            return DestinationAddress.HasValue && X.HasValue && Y.HasValue;
        }
        public bool IsEmpty()
        {
            return !X.HasValue && !Y.HasValue;
        }
    }

    public class IntCodeComputer
    {
        public long ID { get; private set; }
        public IntCodeComputer(List<long> computerMemory, long startValue)
        {
            ID = startValue;
            LocalComputerMemory = new ComputerMemory(0, computerMemory, startValue, true);
        }

        public long Start(long? value = null)
        {
            if (value.HasValue)
            {
                LocalComputerMemory.ProgramValue = value.Value;
            }

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

        public long[] StartAndReturn2Outputs(long? value = null)
        {
            var returnValues = new long[3];
            var count = 0;
            if (value.HasValue)
            {
                LocalComputerMemory.ProgramValue = value.Value;
            }

            while (true)
            {
                //Get opcode from pointer
                var opcode = GetOpCodeFromCurrentPointer();
                opcode.Run(LocalComputerMemory);
                if (opcode is Exit)
                {
                    returnValues[2] = 1;
                    break;
                }
                else if (opcode is Output)
                {
                    returnValues[count] = LocalComputerMemory.ProgramValue;
                    count++;
                }

                if (count == 2)
                {
                    break;
                }
            }

            return returnValues;
        }


        public long[] StartAndReturn3Outputs(long? value = null)
        {
            var returnValues = new long[4];
            var count = 0;
            if (value.HasValue)
            {
                LocalComputerMemory.ProgramValue = value.Value;
            }

            while (true)
            {
                //Get opcode from pointer
                var opcode = GetOpCodeFromCurrentPointer();
                opcode.Run(LocalComputerMemory);
                if (opcode is Exit)
                {
                    returnValues[3] = 1;
                    break;
                }
                else if (opcode is Output)
                {
                    returnValues[count] = LocalComputerMemory.ProgramValue;
                    count++;
                }

                if (count == 3)
                {
                    break;
                }
            }

            return returnValues;
        }

        //Added - Day23
        private Packet inputPacket { get; set; } = null;
        private Packet outputPacket { get; set; } = null;
        private bool firstInput { get; set; } = true;
        private bool isIdle { get; set; }
        private int idleCount { get; set; }
        public long? RunTillOutput(ConcurrentDictionary<long, Queue<Packet>> packets, CancellationTokenSource cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var opcode = GetOpCodeFromCurrentPointer();

                if (opcode is Input)
                {
                    if (firstInput)
                    {
                        firstInput = false;
                    }
                    else
                    {
                        long value;
                        if (inputPacket == null)
                        {
                            packets.TryGetValue(ID, out Queue<Packet> packetQueue);
                            if (packetQueue != null)
                            {
                                packetQueue.TryDequeue(out Packet dequeudInputPacket);
                                if (dequeudInputPacket != null)
                                {
                                    inputPacket = dequeudInputPacket;
                                    Debug.WriteLine($"Dequeue from {packetQueue.Count() + 1} items\t{inputPacket.DestinationAddress} X={inputPacket.X} Y={inputPacket.Y}");
                                }
                            }
                            value = inputPacket?.TryGetInput() ?? -1;
                        }
                        else
                        {
                            value = inputPacket.TryGetInput() ?? -1;
                            if (inputPacket.IsEmpty()) inputPacket = null;
                        }

                        isIdle = value == -1 && LocalComputerMemory.ProgramValue == -1;

                        if (isIdle)
                        {
                            idleCount++;
                            Thread.Sleep(1000);
                        }
                        LocalComputerMemory.ProgramValue = value;
                    }
                }

                opcode.Run(LocalComputerMemory);

                if (opcode is Output)
                {
                    if (outputPacket == null)
                    {
                        outputPacket = new Packet();
                    }
                    isIdle = false;
                    idleCount = 0;
                    outputPacket.Fill(LocalComputerMemory.ProgramValue);
                    if (outputPacket.IsReadyForShipment())
                    {
                        var newPacket = new Packet() { DestinationAddress = outputPacket.DestinationAddress, X = outputPacket.X, Y = outputPacket.Y };
                        packets.TryGetValue(outputPacket.DestinationAddress.Value, out Queue<Packet> packetQueue);

                        if (newPacket.DestinationAddress.Value == 255)
                        {
                            cancellationToken.Cancel();
                            return newPacket.Y.Value;
                        }

                        if (packetQueue == null)
                        {
                            packetQueue = new Queue<Packet>();
                            packetQueue.Enqueue(newPacket);
                            Debug.WriteLine($"Enqueue 1st\t{newPacket.DestinationAddress} X={newPacket.X} Y={newPacket.Y}");
                            packets.TryAdd(outputPacket.DestinationAddress.Value, packetQueue);
                        }
                        else
                        {
                            packetQueue.Enqueue(newPacket);
                            Debug.WriteLine($"Enqueue {packetQueue.Count()}nd\t{newPacket.DestinationAddress} X={newPacket.X} Y={newPacket.Y}");
                        }
                        outputPacket = null;
                    }
                }
            }
            return null;
        }

        public long lastValue { get; set; }
        public long? RunTillOutputWithIdleStatus(ConcurrentDictionary<long, Queue<Packet>> packets, ConcurrentDictionary<long, bool> computerIdleStatus, CancellationTokenSource cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (ID == 0 && computerIdleStatus.Count == 50 && computerIdleStatus.All(x => x.Value == true))
                {
                    Packet NATPackage = null;
                    while (packets[255].TryDequeue(out Packet NATPackageCheck))
                    {
                        if (NATPackageCheck != null) NATPackage = NATPackageCheck;
                    }

                    if (lastValue == NATPackage.Y.Value)
                    {

                        cancellationToken.Cancel();
                        return NATPackage.Y.Value;
                    }

                    lastValue = NATPackage.Y.Value;

                    var newPacket = new Packet() { DestinationAddress = 0, X = NATPackage.X, Y = NATPackage.Y };

                    var queue = new Queue<Packet>();
                    queue.Enqueue(newPacket);
                    if (!packets.TryAdd(ID, queue))
                    {
                        packets[ID].Enqueue(newPacket);
                    }
                    computerIdleStatus.Clear();
                    Debug.WriteLine($"NAT PACKAGE {NATPackage.Y}");
                }

                var opcode = GetOpCodeFromCurrentPointer();

                if (opcode is Input)
                {
                    if (firstInput)
                    {
                        firstInput = false;
                    }
                    else
                    {
                        long value;
                        if (inputPacket == null)
                        {
                            packets.TryGetValue(ID, out Queue<Packet> packetQueue);
                            if (packetQueue != null)
                            {
                                packetQueue.TryDequeue(out Packet dequeudInputPacket);
                                if (dequeudInputPacket != null)
                                {
                                    inputPacket = dequeudInputPacket;
                                }
                            }
                            value = inputPacket?.TryGetInput() ?? -1;
                        }
                        else
                        {
                            value = inputPacket.TryGetInput() ?? -1;
                            if (inputPacket.IsEmpty()) inputPacket = null;
                        }

                        isIdle = value == -1 && LocalComputerMemory.ProgramValue == -1;

                        if (isIdle)
                        {
                            idleCount++;
                            if (idleCount >= 3)
                            {
                                if (!computerIdleStatus.TryAdd(ID, true))
                                {
                                    computerIdleStatus[ID] = true;
                                }
                            }
                            Thread.Sleep(250);

                        }
                        LocalComputerMemory.ProgramValue = value;
                    }
                }

                opcode.Run(LocalComputerMemory);

                if (opcode is Output)
                {
                    if (outputPacket == null)
                    {
                        outputPacket = new Packet();
                    }
                    isIdle = false;
                    idleCount = 0;
                    if (!computerIdleStatus.TryAdd(ID, false))
                    {
                        computerIdleStatus[ID] = false;
                    }

                    outputPacket.Fill(LocalComputerMemory.ProgramValue);
                    if (outputPacket.IsReadyForShipment())
                    {
                        var newPacket = new Packet() { DestinationAddress = outputPacket.DestinationAddress, X = outputPacket.X, Y = outputPacket.Y };
                        packets.TryGetValue(outputPacket.DestinationAddress.Value, out Queue<Packet> packetQueue);

                        if (packetQueue == null)
                        {
                            packetQueue = new Queue<Packet>();
                            packetQueue.Enqueue(newPacket);
                            packets.TryAdd(outputPacket.DestinationAddress.Value, packetQueue);
                        }
                        else
                        {
                            packetQueue.Enqueue(newPacket);
                        }
                        outputPacket = null;
                    }
                }
            }
            return null;
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
                    var addRange = Enumerable.Range(0, 99999).Select(x => Convert.ToInt64(0)).ToList();
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
            private long programValue { get; set; }

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
            public Exit()
            {
                Length = 1;
            }

            public void Run(ComputerMemory computerMemory)
            {
                Exit = true;
                computerMemory.SetNewPointer(Length);
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