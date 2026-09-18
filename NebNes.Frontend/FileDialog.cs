using System.Runtime.InteropServices;

namespace NebNes.Frontend {
    /// <summary>Thin wrapper over the Win32 common "open file" dialog (comdlg32). Windows-only.</summary>
    internal static class FileDialog {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct OpenFileName {
            public int lStructSize;
            public IntPtr hwndOwner;
            public IntPtr hInstance;
            public string? lpstrFilter;
            public string? lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile;
            public string? lpstrFileTitle;
            public int nMaxFileTitle;
            public string? lpstrInitialDir;
            public string? lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string? lpstrDefExt;
            public IntPtr lCustData;
            public IntPtr lpfnHook;
            public string? lpTemplateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int FlagsEx;
        }

        [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool GetOpenFileNameW(ref OpenFileName ofn);

        private const int OFN_PATHMUSTEXIST = 0x00000800;
        private const int OFN_FILEMUSTEXIST = 0x00001000;
        private const int OFN_NOCHANGEDIR = 0x00000008;
        // comdlg32 accepts a buffer well past MAX_PATH; 260 silently rejected long paths.
        private const int MaxPath = 32768;

        /// <summary>Shows the dialog and returns the chosen path, or null if the user cancelled.</summary>
        public static string? OpenRom(IntPtr owner) {
            if (!OperatingSystem.IsWindows())
                throw new PlatformNotSupportedException(
                    "The built-in file browser is Windows-only; drag a .nes file onto the window instead.");

            IntPtr buffer = Marshal.AllocHGlobal(MaxPath * sizeof(char));
            try {
                Marshal.WriteInt16(buffer, 0, 0); // empty initial file name

                var ofn = new OpenFileName {
                    lStructSize = Marshal.SizeOf<OpenFileName>(),
                    hwndOwner = owner,
                    // Filter fields are pairs of NUL-terminated strings, list terminated by an extra NUL.
                    lpstrFilter = "NES ROMs\0*.nes\0All Files\0*.*\0\0",
                    lpstrFile = buffer,
                    nMaxFile = MaxPath,
                    lpstrTitle = "Open NES ROM",
                    Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST | OFN_NOCHANGEDIR,
                };

                return GetOpenFileNameW(ref ofn) ? Marshal.PtrToStringUni(buffer) : null;
            } finally {
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
}
