using Silk.NET.SDL;

namespace NebNes.Frontend {
    /// <summary>
    /// Owns the single process-wide SDL binding.
    /// <para>
    /// Silk.NET's <c>Sdl.GetApi()</c> hands back a brand-new wrapper on every call, and
    /// <c>SDL_Quit()</c> on any one of them tears down *every* subsystem globally, ignoring
    /// the init refcount. Sharing one instance — and quitting exactly once, at shutdown —
    /// is the only shape where subsystem lifetimes don't depend on the order locals happen
    /// to be declared in.
    /// </para>
    /// </summary>
    internal static class SdlHost {
        public const uint InitAudio = 0x00000010;
        public const uint InitVideo = 0x00000020;
        public const uint InitEvents = 0x00004000;

        private static Sdl? _api;
        private static uint _initialized;

        public static Sdl Api => _api ??= Sdl.GetApi();

        /// <summary>Initializes the requested subsystems. Already-initialized ones are skipped.</summary>
        public static void Init(uint subsystems) {
            uint missing = subsystems & ~_initialized;
            if (missing == 0) return;

            // SDL_InitSubSystem wants the base library up first; Init(0) does exactly that.
            if (_initialized == 0 && Api.Init(0) != 0)
                throw new InvalidOperationException($"SDL_Init failed: {Api.GetErrorS()}");

            if (Api.InitSubSystem(missing) != 0)
                throw new InvalidOperationException(
                    $"SDL_InitSubSystem(0x{missing:X}) failed: {Api.GetErrorS()}");

            _initialized |= missing;
        }

        /// <summary>Shuts down just the named subsystems, leaving the rest of SDL running.</summary>
        public static void QuitSubSystem(uint subsystems) {
            uint active = subsystems & _initialized;
            if (active == 0) return;
            Api.QuitSubSystem(active);
            _initialized &= ~active;
        }

        /// <summary>Tears down SDL entirely. Safe to call more than once.</summary>
        public static void Shutdown() {
            if (_api is null) return;
            _api.Quit();
            _api.Dispose();
            _api = null;
            _initialized = 0;
        }
    }
}
