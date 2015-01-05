namespace FloydPink.Flickr.Downloadr.UI.Helpers {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public static class PlatformDetection {
        public static readonly bool IsWindows;
        public static readonly bool IsMac;

        static PlatformDetection() {
            IsWindows = Path.DirectorySeparatorChar == '\\';
            IsMac = !IsWindows && IsRunningOnMac();
        }

        //From Managed.Windows.Forms/XplatUI
        private static bool IsRunningOnMac() {
            var buf = IntPtr.Zero;
            try {
                buf = Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname ()
                if (uname(buf) == 0) {
                    var os = Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin") {
                        return true;
                    }
                }
            }
            catch { }
            finally {
                if (buf != IntPtr.Zero) {
                    Marshal.FreeHGlobal(buf);
                }
            }
            return false;
        }

        [DllImport("libc")]
        private static extern int uname(IntPtr buf);
    }
}
