using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class APUFrameCounter : BaseRegister {
        public APUFrameCounter(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            apu.frameSequencer.reset();
            apu.onQuarterFrameClock();
            apu.onHalfFrameClock();
        }

        public byte use5StepSequencer() {
            return ByteLib.getBit(value, 7);
        }
    }
}
