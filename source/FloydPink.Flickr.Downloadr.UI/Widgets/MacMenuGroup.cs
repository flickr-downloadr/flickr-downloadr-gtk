using System;
using System.Runtime.InteropServices;

namespace FloydPink.Flickr.Downloadr.UI.Widgets
{
	public class MacMenuGroup : GLib.Opaque
	{
		[DllImport("libigemacintegration.dylib")]
		static extern void ige_mac_menu_add_app_menu_item(IntPtr raw, IntPtr menu_item, IntPtr label);

		public void AddMenuItem(Gtk.MenuItem menu_item, string label) {
			IntPtr native_label = GLib.Marshaller.StringToPtrGStrdup (label);
			ige_mac_menu_add_app_menu_item(Handle, menu_item == null ? IntPtr.Zero : menu_item.Handle, native_label);
			GLib.Marshaller.Free (native_label);
		}

		public MacMenuGroup(IntPtr raw) : base(raw) {}	
	}
}

