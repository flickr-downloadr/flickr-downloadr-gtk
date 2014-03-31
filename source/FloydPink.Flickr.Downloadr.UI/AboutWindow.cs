using System;
using System.Diagnostics;
using Gtk;
using FloydPink.Flickr.Downloadr.UI.Helpers;

namespace FloydPink.Flickr.Downloadr
{
	public partial class AboutWindow : Gtk.Window
	{
		public AboutWindow () : 
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			Title += VersionHelper.GetVersionString ();
			labelVersion.LabelProp = string.Format ("<big><big>flickr downloadr {0}</big></big>",
				VersionHelper.GetVersionString ());
		}

		protected void buttonCloseClick (object sender, EventArgs e)
		{
			this.Destroy ();
		}

		protected void eventboxHyperlinkClicked (object o, ButtonPressEventArgs args)
		{
			Process.Start (VersionHelper.GetAboutUrl ());
		}
	}
}

