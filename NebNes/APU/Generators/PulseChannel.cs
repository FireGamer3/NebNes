using NebNes.APU.Interfaces;
using NebNes.APU.Misc;
using NebNes.APU.Registers;
using NebNes.APU.Sampling;
using NebNes.Misc;

namespace NebNes.APU.Generators {
    public class PulseChannel : IChannel {
        public LengthCounter lengthCounter = new LengthCounter();
        public VolumeEnvelope volumeEnvelope = new VolumeEnvelope();
        public FrequencySweep frequencySweep;

        private NesAPU apu;
        private int id;
        private int timer;
        private PulseOscillator oscillator = new PulseOscillator();
        private double lastSample = 0;

        public PulseChannel(NesAPU apu, int id) {
            this.apu = apu;
            this.id = id;
            frequencySweep = new FrequencySweep(this);
        }

        public double sample() {
            if(!enabled() || !lengthCounter.isActive() || frequencySweep.mute) {
                return lastSample;
            }
            oscillator.frequency = 1789773 / (16 * (timer + 1));
            oscillator.dutyCycle = registers().control.dutyCycleId();
            oscillator.volume = registers().control.constantVolume() == 1
              ? registers().control.volumeOrEnvelopePeriod()
              : volumeEnvelope.volume;
            double sample = oscillator.sample();
            lastSample = sample;
            return sample;
        }

        public void updateTimer() {
            timer = ByteLib.buildU16(
                registers().timerLow.getValue(),
                registers().timerHighLCL.timerHigh()
            );
        }

        public void step() {
            frequencySweep.muteIfNeeded();
            if (!sweepEnabled()) updateTimer();
        }

        public void quarterFrame() {
            volumeEnvelope.clock(registers().control.volumeOrEnvelopePeriod(),
                                 registers().control.envelopeLoopOrLengthCounterHalt() == 1);
        }

        public void halfFrame() {
            lengthCounter.clock(enabled(), registers().control.envelopeLoopOrLengthCounterHalt() == 1);
            frequencySweep.clock();
        }

        public bool enabled() {
            if (id == 0) return apu.registers.apuControl.enablePulse1() == 1;
            return apu.registers.apuControl.enablePulse2() == 1;
        }

        public bool sweepEnabled() {
            return registers().sweep.enabledFlag() == 1;
        }

        public bool sweepNegate() {
            return registers().sweep.negateFlag() == 1;
        }

        public byte sweepShiftCount() {
            return registers().sweep.shiftCount();
        }

        public byte sweepDividerPeriodMinusOne() {
            return registers().sweep.dividerPeriodMinusOne();
        }

        public int getTimer() {
            return timer;
        }

        public void setTimer(int value) {
            timer = value;
        }

        private PulseRegisters registers() {
            return apu.registers.pulses[id];
        }
    }
}
