using NebNes.CPU;
using NebNes.Enums;
using NebNes.Mappers.Interfaces;
using NebNes.Misc;

namespace NebNes.Mappers {
    public class MMC3 : Mapper, IMapper {
        byte[] prgRam = new byte[0x2000];
        byte[] registers = new byte[8];
        byte targetRegister = 0;
        byte prgBankMode = 0;
        byte chrBankMode = 0;
        PPUBgMirroring mirroring;
        byte irqCounterReload = 0;
        byte irqCounter = 0;
        bool irqEnabled = false;
        public MMC3(MOS6502 cpu, Cart cart) : base(cpu, cart) {
            mirroring = cart.getMirroring();
        }

        public byte cpuRead(ushort address) {
            if (address >= 0x6000 && address <= 0x7FFF) {
                return prgRam[address - 0x6000];
            }
            if (address >= 0x8000 && address <= 0x9FFF) {
                if (prgBankMode == 1) return getPrgPage8K(prgPages8k.Length - 2)[address - 0x8000];
                return getPrgPage8K(registers[6])[address - 0x8000];
            }
            if (address >= 0xA000 && address <= 0xBFFF) {
                return getPrgPage8K(registers[7])[address - 0xA000];
            }
            if (address >= 0xC000 && address <= 0xDFFF) {
                if(prgBankMode == 0) return getPrgPage8K(prgPages8k.Length - 2)[address - 0xC000];
                return getPrgPage8K(registers[6])[address - 0xC000];
            }
            if (address >= 0xE000)
                return getPrgPage8K(prgPages8k.Length - 1)[address - 0xE000];
            return 0;
        }

        public void cpuWrite(ushort address, byte value) {
            if (address >= 0x6000 && address <= 0x7FFF) {
                prgRam[address - 0x6000] = value;
                cpu.PendingInterrupts.clearInterrupt(InterruptIndex.MAPPER);
            }
            if ((address & 0x01) == 0) { // Even Register Banks
                if (address >= 0x8000 && address <= 0x9FFE) { //Bank Select
                    targetRegister = ByteLib.getBits(value, 0, 3);
                    prgBankMode = ByteLib.getBit(value, 6);
                    chrBankMode = ByteLib.getBit(value, 7);
                    cpu.PendingInterrupts.clearInterrupt(InterruptIndex.MAPPER);
                }
                if(address >= 0xA000 && address <= 0xBFFE) { // Nametable Mirroring
                    if (ByteLib.getBit(value, 0) == 0) {
                        mirroring = PPUBgMirroring.VERTICAL;
                        cpu.PendingInterrupts.clearInterrupt(InterruptIndex.MAPPER);
                        return;
                    }
                    mirroring = PPUBgMirroring.HORIZONTAL;
                    cpu.PendingInterrupts.clearInterrupt(InterruptIndex.MAPPER);
                }
                if (address >= 0xC000 && address <= 0xDFFE) { //IRQ Latch
                    irqCounterReload = value;
                    cpu.PendingInterrupts.clearInterrupt(InterruptIndex.MAPPER);
                }
                if (address >= 0xE000 && address <= 0xFFFE) { //IRQ Disable
                    irqEnabled = false;
                    cpu.PendingInterrupts.clearInterrupt(InterruptIndex.MAPPER);
                }
            } else { // Odd Register Banks
                if (address >= 0x8001 && address <= 0x9FFF) { //Bank Data
                    if(targetRegister == 0 || targetRegister == 1) {
                        registers[targetRegister] = (byte)((byte)(value & 0xFE) >> 1);
                    } else if(targetRegister == 6 || targetRegister == 7) {
                        registers[targetRegister] = (byte)(value & 0x3F);
                    }else {
                        registers[targetRegister] = value;
                    }
                }
                if (address >= 0xC001 && address <= 0xDFFF)
                    irqCounter = 0;
                if (address >= 0xE001 && address <= 0xFFFF) //IRQ Enable
                    irqEnabled = true;
            }
        }

        public byte ppuRead(ushort address) {
            if(chrBankMode == 0) {
                //2k banks
                if (address <= 0x07FF) 
                    return getChrPage2K(registers[0])[address];
                if (address >= 0x0800 && address <= 0x0FFF) 
                    return getChrPage2K(registers[1])[address - 0x0800];
                //1k banks
                if (address >= 0x1000 && address <= 0x13FF)
                    return getChrPage1K(registers[2])[address - 0x1000];
                if (address >= 0x1400 && address <= 0x17FF)
                    return getChrPage1K(registers[3])[address - 0x1400];
                if (address >= 0x1800 && address <= 0x1BFF)
                    return getChrPage1K(registers[4])[address - 0x1800];
                if (address >= 0x1C00 && address <= 0x1FFF)
                    return getChrPage1K(registers[5])[address - 0x1C00];
            } else {
                //1k Banks
                if (address <= 0x03FF)
                    return getChrPage1K(registers[2])[address];
                if (address >= 0x0400 && address <= 0x07FF)
                    return getChrPage1K(registers[3])[address - 0x0400];
                if (address >= 0x0800 && address <= 0x0BFF)
                    return getChrPage1K(registers[4])[address - 0x0800];
                if (address >= 0x0C00 && address <= 0x0FFF)
                    return getChrPage1K(registers[5])[address - 0x0C00];
                //2k Banks
                if (address >= 0x1000 && address <= 0x17FF)
                    return getChrPage2K(registers[0])[address - 0x1000];
                if (address >= 0x1800 && address <= 0x1FFF)
                    return getChrPage2K(registers[1])[address - 0x1800];
            }
            return 0;
        }

        public void ppuWrite(ushort address, byte value) {
            if(!cart.usesChrRam()) return;
            if (chrBankMode == 0) {
                //2k banks
                if (address <= 0x07FF)
                    getChrPage2K(registers[0])[address] = value;
                if (address >= 0x0800 && address <= 0x0FFF)
                    getChrPage2K(registers[1])[address - 0x0800] = value;
                //1k banks
                if (address >= 0x1000 && address <= 0x13FF)
                    getChrPage1K(registers[2])[address - 0x1000] = value;
                if (address >= 0x1400 && address <= 0x17FF)
                    getChrPage1K(registers[3])[address - 0x1400] = value;
                if (address >= 0x1800 && address <= 0x1BFF)
                    getChrPage1K(registers[4])[address - 0x1800] = value;
                if (address >= 0x1C00 && address <= 0x1FFF)
                    getChrPage1K(registers[5])[address - 0x1C00] = value;
            } else {
                //1k Banks
                if (address <= 0x03FF)
                    getChrPage1K(registers[2])[address] = value;
                if (address >= 0x0400 && address <= 0x07FF)
                    getChrPage1K(registers[3])[address - 0x0400] = value;
                if (address >= 0x0800 && address <= 0x0BFF)
                    getChrPage1K(registers[4])[address - 0x0800] = value;
                if (address >= 0x0C00 && address <= 0x0FFF)
                    getChrPage1K(registers[5])[address - 0x0C00] = value;
                //2k Banks
                if (address >= 0x1000 && address <= 0x17FF)
                    getChrPage2K(registers[0])[address - 0x1000] = value;
                if (address >= 0x1800 && address <= 0x1FFF)
                    getChrPage2K(registers[1])[address - 0x1800] = value;
            }
        }

        public override PPUBgMirroring getMirroring() {
            return mirroring;
        }

        public void tick() {
            if (irqCounter == 0) irqCounter = irqCounterReload;
            else irqCounter--;
            if (irqCounter == 0 && irqEnabled) cpu.PendingInterrupts.setInterrupt(InterruptIndex.MAPPER);
        }
    }
}
