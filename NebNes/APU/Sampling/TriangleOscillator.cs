namespace NebNes.APU.Sampling {
    public class TriangleOscillator {
        int[] TRIANGLE_SEQUENCE = [
          15,
          14,
          13,
          12,
          11,
          10,
          9,
          8,
          7,
          6,
          5,
          4,
          3,
          2,
          1,
          0,
          0,
          1,
          2,
          3,
          4,
          5,
          6,
          7,
          8,
          9,
          10,
          11,
          12,
          13,
          14,
          15
        ];
        public double frequency = 0;

        private double phase = 0;


        public float sample() {
            phase = (phase + frequency / 44100) % 1;
            int step = (int)Math.Floor(phase * TRIANGLE_SEQUENCE.Length);

            return TRIANGLE_SEQUENCE[step];
        }
    }
}
