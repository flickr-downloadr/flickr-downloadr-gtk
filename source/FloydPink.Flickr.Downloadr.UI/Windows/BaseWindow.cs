using System;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.UI.Widgets;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Windows
{
	public class BaseWindow : Gtk.Window
	{
		public BaseWindow () : base (WindowType.Toplevel)
		{
			if (PlatformDetection.IsMac) {
				MenuItem menuItemQuit = new MenuItem ();
				menuItemQuit.ButtonPressEvent += (object o, ButtonPressEventArgs args) => {
					Application.Quit ();	
					args.RetVal = true;
				};

				MacMenu.GlobalKeyHandlerEnabled = true;

				MacMenu.QuitMenuItem = menuItemQuit;
			}
		}
	}
}

