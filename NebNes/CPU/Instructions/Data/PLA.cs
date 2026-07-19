using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class PLA : Instruction, IInstruction {
        public PLA(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte val = cpu.stack.pop();
            cpu.A.set(val);
            cpu.Flags.updateZeroAndNegative(val);
        }
    }
}
