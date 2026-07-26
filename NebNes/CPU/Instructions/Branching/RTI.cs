using NebNes.Misc;

namespace NebNes.CPU.Instructions.Branching {
    public class RTI : Instruction, IInstruction {
        public RTI(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.inInterrupt = false;
            // Pulling flags discards the B flag (bit 4) and forces the unused bit 5 to 1.
            cpu.Flags.set((byte)((cpu.stack.pop() & 0xEF) | 0x20));
            cpu.PC.set(cpu.stack.pop16());
        }
    }
}
