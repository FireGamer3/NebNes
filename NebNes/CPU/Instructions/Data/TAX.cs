using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class TAX : Instruction, IInstruction {
        public TAX(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte val = cpu.A.get();
            cpu.X.set(val);
            cpu.Flags.updateZeroAndNegative(val);
        }
    }
}
