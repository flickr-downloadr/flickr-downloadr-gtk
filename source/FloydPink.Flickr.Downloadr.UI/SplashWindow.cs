using System;

namespace FloydPink.Flickr.Downloadr
{
	public partial class SplashWindow : Gtk.Window
	{
		public SplashWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

