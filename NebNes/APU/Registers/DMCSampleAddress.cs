namespace NebNes.APU.Registers {
    public class DMCSampleAddress : BaseRegister {
        public DMCSampleAddress(NesAPU apu, int id) : base(apu, id) { }

        public override void onWrite(byte value) {
            set(value);
        }
    }
}
