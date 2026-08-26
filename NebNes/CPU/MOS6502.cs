using NebNes.CPU.Instructions;
using NebNes.CPU.Misc;
using NebNes.CPU.Registers;
using NebNes.Enums;
using NebNes.Misc;
using System.Diagnostics;

namespace NebNes.CPU {
    public class MOS6502 {
        private Bus bus;
        public BaseRegister A = new BaseRegister();
        public BaseRegister X = new BaseRegister();
        public BaseRegister Y = new BaseRegister();
        public BaseRegister SP = new BaseRegister();
        public Register16Bit PC = new Register16Bit();
        public RegisterFlags Flags = new RegisterFlags();
        public RegisterInterrupts PendingInterrupts = new RegisterInterrupts();
        public Stack stack;
        private Operation[] opTable;
        private Operation currentOperation;
        public long currentCycle = 0;
        private int cycles = 0;
        private int extraCycles = 0;

        /// <summary>
        /// Debug hook fired just before each instruction is decoded, with the registers still holding
        /// their pre-instruction values. Null in normal operation; see <see cref="Misc.CpuTracer"/>.
        /// </summary>
        public Action<MOS6502>? OnInstruction;

        public MOS6502(Bus bus) {
            this.bus = bus;
            stack = new Stack(bus, SP);
            opTable = DefineOperations.define(this, bus);
        }

        public int step() {
            if(cycles > 1) {
                cycles--;
                currentCycle++;
                return 0;
            }
            if (PendingInterrupts.get() > 0) handlePendingInterrupt();
            OnInstruction?.Invoke(this);
            ushort originalPC = PC.get();
            Operation operation = fetchOperation();
            if (operation.instruction == null) {
                Console.WriteLine($"Unknown Instruction at ${originalPC:X4} opcode ${bus.read(originalPC):X2}");
                return 0;
            }
            RunOperation(operation);
            int totalCycles = operation.cycles + extraCycles;
            extraCycles = 0;
            cycles += totalCycles - 1;
            return totalCycles - 1;
        }

        public void addCycles(int cycles = 1) {
            this.cycles += cycles;
        }

        public void addExtraCycles(int cycles = 1) {
            extraCycles += cycles;
        }

        public void clearExtraCycles() {
            extraCycles = 0;
        }

        public void triggerInterrupt(InterruptIndex ii) {
            PendingInterrupts.setInterrupt(ii);
        }

        public Operation getOperation(byte opcode) {
            return opTable[opcode];
        }

        public byte read(ushort address) {
            return bus.read(address);
        }

        private void handlePendingInterrupt() {
            if (PendingInterrupts.getInterrupt(InterruptIndex.RESET)) {
                HandleInterrupt(InterruptIndex.RESET, 0xFFFC);
            } else if (PendingInterrupts.getInterrupt(InterruptIndex.NMI)) {
                HandleInterrupt(InterruptIndex.NMI, 0xFFFA);
            } else if (PendingInterrupts.getInterrupt(InterruptIndex.MAPPER)) {
                if (Flags.getFlag(FlagsIndex.I)) return;
                HandleInterrupt(InterruptIndex.MAPPER, 0xFFFE, false);
            }
        }

        private void HandleInterrupt(InterruptIndex inter, ushort vector, bool shouldClear = true) {
            cycles += 7;
            stack.push(PC.get());
            stack.push((byte)((Flags.get() & ~0x10) | 0x20));
            Flags.setFlag(FlagsIndex.I);
            PC.set(bus.read16(vector));
            if(shouldClear)PendingInterrupts.clearInterrupt(inter);
        }

        private Operation fetchOperation() {
            byte opcode = bus.read(PC.get());
            Operation op = opTable[opcode];
            PC.increment();
            return op;
        }

        private void RunOperation(Operation operation) {
            switch (operation.addressMode) {
                case AddressingMode.IMMEDIATE:
                    byte val = fetchByte();
                    operation.instruction.runImmediate(val);
                    break;
                case AddressingMode.IMPLICIT:
                    operation.instruction.runImplicit();
                    break;
                case AddressingMode.ACCUMULATOR:
                    operation.instruction.runAcc();
                    break;
                default:
                    operation.instruction.runAddress(resolveAddress(operation.addressMode));
                    break;
            }
        }

        private ushort resolveAddress(AddressingMode mode) {
            switch (mode) {
                case AddressingMode.ZERO_PAGE:
                    return fetchByte();

                case AddressingMode.ZERO_PAGE_INDEXED_X:
                    return (byte)(fetchByte() + X.get());   // wraps within page 0

                case AddressingMode.ZERO_PAGE_INDEXED_Y:
                    return (byte)(fetchByte() + Y.get());

                case AddressingMode.ABSOLUTE:
                    return fetchWord();

                case AddressingMode.ABSOLUTE_INDEXED_X: {
                        ushort baseAddr = fetchWord();
                        ushort addr = (ushort)(baseAddr + X.get());
                        checkPageCross(baseAddr, addr);
                        return addr;
                    }

                case AddressingMode.ABSOLUTE_INDEXED_Y: {
                        ushort baseAddr = fetchWord();
                        ushort addr = (ushort)(baseAddr + Y.get());
                        checkPageCross(baseAddr, addr);
                        return addr;
                    }

                case AddressingMode.INDIRECT: {                 // JMP ($xxxx) only
                        ushort ptr = fetchWord();
                        byte lo = bus.read(ptr);
                        // hardware bug: high byte does NOT cross the page
                        byte hi = bus.read((ushort)((ptr & 0xFF00) | ((ptr + 1) & 0x00FF)));
                        return (ushort)(lo | (hi << 8));
                    }

                case AddressingMode.INDEXED_INDIRECT: {         // (zp,X)
                        byte zp = (byte)(fetchByte() + X.get());
                        byte lo = bus.read(zp);
                        byte hi = bus.read((byte)(zp + 1));         // wraps in zero page
                        return (ushort)(lo | (hi << 8));
                    }

                case AddressingMode.INDIRECT_INDEXED: {         // (zp),Y
                        byte zp = fetchByte();
                        byte lo = bus.read(zp);
                        byte hi = bus.read((byte)(zp + 1));
                        ushort baseAddr = (ushort)(lo | (hi << 8));
                        ushort addr = (ushort)(baseAddr + Y.get());
                        checkPageCross(baseAddr, addr);
                        return addr;
                    }

                case AddressingMode.RELATIVE: {                 // branches
                        sbyte offset = (sbyte)fetchByte();
                        return (ushort)(PC.get() + offset);
                    }

                default:
                    throw new InvalidOperationException($"{mode} has no effective address");
            }
        }

        private byte fetchByte() {
            byte value = bus.read(PC.get());
            PC.increment();
            return value;
        }

        private ushort fetchWord() {
            byte lo = fetchByte();
            byte hi = fetchByte();
            return (ushort)(lo | (hi << 8));
        }

        private void checkPageCross(ushort baseAddr, ushort addr) {
            if ((baseAddr & 0xFF00) != (addr & 0xFF00))
                addExtraCycles();
        }
    }
}
