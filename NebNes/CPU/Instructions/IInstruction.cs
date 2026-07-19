namespace NebNes.CPU.Instructions {
    public interface IInstruction {
        public void runImplicit();
        public void runImmediate(byte value);
        public void runAddress(ushort address);
        public void runAcc();
    }
}
