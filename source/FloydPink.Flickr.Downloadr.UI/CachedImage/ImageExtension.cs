using System;
using System.IO;
using FloydPink.Flickr.Downloadr.UI.CachedImage;

namespace FloydPink.Flickr.Downloadr
{
	public static class ImageExtension
	{
		public static void SetCachedImage(this Gtk.Image self, string imageUrl) {
			using (FileStream file = new FileStream (FileCache.FromUrl (imageUrl), FileMode.Open)) {
				self.Pixbuf = new Gdk.Pixbuf (file);
			}
		}
	}
}
