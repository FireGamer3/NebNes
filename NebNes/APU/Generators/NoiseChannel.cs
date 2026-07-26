using NebNes.APU.Misc;
using NebNes.APU.Registers;
using NebNes.Misc;

namespace NebNes.APU.Generators {
    public class NoiseChannel {
        public LengthCounter lengthCounter = new LengthCounter();
        public VolumeEnvelope volumeEnvelope = new VolumeEnvelope();
        private NesAPU apu;
        ushort shift = 1;
        int dividerCount = 0;

        public NoiseChannel(NesAPU apu) {
            this.apu = apu;
        }

        public double sample() {
            if (!enabled() || !lengthCounter.isActive() || (shift & 1) == 1)
                return 0;
            byte volume = registers().control.volumeOrEnvelopePeriod();

            if (registers().control.constantVolume() == 0)
                return volumeEnvelope.volume;
            else return volume;
        }

        public void step() {
            dividerCount++;
            if (dividerCount >= Constants.noisePeriods()[registers().form.periodId()]) {
                dividerCount = 0;
                byte feedbackBit = (byte)(registers().form.mode() == 1
                    ? ByteLib.getBit((byte)shift, 0) ^ ByteLib.getBit((byte)shift, 6)
                    : ByteLib.getBit((byte)shift, 0) ^ ByteLib.getBit((byte)shift, 1));
                shift = (ushort)(shift >> 1);
                shift = (ushort)(shift | (feedbackBit << 14));
            }
        }

        public void quarterFrame() {
            volumeEnvelope.clock(
              registers().control.volumeOrEnvelopePeriod(),
              registers().control.envelopeLoopOrLengthCounterHalt() == 1
            );
        }

        public void halfFrame() {
            lengthCounter.clock(
              enabled(),
              registers().control.envelopeLoopOrLengthCounterHalt() == 1
            );
        }

        public NoiseRegisters registers() {
            return apu.registers.noise;
        }

        private bool enabled() {
            return apu.registers.apuControl.enableNoise() == 1;
        }
    }
}
