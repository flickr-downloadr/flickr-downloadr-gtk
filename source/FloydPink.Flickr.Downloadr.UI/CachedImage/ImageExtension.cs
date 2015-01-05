namespace FloydPink.Flickr.Downloadr {
    using System.IO;
    using Gdk;
    using UI.CachedImage;
    using Image = Gtk.Image;

    public static class ImageExtension {
        public static void SetCachedImage(this Image self, string imageUrl) {
            using (var file = new FileStream(FileCache.FromUrl(imageUrl), FileMode.Open)) {
                self.Pixbuf = new Pixbuf(file);
            }
        }
    }
}
