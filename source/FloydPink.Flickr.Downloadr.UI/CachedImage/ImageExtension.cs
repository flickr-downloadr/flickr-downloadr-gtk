using System.IO;
using FloydPink.Flickr.Downloadr.UI.CachedImage;
using Gdk;
using Image = Gtk.Image;

namespace FloydPink.Flickr.Downloadr {
    public static class ImageExtension {
        public static void SetCachedImage(this Image self, string imageUrl) {
            using (var file = new FileStream(FileCache.FromUrl(imageUrl), FileMode.Open)) {
                self.Pixbuf = new Pixbuf(file);
            }
        }
    }
}
