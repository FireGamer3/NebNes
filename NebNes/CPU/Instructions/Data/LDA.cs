using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class LDA : Instruction, IInstruction {
        public LDA(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) {
            cpu.A.set(value);
            cpu.Flags.updateZeroAndNegative(value);
        }

        public void runImplicit() { }
    }
}
