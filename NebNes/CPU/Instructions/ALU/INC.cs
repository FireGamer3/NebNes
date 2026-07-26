using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class INC : Instruction, IInstruction {
        public INC(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, value);
            value += 1;
            bus.write(address, value);
            cpu.Flags.updateZeroAndNegative(value);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }
    }
}
