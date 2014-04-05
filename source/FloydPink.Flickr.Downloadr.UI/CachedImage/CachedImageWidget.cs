using System;
using FloydPink.Flickr.Downloadr.UI.CachedImage;
using System.IO;

namespace FloydPink.Flickr.Downloadr
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class CachedImageWidget : Gtk.Bin
	{
		string _imageUrl;
		public string ImageUrl {
			get {
				return _imageUrl;
			}
			set {
				_imageUrl = value;
				using (FileStream file = new FileStream (FileCache.FromUrl (value), FileMode.Open)) {
					imageMain.Pixbuf = new Gdk.Pixbuf (file);
				}
			}
		}

		public CachedImageWidget ()
		{
			this.Build ();
		}
	}
}

