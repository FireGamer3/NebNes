using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class SBC : Instruction, IInstruction {
        public SBC(MOS6502 cpu, Bus bus) : base(cpu, bus) {
        }

        public void runAcc() { }

        public void runAddress(ushort address) {
            runImmediate(bus.read(address));
        }

        public void runImmediate(byte value) {
            byte realValue = (byte)(0xFF - value);
            ADC adc = new ADC(cpu, bus);
            adc.runImmediate(realValue);
        }

        public void runImplicit() { }
    }
}
