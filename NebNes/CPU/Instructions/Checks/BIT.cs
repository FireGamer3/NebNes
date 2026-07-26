using NebNes.Misc;

namespace NebNes.CPU.Instructions.Checks {
    public class BIT : Instruction, IInstruction {
        public BIT(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) {
            runImmediate(bus.read(address));
        }

        public void runImmediate(byte value) {
            byte andA = (byte)(value & cpu.A.get());
            if (andA == 0) cpu.Flags.setFlag(Enums.FlagsIndex.Z);
            else cpu.Flags.clearFlag(Enums.FlagsIndex.Z);
            byte newN = (byte)((value & 0x80) >> 7);
            byte newV = (byte)((value & 0x40) >> 6);
            if (newN == 0) cpu.Flags.clearFlag(Enums.FlagsIndex.N);
            else cpu.Flags.setFlag(Enums.FlagsIndex.N);
            if (newV == 0) cpu.Flags.clearFlag(Enums.FlagsIndex.V);
            else cpu.Flags.setFlag(Enums.FlagsIndex.V);
        }

        public void runImplicit() { }
    }
}
