using NebNes.Misc;

namespace NebNes.CPU.Instructions.Data {
    public class LDX : Instruction, IInstruction {
        public LDX(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            runImmediate(bus.read(address));
        }

        public void runImmediate(byte value) {
            cpu.X.set(value);
            cpu.Flags.updateZeroAndNegative(value);
        }

        public void runImplicit() { }
    }
}
