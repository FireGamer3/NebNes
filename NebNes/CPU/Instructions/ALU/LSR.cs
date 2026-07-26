using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class LSR : Instruction, IInstruction {
        public LSR(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() {
            byte value = cpu.A.get();
            cpu.A.set(runLSR(value));
        }

        public void runAddress(ushort address) {
            byte value = bus.read(address);
            bus.write(address, value);
            bus.write(address, runLSR(value));

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
