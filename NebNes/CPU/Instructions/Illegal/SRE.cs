using NebNes.CPU.Instructions.Checks;
using NebNes.Misc;

namespace NebNes.CPU.Instructions.Illegal {
    public class SRE : Instruction, IInstruction {
        EOR eor;
        public SRE(MOS6502 cpu, Bus bus) : base(cpu, bus) {
            eor = new EOR(cpu, bus);
        }

        public void runAcc() { }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, value);
            byte res = runLSR(value);
            bus.write(address, res);
            eor.runImmediate(res);
        }

        public void runImmediate(byte value) { }

        public void runImplicit() { }

        private byte runLSR(byte value) {
            bool setCarry = (value & 0x01) == 1;
            byte newValue = (byte)(value >> 1);
            if (setCarry) cpu.Flags.setFlag(Enums.FlagsIndex.C);
            else cpu.Flags.clearFlag(Enums.FlagsIndex.C);
            cpu.Flags.updateZeroAndNegative(newValue);
            return newValue;
        }
    }
}
