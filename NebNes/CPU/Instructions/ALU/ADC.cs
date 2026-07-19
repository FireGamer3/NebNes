using NebNes.Misc;

namespace NebNes.CPU.Instructions.ALU {
    public class ADC : Instruction, IInstruction {
        public ADC(MOS6502 cpu, Bus bus) : base(cpu, bus) {
        }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) {
            byte oldValue = cpu.A.get();
            byte result = (byte)(oldValue + value + (!cpu.Flags.getFlag(Enums.FlagsIndex.C) ? 0 : 1));
            ushort shortResult = (ushort)(oldValue + value + (!cpu.Flags.getFlag(Enums.FlagsIndex.C) ? 0 : 1));
            cpu.A.set(result);
            cpu.Flags.updateZeroAndNegative(result);

            bool v = (ByteLib.isPositive(oldValue) &&
                 ByteLib.isPositive(value) &&
                 ByteLib.isNegative(result)) ||
                (ByteLib.isNegative(oldValue) &&
                 ByteLib.isNegative(value) &&
                 ByteLib.isPositive(result));

            if (shortResult > 0x00FF)
                cpu.Flags.setFlag(Enums.FlagsIndex.C);
            else cpu.Flags.clearFlag(Enums.FlagsIndex.C);
            if (v) cpu.Flags.setFlag(Enums.FlagsIndex.V);
            else cpu.Flags.clearFlag(Enums.FlagsIndex.V);
        }

        public void runImplicit() { }
    }
}
