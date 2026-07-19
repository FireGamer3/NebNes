using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class TXA : Instruction, IInstruction {
        public TXA(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte val = cpu.X.get();
            cpu.A.set(val);
            cpu.Flags.updateZeroAndNegative(val);
        }
    }
}
