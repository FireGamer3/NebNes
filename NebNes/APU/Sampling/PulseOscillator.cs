namespace NebNes.APU.Sampling {
    public class PulseOscillator {
        public double frequency = 0;
        public int dutyCycle = 0;
        public int volume = 15;

        private double phase = 0;
        private double[] dutyTable = [0.125, 0.25, 0.5, 0.75];


        public float sample() {
            phase = (phase + frequency / 44100) % 1;

            return phase < dutyTable[dutyCycle] ? volume : 0;
        }
    }
}
