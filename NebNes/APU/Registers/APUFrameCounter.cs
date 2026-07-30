using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class APUFrameCounter : BaseRegister {
        public APUFrameCounter(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            apu.frameSequencer.reset();
            apu.onQuarterFrameClock();
            apu.onHalfFrameClock();
            if (interruptInhibitFlag() == 1)
                apu.cpu.PendingInterrupts.clearInterrupt(Enums.InterruptIndex.APU_FRAME);
        }

        public byte use5StepSequencer() {
            return ByteLib.getBit(value, 7);
        }

        public byte interruptInhibitFlag() {
            return ByteLib.getBit(value, 7);
        }
    }
}
