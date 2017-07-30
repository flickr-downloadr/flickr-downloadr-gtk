using System;
using System.IO;
using Gdk;
using Image = Gtk.Image;

namespace FloydPink.Flickr.Downloadr.UI.CachedImage
{
  public static class ImageExtension
  {
    public static void SetCachedImage(this Image self, string imageUrl)
    {

      int maxRetries = 2;
      bool loaded = false;

      for (int count = 1; !loaded; count++)
      {
        using (var file = new FileStream(FileCache.FromUrl(imageUrl), FileMode.Open))
        {
          try
          {
            self.Pixbuf = new Pixbuf(file);
            loaded = true;
          }
          catch (Exception e)
          {

            //possible corruption in a cached file, for example an partial aborted file.

            if (count == maxRetries)
            {
              throw e;

            }
            else
            {
              file.Close();
              HandeException(imageUrl, e);
            }
          }
        }


      }
    }

    private static void HandeException(string imageUrl, Exception e)
    {
      string corruptedFilename = FileCache.GetLocalFilename(new Uri(imageUrl));

      if (File.Exists(corruptedFilename))
      {
        File.Delete(corruptedFilename);
      }
    }
  }
}
