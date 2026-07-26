namespace NebNes.APU.Registers {
    public class DMCSampleLength : BaseRegister {
        public DMCSampleLength(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
        }
    }
}
