using System;
using FloydPink.Flickr.Downloadr.UI.Helpers;

namespace FloydPink.Flickr.Downloadr
{
	public partial class AboutWindow : Gtk.Window
	{
		public AboutWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			Title += VersionHelper.GetVersionString();
			labelVersion.LabelProp += VersionHelper.GetVersionString ();
		}

		protected void buttonCloseClick (object sender, EventArgs e)
		{
			this.Destroy ();
		}
	}
}

