namespace NebNes.APU.Registers {
    public class TriangleTimerLow : BaseRegister {
        public TriangleTimerLow(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
        }
    }
}
