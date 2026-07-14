using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class ROR : Instruction, IInstruction {
        public ROR(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() {
            byte value = cpu.A.get();
            cpu.A.set(runROR(value));
        }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, runROR(value));
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }

        public void runValue(ushort address, byte value) { }

        private byte runROR(byte value) {
            bool carry = cpu.Flags.getFlag(Enums.FlagsIndex.C);
            bool setCarry = (value & 0x01) == 1;
            byte newValue = (byte)((byte)(value >> 1) + (carry ? 0x80 : 0));
            if (setCarry) cpu.Flags.setFlag(Enums.FlagsIndex.C);
            else cpu.Flags.clearFlag(Enums.FlagsIndex.C);
            cpu.Flags.updateZeroAndNegative(newValue);
            return newValue;
        }
    }
}
