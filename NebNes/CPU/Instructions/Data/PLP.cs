using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class PLP : Instruction, IInstruction {
        public PLP(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            // Pulling flags discards the B flag (bit 4) and forces the unused bit 5 to 1.
            byte val = (byte)((cpu.stack.pop() & 0xEF) | 0x20);
            cpu.Flags.set(val);
        }
    }
}
