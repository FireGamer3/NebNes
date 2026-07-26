using NebNes.APU.Misc;
using NebNes.Misc;

namespace NebNes.APU.Registers {
    public class NoiseLCL : BaseRegister {
        public NoiseLCL(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            int len = Constants.noteLengths()[lengthCounterLoad()];
            apu.noise.lengthCounter.set(len);
            apu.noise.volumeEnvelope.start();
        }

        public byte lengthCounterLoad() {
            return ByteLib.getBits(value, 3, 5);
        }
    }
}
