using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class DEC : Instruction, IInstruction {
        public DEC(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            value -= 1;
            bus.write(address, value);
            cpu.Flags.updateZeroAndNegative(value);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
