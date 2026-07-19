using NebNes.CPU.Registers;
using NebNes.Enums;
using NebNes.Misc;

namespace NebNes.CPU {
    public class MOS6502 {
        private Bus bus;
        public Register8Bit A = new Register8Bit();
        public Register8Bit X = new Register8Bit();
        public Register8Bit Y = new Register8Bit();
        public Register8Bit SP = new Register8Bit();
        public Register16Bit PC = new Register16Bit();
        public RegisterFlags Flags = new RegisterFlags();
        public Stack stack;
        private int cycles = 0;
        private int extraCycles = 0;

        public MOS6502(Bus bus) {
            this.bus = bus;
            stack = new Stack(bus, SP);
        }

        public void addCycles(int cycles = 1) {
            this.cycles += cycles;
        }

        public void addExtraCycles(int cycles = 1) {
            extraCycles += cycles;
        }

        public void clearExtraCycles() {
            extraCycles = 0;
        }

        private ushort resolveAddress(AddressingMode mode) {
            switch (mode) {
                case AddressingMode.ZERO_PAGE:
                    return fetchByte();

                case AddressingMode.ZERO_PAGE_INDEXED_X:
                    return (byte)(fetchByte() + X.get());   // wraps within page 0

                case AddressingMode.ZERO_PAGE_INDEXED_Y:
                    return (byte)(fetchByte() + Y.get());

                case AddressingMode.ABSOLUTE:
                    return fetchWord();

                case AddressingMode.ABSOLUTE_INDEXED_X: {
                        ushort baseAddr = fetchWord();
                        ushort addr = (ushort)(baseAddr + X.get());
                        checkPageCross(baseAddr, addr);
                        return addr;
                    }

                case AddressingMode.ABSOLUTE_INDEXED_Y: {
                        ushort baseAddr = fetchWord();
                        ushort addr = (ushort)(baseAddr + Y.get());
                        checkPageCross(baseAddr, addr);
                        return addr;
                    }

                case AddressingMode.INDIRECT: {                 // JMP ($xxxx) only
                        ushort ptr = fetchWord();
                        byte lo = bus.read(ptr);
                        // hardware bug: high byte does NOT cross the page
                        byte hi = bus.read((ushort)((ptr & 0xFF00) | ((ptr + 1) & 0x00FF)));
                        return (ushort)(lo | (hi << 8));
                    }

                case AddressingMode.INDEXED_INDIRECT: {         // (zp,X)
                        byte zp = (byte)(fetchByte() + X.get());
                        byte lo = bus.read(zp);
                        byte hi = bus.read((byte)(zp + 1));         // wraps in zero page
                        return (ushort)(lo | (hi << 8));
                    }

                case AddressingMode.INDIRECT_INDEXED: {         // (zp),Y
                        byte zp = fetchByte();
                        byte lo = bus.read(zp);
                        byte hi = bus.read((byte)(zp + 1));
                        ushort baseAddr = (ushort)(lo | (hi << 8));
                        ushort addr = (ushort)(baseAddr + Y.get());
                        checkPageCross(baseAddr, addr);
                        return addr;
                    }

                case AddressingMode.RELATIVE: {                 // branches
                        sbyte offset = (sbyte)fetchByte();
                        return (ushort)(PC.get() + offset);
                    }

                default:
                    throw new InvalidOperationException($"{mode} has no effective address");
            }
        }

        private byte fetchByte() {
            byte value = bus.read(PC.get());
            PC.increment();
            return value;
        }

        private ushort fetchWord() {
            byte lo = fetchByte();
            byte hi = fetchByte();
            return (ushort)(lo | (hi << 8));
        }

        private void checkPageCross(ushort baseAddr, ushort addr) {
            if ((baseAddr & 0xFF00) != (addr & 0xFF00))
                addExtraCycles();
        }
    }
}
