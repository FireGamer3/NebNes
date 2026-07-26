namespace NebNes.Frontend {
    /// <summary>
    /// Placeholder framebuffer shown until a ROM is loaded: animated grayscale
    /// noise, like an untuned analog TV. Rendered every frame so it flickers.
    /// </summary>
    internal static class TvStatic {
        private static readonly Random Rng = new();

        public static void Render(uint[] fb) {
            for (int i = 0; i < fb.Length; i++) {
                byte v = (byte)Rng.Next(256);
                fb[i] = 0xFF000000u | (uint)(v << 16) | (uint)(v << 8) | v;
            }
        }
    }
}
