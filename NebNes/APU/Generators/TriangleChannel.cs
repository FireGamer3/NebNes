using NebNes.APU.Interfaces;
using NebNes.APU.Misc;
using NebNes.APU.Registers;
using NebNes.APU.Sampling;
using NebNes.Misc;

namespace NebNes.APU.Generators {
    public class TriangleChannel {
        public LengthCounter lengthCounter = new LengthCounter();
        public LinearLengthCounter linearLengthCounter = new LinearLengthCounter();

        private NesAPU apu;
        private int timer;
        private TriangleOscillator oscillator = new TriangleOscillator();
        private double lastSample = 0;

        public TriangleChannel(NesAPU apu) {
            this.apu = apu;
        }

        public double sample() {
            ushort timer = ByteLib.buildU16(registers().timerLow.getValue(), registers().timerHighLCL.timerHigh());
            if (timer < 0x0002 || timer > 0x07FF)
                return 0;

            if (!enabled() || !lengthCounter.isActive() || !linearLengthCounter.isActive())
                return lastSample;

            oscillator.frequency = 1789773 / (16 * (timer + 1)) / 2;
            float sample = oscillator.sample();
            lastSample = sample;
            return sample;
        }

        public void quarterFrame() {
            linearLengthCounter.clock(enabled(), registers().lengthControl.halt() == 1);
        }

        public void halfFrame() {
            lengthCounter.clock(enabled(), registers().lengthControl.halt() == 1);
        }

        private bool enabled() {
            return apu.registers.apuControl.enableTriangle() == 1;
        }

        public TriagnleRegisters registers() {
            return apu.registers.triangle;
        }
    }
}
