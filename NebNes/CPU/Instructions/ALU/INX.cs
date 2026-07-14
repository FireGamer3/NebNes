using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class INX : Instruction, IInstruction {
        public INX(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.X.increment();
            cpu.Flags.updateZeroAndNegative(cpu.X.get());
        }

        public void runValue(ushort address, byte value) { }
    }
}
