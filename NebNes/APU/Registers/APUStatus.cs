using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class APUStatus : BaseRegister {
        public APUStatus(NesAPU apu, int id) : base(apu, id) { }

        public override byte onRead() {
            byte b0 = (byte)(apu.pulses[0].lengthCounter.isActive() ? 1 : 0);
            byte b1 = (byte)(apu.pulses[1].lengthCounter.isActive() ? 1 : 0);
            byte b2 = (byte)(apu.triangle.lengthCounter.isActive() ? 1 : 0);
            byte b3 = (byte)(apu.noise.lengthCounter.isActive() ? 1 : 0);
            //byte b4 = apu.dmc.dpcm.remainingBytes() > 0 ? 1 : 0;
            return ByteLib.bitfield(b0, b1, b2, b3, 0, 0, 0, 0);
        }
    }
}
