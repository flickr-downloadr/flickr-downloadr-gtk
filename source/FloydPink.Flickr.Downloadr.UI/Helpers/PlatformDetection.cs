using System;
using System.IO;

namespace FloydPink.Flickr.Downloadr.UI.Helpers
{
	public static class PlatformDetection
	{
		public readonly static bool IsWindows;
		public readonly static bool IsMac;

		static PlatformDetection ()
		{
			IsWindows = Path.DirectorySeparatorChar == '\\';
			IsMac = !IsWindows && IsRunningOnMac();
		}

		//From Managed.Windows.Forms/XplatUI
		static bool IsRunningOnMac ()
		{
			IntPtr buf = IntPtr.Zero;
			try {
				buf = System.Runtime.InteropServices.Marshal.AllocHGlobal (8192);
				// This is a hacktastic way of getting sysname from uname ()
				if (uname (buf) == 0) {
					string os = System.Runtime.InteropServices.Marshal.PtrToStringAnsi (buf);
					if (os == "Darwin")
						return true;
				}
			} catch {
			} finally {
				if (buf != IntPtr.Zero)
					System.Runtime.InteropServices.Marshal.FreeHGlobal (buf);
			}
			return false;
		}

		[System.Runtime.InteropServices.DllImport ("libc")]
		static extern int uname (IntPtr buf);
	}
}
