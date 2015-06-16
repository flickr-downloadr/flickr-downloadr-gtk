namespace FloydPink.Flickr.Downloadr.Presentation {
    using System.IO;
    using System.Linq;
    using Logic.Interfaces;
    using Model;
    using Views;

    public class PreferencesPresenter : PresenterBase, IPreferencesPresenter {
        private readonly IPreferencesLogic _logic;

        public PreferencesPresenter(IPreferencesView view, IPreferencesLogic logic) {
            this._logic = logic;
        }

        public void Save(Preferences preferences) {
            this._logic.SavePreferences(preferences);
        }

        public string GetCacheFolderSize(string cacheLocation) {
            var folderSize = "-";
            if (Directory.Exists(cacheLocation)) {
                var cacheDirectory = new DirectoryInfo(cacheLocation);
                var bytes = cacheDirectory.EnumerateFiles("*", SearchOption.AllDirectories).Sum(x => x.Length);
                folderSize = GetBytesReadable(bytes);
            }
            return folderSize;
        }

        public void EmptyCacheDirectory(string cacheLocation) {
            this._logic.EmptyCacheDirectory(cacheLocation);
        }

        // Thank you, humbads - http://stackoverflow.com/a/11124118/218882
        // Returns the human-readable file size for an arbitrary, 64-bit file size
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        private static string GetBytesReadable(long bytes) {
            var sign = (bytes < 0 ? "-" : "");
            double readable = (bytes < 0 ? -bytes : bytes);
            string suffix;
            if (bytes >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = bytes >> 50;
            } else if (bytes >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = bytes >> 40;
            } else if (bytes >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = bytes >> 30;
            } else if (bytes >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = bytes >> 20;
            } else if (bytes >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = bytes >> 10;
            } else if (bytes >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = bytes;
            } else {
                return bytes.ToString(sign + "0 B"); // Byte
            }
            readable /= 1024;

            return sign + readable.ToString("0.## ") + suffix;
        }
    }
}
