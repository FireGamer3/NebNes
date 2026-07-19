using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class DEY : Instruction, IInstruction {
        public DEY(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.Y.decrement();
            cpu.Flags.updateZeroAndNegative(cpu.Y.get());
        }
    }
}
