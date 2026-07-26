namespace NebNes.APU.Misc {
    public static class Constants {
        private static readonly int[] _noteLengths = [
              10,
              254,
              20,
              2,
              40,
              4,
              80,
              6,
              160,
              8,
              60,
              10,
              14,
              12,
              26,
              14,
              12,
              16,
              24,
              18,
              48,
              20,
              96,
              22,
              192,
              24,
              72,
              26,
              16,
              28,
              32,
              30
            ];

        private static readonly int[] _noisePeriods = [
              2,
              4,
              8,
              16,
              32,
              48,
              64,
              80,
              101,
              127,
              190,
              254,
              381,
              508,
              1017,
              2034
            ];

        public static int[] noteLengths() => _noteLengths;

        public static int[] noisePeriods() => _noisePeriods;
    }
}
