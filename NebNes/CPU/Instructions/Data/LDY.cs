using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class LDY : Instruction, IInstruction {
        public LDY(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            runImmediate(bus.read(address));
        }

        public void runImmediate(byte value) {
            cpu.Y.set(value);
            cpu.Flags.updateZeroAndNegative(value);
        }

        public void runImplicit() { }
    }
}
