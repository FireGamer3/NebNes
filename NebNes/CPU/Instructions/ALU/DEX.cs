using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class DEX : Instruction, IInstruction {
        public DEX(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.X.decrement();
            cpu.Flags.updateZeroAndNegative(cpu.X.get());
        }
    }
}
