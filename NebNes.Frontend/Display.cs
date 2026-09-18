using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.SDL;

namespace NebNes.Frontend {
    internal sealed unsafe class Display : IDisposable {
        public const int NesWidth = 256;
        public const int NesHeight = 240;

        private const uint PIXELFORMAT_ARGB8888 = 0x16362004;
        private const uint Subsystems = SdlHost.InitVideo | SdlHost.InitEvents;

        private readonly Sdl _sdl;
        private Window* _window;
        private Renderer* _renderer;
        private Texture* _texture;

        // KMOD_LCTRL | KMOD_RCTRL — the Ctrl bits within Keysym.Mod.
        private const ushort KMOD_CTRL = 0x0040 | 0x0080;

        /// <summary>Raised on key press (true) and release (false). May repeat while held; setting state is idempotent.</summary>
        public Action<Scancode, bool>? KeyChanged;

        /// <summary>Raised when a file is dropped onto the window, with its path.</summary>
        public Action<string>? FileDropped;

        /// <summary>Raised when the user asks to open a ROM (Ctrl+O).</summary>
        public Action? OpenRomRequested;

        /// <summary>Raised when the user asks to reset the console (R, without Ctrl).</summary>
        public Action? ResetRequested;

        /// <summary>Raised when the user asks to record a CPU trace (T, without Ctrl).</summary>
        public Action? TraceRequested;

        public Display(string title, int scale) {
            SdlHost.Init(Subsystems);
            _sdl = SdlHost.Api;

            _sdl.SetHint("SDL_RENDER_SCALE_QUALITY", "0");

            _window = _sdl.CreateWindow(
                title,
                Sdl.WindowposCentered, Sdl.WindowposCentered,
                NesWidth * scale, NesHeight * scale,
                (uint)(WindowFlags.Shown | WindowFlags.Resizable));
            if (_window == null)
                throw new InvalidOperationException($"SDL_CreateWindow failed: {_sdl.GetErrorS()}");

            // Everything past the window has to unwind by hand: a throw from here leaves the
            // caller's `using` un-entered, so nothing else would ever destroy the window.
            try {
                // No vsync: audio is the master clock (the main loop paces the emulator by how
                // many samples SDL still needs). Blocking on vsync would throttle the producer to
                // ~1x real-time, so the audio queue could never build a cushion and would underrun.
                _renderer = _sdl.CreateRenderer(_window, -1,
                    (uint)RendererFlags.Accelerated);
                if (_renderer == null)
                    throw new InvalidOperationException($"SDL_CreateRenderer failed: {_sdl.GetErrorS()}");

                _sdl.RenderSetLogicalSize(_renderer, NesWidth, NesHeight);
                _sdl.RenderSetIntegerScale(_renderer, SdlBool.True);

                _texture = _sdl.CreateTexture(_renderer, PIXELFORMAT_ARGB8888,
                    (int)TextureAccess.Streaming, NesWidth, NesHeight);
                if (_texture == null)
                    throw new InvalidOperationException($"SDL_CreateTexture failed: {_sdl.GetErrorS()}");
            } catch {
                Dispose();
                throw;
            }

            // Deliver SDL_DROPFILE events (enabled by default on most builds; make it explicit).
            _sdl.EventState((uint)EventType.Dropfile, 1);
        }

        public void SetTitle(string title) => _sdl.SetWindowTitle(_window, title);

        /// <summary>
        /// The native window handle (HWND on Windows), or <see cref="IntPtr.Zero"/> if SDL can't
        /// supply one. Used to parent modal dialogs so they can't open behind the emulator window.
        /// </summary>
        public IntPtr NativeHandle {
            get {
                if (_window == null) return IntPtr.Zero;

                SysWMInfo info = default;
                // SDL refuses the call unless the caller stamps the headers' version first.
                info.Version = new Silk.NET.SDL.Version {
                    Major = Sdl.MajorVersion, Minor = Sdl.MinorVersion, Patch = Sdl.Patchlevel,
                };
                if (!_sdl.GetWindowWMInfo(_window, &info)) return IntPtr.Zero;
                return info.Subsystem == SysWMType.Windows ? info.Info.Win.Hwnd : IntPtr.Zero;
            }
        }

        public void UpdateFrame(uint[] framebuffer) {
            if (framebuffer.Length != NesWidth * NesHeight)
                throw new ArgumentException(
                    $"Framebuffer must be {NesWidth * NesHeight} pixels, got {framebuffer.Length}.");

            fixed (uint* pixels = framebuffer) {
                _sdl.UpdateTexture(_texture, (Rectangle<int>*)null, pixels, NesWidth * sizeof(uint));
            }

            _sdl.RenderClear(_renderer);
            _sdl.RenderCopy(_renderer, _texture, (Rectangle<int>*)null, (Rectangle<int>*)null);
            _sdl.RenderPresent(_renderer);
        }

        public bool PumpEvents() {
            Event e = default;
            while (_sdl.PollEvent(ref e) != 0) {
                switch ((EventType)e.Type) {
                    case EventType.Quit:
                        return false;
                    case EventType.Keydown:
                        Scancode scancode = e.Key.Keysym.Scancode;
                        bool ctrl = (e.Key.Keysym.Mod & KMOD_CTRL) != 0;
                        if (scancode == Scancode.ScancodeEscape)
                            return false;
                        if (ctrl && scancode == Scancode.ScancodeO) {
                            OpenRomRequested?.Invoke();
                            break;
                        }
                        // Ctrl+R is a common "reload" reflex elsewhere; don't let it power-cycle.
                        if (scancode == Scancode.ScancodeR && !ctrl && e.Key.Repeat == 0) {
                            ResetRequested?.Invoke();
                            break;
                        }
                        if (scancode == Scancode.ScancodeT && !ctrl && e.Key.Repeat == 0) {
                            TraceRequested?.Invoke();
                            break;
                        }
                        KeyChanged?.Invoke(scancode, true);
                        break;
                    case EventType.Keyup:
                        KeyChanged?.Invoke(e.Key.Keysym.Scancode, false);
                        break;
                    case EventType.Dropfile:
                        string? path = Marshal.PtrToStringUTF8((nint)e.Drop.File);
                        _sdl.Free(e.Drop.File);
                        if (!string.IsNullOrEmpty(path))
                            FileDropped?.Invoke(path);
                        break;
                }
            }
            return true;
        }

        /// <summary>The display's refresh rate in Hz, or 0 if SDL can't report one.</summary>
        public int RefreshRate {
            get {
                DisplayMode mode = default;
                return _sdl.GetCurrentDisplayMode(0, ref mode) == 0 ? mode.RefreshRate : 0;
            }
        }

        public static int SuggestScale() {
            SdlHost.Init(SdlHost.InitVideo);

            Rectangle<int> bounds = default;
            if (SdlHost.Api.GetDisplayUsableBounds(0, ref bounds) != 0 || bounds.Size.Y <= 0)
                return 3;

            int byHeight = (int)(bounds.Size.Y * 0.9) / NesHeight;
            int byWidth = (int)(bounds.Size.X * 0.9) / NesWidth;
            return Math.Max(1, Math.Min(byHeight, byWidth));
        }

        public void Dispose() {
            if (_texture != null) { _sdl.DestroyTexture(_texture); _texture = null; }
            if (_renderer != null) { _sdl.DestroyRenderer(_renderer); _renderer = null; }
            if (_window != null) { _sdl.DestroyWindow(_window); _window = null; }
            SdlHost.QuitSubSystem(Subsystems);
        }
    }
}
