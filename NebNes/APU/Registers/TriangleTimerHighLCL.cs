using NebNes.APU.Misc;
using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class TriangleTimerHighLCL : BaseRegister {
        public TriangleTimerHighLCL(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            int len = Constants.noteLengths()[lengthCounterLoad()];
            apu.triangle.lengthCounter.set(len);
            apu.triangle.linearLengthCounter.reloadFlag = true;
        }

        public byte timerHigh() {
            return ByteLib.getBits(value, 0, 3);
        }

        public byte lengthCounterLoad() {
            return ByteLib.getBits(value, 3, 5);
        }
    }
}
