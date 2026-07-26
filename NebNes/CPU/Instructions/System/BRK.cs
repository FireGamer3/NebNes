using NebNes.Enums;
using NebNes.Misc;

namespace NebNes.CPU.Instructions.System {
    public class BRK : Instruction, IInstruction {
        public BRK(MOS6502 cpu, Bus bus) : base(cpu, bus) { }

        public void runAcc() { }

        public void runAddress(ushort address) { }

        public void runImmediate(byte value) { }

        public void runImplicit() {
            cpu.PC.increment();
            cpu.stack.push(cpu.PC.get());
            cpu.stack.push((byte)(cpu.Flags.get() | 0x30));
            cpu.Flags.setFlag(FlagsIndex.I);
            cpu.PC.set(bus.read16(0xFFFE));
        }
    }
}
