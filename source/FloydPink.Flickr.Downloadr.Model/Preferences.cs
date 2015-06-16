namespace FloydPink.Flickr.Downloadr.Model {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Enums;

    public class Preferences {
        public bool TitleAsFilename { get; set; }
        public string DownloadLocation { get; set; }
        public PhotoDownloadSize DownloadSize { get; set; }
        public int PhotosPerPage { get; set; }
        public string SafetyLevel { get; set; }
        public List<string> Metadata { get; set; }
        public bool NeedOriginalTags { get; set; }
        public string CacheLocation { get; set; }
        public bool CheckForUpdates { get; set; }
        public LogLevel LogLevel { get; set; }
        public string LogLocation { get; set; }

        public static Preferences GetDefault() {
            return new Preferences {
                TitleAsFilename = false,
                PhotosPerPage = 25,
                DownloadLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                Metadata =
                    new List<string> {
                        PhotoMetadata.Title,
                        PhotoMetadata.Description,
                        PhotoMetadata.Tags
                    },
                DownloadSize = PhotoDownloadSize.Original,
                SafetyLevel = SafeSearch.Safe,
                NeedOriginalTags = false,
                CacheLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "flickr-downloadr", "Cache"),
                CheckForUpdates = true,
                LogLevel = LogLevel.Info,
                LogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "flickr-downloadr", "Logs")
            };
        }
    }
}
