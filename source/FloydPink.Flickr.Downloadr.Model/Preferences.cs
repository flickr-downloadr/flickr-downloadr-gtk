using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Model.Extensions;

namespace FloydPink.Flickr.Downloadr.Model {
    public class Preferences : INotifyPropertyChanged {
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

        public bool TitleAsFilename {
            get { return _titleAsFilename; }
            set {
                _titleAsFilename = value;
                PropertyChanged.Notify(() => TitleAsFilename);
            }
        }

        public string DownloadLocation {
            get { return _downloadLocation; }
            set {
                _downloadLocation = value;
                PropertyChanged.Notify(() => DownloadLocation);
            }
        }

        public PhotoDownloadSize DownloadSize {
            get { return _downloadSize; }
            set {
                _downloadSize = value;
                PropertyChanged.Notify(() => DownloadSize);
            }
        }

        public int PhotosPerPage {
            get { return _photosPerPage; }
            set {
                _photosPerPage = value;
                PropertyChanged.Notify(() => PhotosPerPage);
            }
        }

        public string SafetyLevel {
            get { return _safetyLevel; }
            set {
                _safetyLevel = value;
                PropertyChanged.Notify(() => SafetyLevel);
            }
        }

        public List<string> Metadata { get; set; }

        public bool NeedOriginalTags {
            get { return _needOriginalTags; }
            set {
                _needOriginalTags = value;
                PropertyChanged.Notify(() => NeedOriginalTags);
            }
        }

        public string CacheLocation {
            get { return _cacheLocation; }
            set {
                _cacheLocation = value;
                PropertyChanged.Notify(() => CacheLocation);
            }
        }

        public bool CheckForUpdates {
            get { return _checkForUpdates; }
            set {
                _checkForUpdates = value;
                PropertyChanged.Notify(() => CheckForUpdates);
            }
        }

        public LogLevel LogLevel {
            get { return _logLevel; }
            set {
                _logLevel = value;
                PropertyChanged.Notify(() => LogLevel);
            }
        }

        public string LogLocation {
            get { return _logLocation; }
            set {
                _logLocation = value;
                PropertyChanged.Notify(() => LogLocation);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
                LogLevel = LogLevel.Off,
                LogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "flickr-downloadr", "Logs")
            };
        }
    }
}
