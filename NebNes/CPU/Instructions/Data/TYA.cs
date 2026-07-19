using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class TYA : Instruction, IInstruction {
        public TYA(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte val = cpu.Y.get();
            cpu.A.set(val);
            cpu.Flags.updateZeroAndNegative(val);
        }
    }
}
