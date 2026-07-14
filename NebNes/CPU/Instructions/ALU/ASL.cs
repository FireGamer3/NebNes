using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class ASL : Instruction, IInstruction {
        public ASL(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() {
            byte value = cpu.A.get();
            cpu.A.set(runASL(value));
        }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, runASL(value));

        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }

        public void runValue(ushort address, byte value) { }

        private byte runASL(byte value) {
            bool setCarry = ((value & 0x80) >> 7) == 1;
            byte newValue = (byte)(value << 1);
            if (setCarry) cpu.Flags.setFlag(Enums.FlagsIndex.C);
            else cpu.Flags.clearFlag(Enums.FlagsIndex.C);
            cpu.Flags.updateZeroAndNegative(value);
            return newValue;
        }
    }
}
