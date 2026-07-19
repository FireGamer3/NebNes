using NebNes.Misc;

namespace NebNes.CPU.Instructions.Branching {
    public class RTS : Instruction, IInstruction {
        public RTS(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.PC.set((ushort)(cpu.stack.pop16() + 1));
        }
    }
}
