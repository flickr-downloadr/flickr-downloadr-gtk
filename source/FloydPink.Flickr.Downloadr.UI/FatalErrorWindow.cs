using System;
using FloydPink.Flickr.Downloadr.UI.Helpers;

namespace FloydPink.Flickr.Downloadr
{
	public partial class FatalErrorWindow : Gtk.Window
	{
		public FatalErrorWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			Title += VersionHelper.GetVersionString ();
		}
	}
}

