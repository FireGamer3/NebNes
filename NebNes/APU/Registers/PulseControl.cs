using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class PulseControl : BaseRegister {
        public PulseControl(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
        }

        public byte volumeOrEnvelopePeriod() {
            return ByteLib.getBits(value, 0, 4);
        }

        public byte constantVolume() {
            return ByteLib.getBit(value, 4);
        }

        public byte envelopeLoopOrLengthCounterHalt() {
            return ByteLib.getBit(value, 5);
        }

        public byte dutyCycleId() {
            return ByteLib.getBits(value, 6, 2);
        }
    }
}
