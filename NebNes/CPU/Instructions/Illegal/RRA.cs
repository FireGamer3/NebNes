using NebNes.CPU.Instructions.ALU;
using NebNes.Misc;

namespace NebNes.CPU.Instructions.Illegal {
    public class RRA : Instruction, IInstruction {
        ADC adc;
        public RRA(MOS6502 cpu, Bus bus) : base(cpu, bus) { 
            adc = new ADC(cpu, bus);
        }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, value);
            byte res = runROR(value);
            bus.write(address, res);
            adc.runImmediate(res);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }

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
