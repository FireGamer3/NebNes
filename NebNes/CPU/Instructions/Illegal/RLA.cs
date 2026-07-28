using NebNes.CPU.Instructions.Checks;
using NebNes.Misc;

namespace NebNes.CPU.Instructions.Illegal {
    public class RLA : Instruction, IInstruction {
        AND and;
        public RLA(MOS6502 cpu, Bus bus) : base(cpu, bus) { 
            and = new AND(cpu, bus);
        }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, value);
            byte res = runROL(value);
            bus.write(address, res);
            and.runImmediate(res);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }

        private byte runROL(byte value) {
            bool carry = cpu.Flags.getFlag(Enums.FlagsIndex.C);
            bool setCarry = ((value & 0x80) >> 7) == 1;
            byte newValue = (byte)((byte)(value << 1) + (carry ? 1 : 0));
            if (setCarry) cpu.Flags.setFlag(Enums.FlagsIndex.C);
            else cpu.Flags.clearFlag(Enums.FlagsIndex.C);
            cpu.Flags.updateZeroAndNegative(newValue);
            return newValue;
        }
    }
}
