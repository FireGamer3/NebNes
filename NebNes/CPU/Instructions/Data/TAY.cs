using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class TAY : Instruction, IInstruction {
        public TAY(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte val = cpu.A.get();
            cpu.Y.set(val);
            cpu.Flags.updateZeroAndNegative(val);
        }
    }
}
