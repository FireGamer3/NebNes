using NebNes.APU.Misc;
using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class PulseTimerHighLCL : BaseRegister {
        public PulseTimerHighLCL(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            apu.pulses[id].updateTimer();
            int len = Constants.noteLengths()[lengthCounterLoad()];
            apu.pulses[id].lengthCounter.set(len);
            apu.pulses[id].volumeEnvelope.start();
        }

        public byte timerHigh() {
            return ByteLib.getBits(value, 0, 3);
        }

        public byte lengthCounterLoad() {
            return ByteLib.getBits(value, 3, 5);
        }
    }
}
