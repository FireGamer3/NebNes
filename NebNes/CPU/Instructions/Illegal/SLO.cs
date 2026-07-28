using NebNes.CPU.Instructions.Checks;
using NebNes.Misc;

namespace NebNes.CPU.Instructions.Illegal {
    public class SLO : Instruction, IInstruction {
        ORA ora;
        public SLO(MOS6502 cpu, Bus bus) : base(cpu, bus) {
            ora = new ORA(cpu, bus);
        }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, value);
            byte res = runASL(value);
            bus.write(address, res);
            ora.runImmediate(res);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }

        private byte runASL(byte value) {
            bool setCarry = ((value & 0x80) >> 7) == 1;
            byte newValue = (byte)(value << 1);
            if (setCarry) cpu.Flags.setFlag(Enums.FlagsIndex.C);
            else cpu.Flags.clearFlag(Enums.FlagsIndex.C);
            cpu.Flags.updateZeroAndNegative(newValue);
            return newValue;
        }
    }
}
