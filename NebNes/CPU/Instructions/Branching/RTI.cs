using NebNes.Misc;

namespace NebNes.CPU.Instructions.Branching {
    public class RTI : Instruction, IInstruction {
        public RTI(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.Flags.set(cpu.stack.pop());
            cpu.PC.set(cpu.stack.pop16());
        }
    }
}
