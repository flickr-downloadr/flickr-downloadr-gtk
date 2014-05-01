using System;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using Gtk;

namespace FloydPink.Flickr.Downloadr
{
	public class BaseWindow : Gtk.Window
	{
		public BaseWindow () : base(WindowType.Toplevel)
		{
			if (PlatformDetection.IsMac) {

				MenuBar appMenuBar = new MenuBar ();

				MenuItem menuItemAbout = new MenuItem ();
				MenuItem menuItemPrefs = new MenuItem ();
				MenuItem menuItemQuit = new MenuItem (	);

				AccelGroup accelGroup = new AccelGroup ();
				AddAccelGroup (accelGroup);

				menuItemPrefs.AddAccelerator ("activate", accelGroup, new AccelKey (Gdk.Key.comma, Gdk.ModifierType.ControlMask, AccelFlags.Visible));

				menuItemQuit.ButtonPressEvent += (object o, ButtonPressEventArgs args) => {
					Application.Quit();
					args.RetVal = true;
				};

				appMenuBar.Append (menuItemAbout);
				appMenuBar.Append (menuItemPrefs);
				appMenuBar.Append (menuItemQuit);

				//enable the global key handler for keyboard shortcuts
				MacMenu.GlobalKeyHandlerEnabled = true;

				//Tell the IGE library to use your GTK menu as the Mac main menu
				MacMenu.MenuBar = appMenuBar;

				//tell IGE which menu item should be used for the app menu's quit item
				MacMenu.QuitMenuItem = menuItemQuit;

				//add a new group to the app menu, and add some items to it
				var appGroup = MacMenu.AddAppMenuGroup ();
				appGroup.AddMenuItem (menuItemAbout, "About flickr downloadr");
				appGroup.AddMenuItem (menuItemPrefs, "Preferences...");
				appGroup.AddMenuItem (menuItemQuit, "Quit");

			}
		}
	}
}

