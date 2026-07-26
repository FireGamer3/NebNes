using NebNes.Misc;

namespace NebNes.CPU.Instructions.Checks {
    public class CPY : Instruction, IInstruction {
        public CPY(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            runImmediate(bus.read(address));
        }

        public void runImmediate(byte value) {
            bool shouldZ = cpu.Y.get() == value;
            bool isNegative = (((cpu.Y.get() - value) & 0x80) >> 7) == 1;
            bool shouldCarry = cpu.Y.get() >= value;

            if (!shouldZ) cpu.Flags.clearFlag(Enums.FlagsIndex.Z);
            else cpu.Flags.setFlag(Enums.FlagsIndex.Z);
            if (!isNegative) cpu.Flags.clearFlag(Enums.FlagsIndex.N);
            else cpu.Flags.setFlag(Enums.FlagsIndex.N);
            if (!shouldCarry) cpu.Flags.clearFlag(Enums.FlagsIndex.C);
            else cpu.Flags.setFlag(Enums.FlagsIndex.C);
        }

        public void runImplicit() { }
    }
}
