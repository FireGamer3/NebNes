using NebNes.APU.Generators;
using NebNes.APU.Misc;
using NebNes.APU.Registers;
using NebNes.CPU;

namespace NebNes.APU {
    public class NesAPU {
        public MOS6502 cpu;
        public FrameSequencer frameSequencer;
        public AudioRegisters registers;
        public PulseChannel[] pulses = new PulseChannel[2];
        public TriangleChannel triangle;
        public NoiseChannel noise;

        int sampleCounter = 0;

        private double dcPrevIn = 0;
        private double dcPrevOut = 0;
        private const double DcBlockR = 0.995;

        public NesAPU(MOS6502 cpu) {
            this.cpu = cpu;
            registers = new AudioRegisters(this);
            frameSequencer = new FrameSequencer(this);
            pulses[0] = new PulseChannel(this, 0);
            pulses[1] = new PulseChannel(this, 1);
            triangle = new TriangleChannel(this);
            noise = new NoiseChannel(this);
        }


        public void step(Action<float> onSample) {
            //step all channels
            pulses[0].step();
            pulses[1].step();
            noise.step();
            sampleCounter++;
            frameSequencer.step();
            if(sampleCounter == 20) {
                sampleCounter = 0;
                double pulse1 = pulses[0].sample();
                double pulse2 = pulses[1].sample();
                double tri = triangle.sample();
                double noisy = noise.sample();

                double raw = (
                    0.00752 * (pulse1 + pulse2) +
                    0.00851 * tri +
                    0.00494 * noisy
                );

                // y[n] = x[n] - x[n-1] + R*y[n-1]
                double filtered = raw - dcPrevIn + DcBlockR * dcPrevOut;
                dcPrevIn = raw;
                dcPrevOut = filtered;

                onSample((float)filtered);
            }
        }

        public void onQuarterFrameClock() {
            pulses[0].quarterFrame();
            pulses[1].quarterFrame();
            triangle.quarterFrame();
            noise.quarterFrame();
        }

        public void onHalfFrameClock() {
            pulses[0].halfFrame();
            pulses[1].halfFrame();
            triangle.halfFrame();
            noise.halfFrame();
        }
    }
}
