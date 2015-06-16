namespace FloydPink.Flickr.Downloadr.Model {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using Enums;

    public class Preferences {
        private string _cacheLocation;
        private bool _checkForUpdates;
        private string _downloadLocation;
        private PhotoDownloadSize _downloadSize;
        private LogLevel _logLevel;
        private string _logLocation;
        private bool _needOriginalTags;
        private int _photosPerPage;
        private string _safetyLevel;
        private bool _titleAsFilename;

        public bool TitleAsFilename
        {
            get { return this._titleAsFilename; }
            set
            {
                this._titleAsFilename = value;
            }
        }

        public string DownloadLocation
        {
            get { return this._downloadLocation; }
            set
            {
                this._downloadLocation = value;
            }
        }

        public PhotoDownloadSize DownloadSize
        {
            get { return this._downloadSize; }
            set
            {
                this._downloadSize = value;
            }
        }

        public int PhotosPerPage
        {
            get { return this._photosPerPage; }
            set
            {
                this._photosPerPage = value;
            }
        }

        public string SafetyLevel
        {
            get { return this._safetyLevel; }
            set
            {
                this._safetyLevel = value;
            }
        }

        public List<string> Metadata { get; set; }

        public bool NeedOriginalTags
        {
            get { return this._needOriginalTags; }
            set
            {
                this._needOriginalTags = value;
            }
        }

        public string CacheLocation
        {
            get { return this._cacheLocation; }
            set
            {
                this._cacheLocation = value;
            }
        }

        public bool CheckForUpdates
        {
            get { return this._checkForUpdates; }
            set
            {
                this._checkForUpdates = value;
            }
        }

        public LogLevel LogLevel
        {
            get { return this._logLevel; }
            set
            {
                this._logLevel = value;
            }
        }

        public string LogLocation
        {
            get { return this._logLocation; }
            set
            {
                this._logLocation = value;
            }
        }

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
