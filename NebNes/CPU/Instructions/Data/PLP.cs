using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class PLP : Instruction, IInstruction {
        public PLP(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            byte val = cpu.stack.pop();
            cpu.Flags.set(val);
        }
    }
}
