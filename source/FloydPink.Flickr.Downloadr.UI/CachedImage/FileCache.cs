namespace FloydPink.Flickr.Downloadr.UI.CachedImage {
    using System;
    using System.Diagnostics;
    using System.IO;

    public class FileCache {
        static FileCache() {
            // default cache directory - can be changed if needed from App.xaml
            AppCacheDirectory = string.Format("{0}/{1}/Cache/",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Process.GetCurrentProcess().ProcessName);
        }

        /// <summary>
        ///     Gets or sets the path to the folder that stores the filesystem-based cache
        /// </summary>
        public static string AppCacheDirectory { get; set; }

        public static string FromUrl(string url) {
            // Check to see if the cache directory has been created
            if (!Directory.Exists(AppCacheDirectory)) {
                // create it
                Directory.CreateDirectory(AppCacheDirectory);
            }

            // Cast the string into a Uri so we can access the image name without regex
            var uri = new Uri(url);
            var localFile = string.Format("{0}/{1}", AppCacheDirectory, uri.Segments[uri.Segments.Length - 1]);

            if (!File.Exists(localFile)) {
                HttpHelper.GetAndSaveToFile(url, localFile);
            }

            // The full path of the image on the local computer
            return localFile;
        }
    }
}
