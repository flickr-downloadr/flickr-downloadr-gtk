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
        private bool _needOriginalTags;
        private int _photosPerPage;
        private string _safetyLevel;
        private bool _titleAsFilename;

        public bool TitleAsFilename {
            get { return this._titleAsFilename; }
            set {
                this._titleAsFilename = value;
                PropertyChanged.Notify(() => TitleAsFilename);
            }
        }

        public string DownloadLocation {
            get { return this._downloadLocation; }
            set {
                this._downloadLocation = value;
                PropertyChanged.Notify(() => DownloadLocation);
            }
        }

        public PhotoDownloadSize DownloadSize {
            get { return this._downloadSize; }
            set {
                this._downloadSize = value;
                PropertyChanged.Notify(() => DownloadSize);
            }
        }

        public int PhotosPerPage {
            get { return this._photosPerPage; }
            set {
                this._photosPerPage = value;
                PropertyChanged.Notify(() => PhotosPerPage);
            }
        }

        public string SafetyLevel {
            get { return this._safetyLevel; }
            set {
                this._safetyLevel = value;
                PropertyChanged.Notify(() => SafetyLevel);
            }
        }

        public List<string> Metadata { get; set; }

        public bool NeedOriginalTags {
            get { return this._needOriginalTags; }
            set {
                this._needOriginalTags = value;
                PropertyChanged.Notify(() => NeedOriginalTags);
            }
        }

        public string CacheLocation {
            get { return this._cacheLocation; }
            set {
                this._cacheLocation = value;
                PropertyChanged.Notify(() => CacheLocation);
            }
        }

        public bool CheckForUpdates {
            get { return this._checkForUpdates; }
            set {
                this._checkForUpdates = value;
                PropertyChanged.Notify(() => CheckForUpdates);
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
                CheckForUpdates = true
            };
        }
    }
}
