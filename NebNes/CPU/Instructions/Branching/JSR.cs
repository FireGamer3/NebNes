using NebNes.Misc;

namespace NebNes.CPU.Instructions.Branching {
    public class JSR : Instruction, IInstruction {
        public JSR(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            cpu.stack.push((ushort)(cpu.PC.get() - 1));
            cpu.PC.set(address);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
