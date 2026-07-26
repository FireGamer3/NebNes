namespace NebNes.APU.Registers {
    public class PulseTimerLow : BaseRegister {
        public PulseTimerLow(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
            apu.pulses[id].updateTimer();
        }
    }
}
