using System;
using System.Runtime.InteropServices;

namespace FloydPink.Flickr.Downloadr
{
	public class MacMenu
	{
		[DllImport("libigemacintegration.dylib")]
		static extern void ige_mac_menu_connect_window_key_handler (IntPtr window);

		public static void ConnectWindowKeyHandler (Gtk.Window window)
		{
			ige_mac_menu_connect_window_key_handler (window.Handle);
		}

		[DllImport("libigemacintegration.dylib")]
		static extern void ige_mac_menu_set_global_key_handler_enabled (bool enabled);

		public static bool GlobalKeyHandlerEnabled {
			set { 
				ige_mac_menu_set_global_key_handler_enabled (value);
			}
		}

		[DllImport("libigemacintegration.dylib")]
		static extern void ige_mac_menu_set_menu_bar(IntPtr menu_shell);

		public static Gtk.MenuShell MenuBar { 
			set {
				ige_mac_menu_set_menu_bar(value == null ? IntPtr.Zero : value.Handle);
			}
		}

		[DllImport("libigemacintegration.dylib")]
		static extern void ige_mac_menu_set_quit_menu_item(IntPtr quit_item);

		public static Gtk.MenuItem QuitMenuItem { 
			set {
				ige_mac_menu_set_quit_menu_item(value == null ? IntPtr.Zero : value.Handle);
			}
		}

		[DllImport("libigemacintegration.dylib")]
		static extern IntPtr ige_mac_menu_add_app_menu_group();

		public static MacMenuGroup AddAppMenuGroup() {
			IntPtr raw_ret = ige_mac_menu_add_app_menu_group();
			MacMenuGroup ret = raw_ret == IntPtr.Zero ? null : (MacMenuGroup) GLib.Opaque.GetOpaque (raw_ret, typeof (MacMenuGroup), false);
			return ret;
		}
	}
}

