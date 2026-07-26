using NebNes.CPU;
using NebNes.Mappers;
using NebNes.Misc;
using NebNes.PPU.BG;
using NebNes.PPU.Registers;
using NebNes.PPU.Registers.Loopy;
using NebNes.PPU.Sprites;

namespace NebNes.PPU {
    public class NesPPU {
        public LoopyRegister loopy = new LoopyRegister();
        public MOS6502 cpu;
        public Bus cpuBus;
        public PPUBus bus;
        public OAMAddr oamAddr;
        public OAMData oamData;
        public PPUCtrl ppuCtrl;
        public PPUMask ppuMask;
        public PPUStatus ppuStatus;
        public PPUScroll ppuScroll;
        public PPUAddr ppuAddr;
        public PPUData ppuData;
        public OAMDMA oamdma;
        public int scanline = -1;

        int cycle = 0;
        int frame = 0;
        uint[] frameBuffer = new uint[256 * 240];
        byte[] colorIndexes = new byte[256 * 240];
        IMapper mapper;
        BackgroundRenderer br;
        SpriteRenderer sr;

        public NesPPU(MOS6502 cpu, Bus cpuBus, IMapper mapper) {
            this.mapper = mapper;
            this.cpu = cpu;
            this.cpuBus = cpuBus;
            oamAddr = new OAMAddr(this);
            oamData = new OAMData(this);
            ppuCtrl = new PPUCtrl(this);
            ppuMask = new PPUMask(this);
            ppuStatus = new PPUStatus(this);
            ppuScroll = new PPUScroll(this);
            ppuAddr = new PPUAddr(this);
            ppuData = new PPUData(this);
            oamdma = new OAMDMA(this);
            bus = new PPUBus();
            br = new BackgroundRenderer(this);
            sr = new SpriteRenderer(this);
        }

        public byte registerRead(ushort address) {
            return getRegister(address).onRead();
        }

        public void registerWrite(ushort address, byte value) {
            getRegister(address).onWrite(value);
        }

        public void step(Action<uint[]> onFrame) {
            if (scanline == -1)
                onPreLine();
            else if (scanline < 240)
                onVisibleLine();
            else if (scanline == 241)
                onVBlankLine();

            cycle++;
            if (cycle >= 341) {
                cycle = 0;
                scanline++;
                if (scanline >= 261) {
                    scanline = -1;
                    frame++;
                    onFrame(frameBuffer);
                }
            }
        }

        public uint getColor(byte paletteID, byte colorIndex) {
            ushort startAddress = (ushort)(0x3F00 + paletteID * 4);
            byte masterColorIndex = bus.read((ushort)(startAddress + colorIndex));
            return nesMasterPalette[masterColorIndex % 64];
        }

        public void plotBG(byte x, byte y, uint color, byte colorIndex) {
            colorIndexes[y * 256 + x] = colorIndex;
            plot(x, y, color);
            if (ppuMask.showBackground() == 1)
                loopy.onPlot(x);
        }

        public bool isBackgroundPixelOpaque(byte x, byte y) {
            return colorIndexes[y * 256 + x] > 0;
        }

        public void plot(byte x, byte y, uint color) {
            frameBuffer[y * 256 + x] = ppuMask.transform(color);
        }

        private void onPreLine() {
            if(cycle == 1) {
                ppuStatus.setInVBlankInterval(false);
                ppuStatus.setSpriteOverflow(false);
                ppuStatus.setSprite0Hit(false);
            }
            if (!ppuMask.isRenderingEnabled()) return;
            loopy.onPreLine(cycle);
            if (cycle == 260) mapper.tick();
        }

        private void onVisibleLine() {
            if(cycle == 0) {
                //render
                br.renderScanline();
                sr.renderScanline();
            }
            if (!ppuMask.isRenderingEnabled()) return;
            loopy.onVisibleLine(cycle);
            if (cycle == 260) mapper.tick();
        }

        private void onVBlankLine() {
            if(cycle == 0) {
                ppuStatus.setInVBlankInterval(true);
            }else if(cycle == 1 && ppuCtrl.generateNMIOnVBlank() == 1) {
                cpu.triggerInterrupt(Enums.InterruptIndex.NMI);
            }
        }

        private BaseRegister getRegister(ushort address) {
            switch (address) {
                case 0x2000:
                    return ppuCtrl;
                case 0x2001:
                    return ppuMask;
                case 0x2002:
                    return ppuStatus;
                case 0x2003:
                    return oamAddr;
                case 0x2004:
                    return oamData;
                case 0x2005:
                    return ppuScroll;
                case 0x2006:
                    return ppuAddr;
                case 0x2007:
                    return ppuData;
                case 0x2014:
                    return oamdma;
                default:
                    return oamdma;
            }
        }
        public static uint[] nesMasterPalette = [
          /* 0x00 */ 0xff626262,
          /* 0x01 */ 0xff902001,
          /* 0x02 */ 0xffa00b24,
          /* 0x03 */ 0xff900047,
          /* 0x04 */ 0xff620060,
          /* 0x05 */ 0xff24006a,
          /* 0x06 */ 0xff001160,
          /* 0x07 */ 0xff002747,
          /* 0x08 */ 0xff003c24,
          /* 0x09 */ 0xff004a01,
          /* 0x0a */ 0xff004f00,
          /* 0x0b */ 0xff244700,
          /* 0x0c */ 0xff623600,
          /* 0x0d */ 0xff000000,
          /* 0x0e */ 0xff000000,
          /* 0x0f */ 0xff000000,
          /* 0x10 */ 0xffababab,
          /* 0x11 */ 0xffe1561f,
          /* 0x12 */ 0xffff394d,
          /* 0x13 */ 0xffef237e,
          /* 0x14 */ 0xffb71ba3,
          /* 0x15 */ 0xff6422b4,
          /* 0x16 */ 0xff0e37ac,
          /* 0x17 */ 0xff00558c,
          /* 0x18 */ 0xff00725e,
          /* 0x19 */ 0xff00882d,
          /* 0x1a */ 0xff009007,
          /* 0x1b */ 0xff478900,
          /* 0x1c */ 0xff9d7300,
          /* 0x1d */ 0xff000000,
          /* 0x1e */ 0xff000000,
          /* 0x1f */ 0xff000000,
          /* 0x20 */ 0xffffffff,
          /* 0x21 */ 0xffffac67,
          /* 0x22 */ 0xffff8d95,
          /* 0x23 */ 0xffff75c8,
          /* 0x24 */ 0xffff6af2,
          /* 0x25 */ 0xffc56fff,
          /* 0x26 */ 0xff6a83ff,
          /* 0x27 */ 0xff1fa0e6,
          /* 0x28 */ 0xff00bfb8,
          /* 0x29 */ 0xff01d885,
          /* 0x2a */ 0xff35e35b,
          /* 0x2b */ 0xff88de45,
          /* 0x2c */ 0xffe3ca49,
          /* 0x2d */ 0xff4e4e4e,
          /* 0x2e */ 0xff000000,
          /* 0x2f */ 0xff000000,
          /* 0x30 */ 0xffffffff,
          /* 0x31 */ 0xffffe0bf,
          /* 0x32 */ 0xffffd3d1,
          /* 0x33 */ 0xffffc9e6,
          /* 0x34 */ 0xffffc3f7,
          /* 0x35 */ 0xffeec4ff,
          /* 0x36 */ 0xffc9cbff,
          /* 0x37 */ 0xffa9d7f7,
          /* 0x38 */ 0xff97e3e6,
          /* 0x39 */ 0xff97eed1,
          /* 0x3a */ 0xffa9f3bf,
          /* 0x3b */ 0xffc9f2b5,
          /* 0x3c */ 0xffeeebb5,
          /* 0x3d */ 0xffb8b8b8,
          /* 0x3e */ 0xff000000,
          /* 0x3f */ 0xff000000
        ];
    }
}
