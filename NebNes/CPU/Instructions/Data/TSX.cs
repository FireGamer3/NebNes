using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class TSX : Instruction, IInstruction {
        public TSX(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte val = cpu.SP.get();
            cpu.X.set(val);
            cpu.Flags.updateZeroAndNegative(val);
        }
    }
}
