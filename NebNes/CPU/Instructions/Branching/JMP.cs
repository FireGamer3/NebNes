using NebNes.Misc;

namespace NebNes.CPU.Instructions.Branching {
    public class JMP : Instruction, IInstruction {
        public JMP(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            cpu.PC.set(address);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
