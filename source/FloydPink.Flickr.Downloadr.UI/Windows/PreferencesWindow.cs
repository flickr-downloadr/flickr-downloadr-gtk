namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using System;
    using System.Collections.Generic;
    using Bootstrap;
    using CachedImage;
    using Gtk;
    using Helpers;
    using Model;
    using Model.Enums;
    using Presentation;
    using Presentation.Views;

    public partial class PreferencesWindow : BaseWindow, IPreferencesView {
        private readonly IPreferencesPresenter _presenter;
        private Preferences _preferences;

        public PreferencesWindow(Session session) {
            Log.Debug("ctor");
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            Preferences = session.Preferences;
            User = session.User;

            this._presenter = Bootstrapper.GetPresenter<IPreferencesView, IPreferencesPresenter>(this);

            SetCacheSize();
        }

        private User User { get; set; }

        public Preferences Preferences
        {
            get { return this._preferences; }
            set
            {
                this._preferences = value;
                setFieldsFromModel(Preferences);
            }
        }

        public override void ShowSpinner(bool show) {
            //			Visibility visibility = show ? Visibility.Visible : Visibility.Collapsed;
            //			Spinner.Dispatch(s => s.Visibility = visibility);
        }

        private void OnDeleteEvent(object sender, DeleteEventArgs args) {
            Log.Debug("OnDeleteEvent");
            MainClass.Quit();
            args.RetVal = true;
        }

        private void AddTooltips() {
            Log.Debug("AddTooltips");
            this.labelFilename.TooltipText = this.radioPhotoId.TooltipText = this.radioPhotoTitle.TooltipText =
                "Choose to name the downloaded photos with its internal photo id (a unique number) or its title (Untitled images will be assigned random unique names)";
            this.labelDownloadLocation.TooltipText = this.entryDownloadLocation.TooltipText = this.buttonDownloadLocation.TooltipText =
                "Set the location to save the downloaded photos and metadata";
            this.labelDownloadSize.TooltipText = this.comboboxDownloadSize.TooltipText =
                "Set the size of the photos to download";
            this.labelMetadata.TooltipText =
                this.checkbuttonTags.TooltipText = this.checkbuttonDescription.TooltipText = this.checkbuttonTitle.TooltipText =
                    "Select the attributes of the metadata to be downloaded";
            this.labelPhotosPerPage.TooltipText = this.comboboxPhotosPerPage.TooltipText =
                "Set the number of photos to be displayed in a page on the browser window";
            this.labelSafetyLevel.TooltipText = this.comboboxSafetyLevel.TooltipText =
                "Set the safety level of the photos to be downloaded";
            this.labelTags.TooltipText = this.radioTagsInternal.TooltipText = this.radioTagsOriginal.TooltipText =
                "Choose the type of tags to be downloaded - internal tags does not preserve the space, original will be exactly as it were entered";
            this.labelCacheLocation.TooltipText = this.entryCacheLocation.TooltipText = this.buttonCacheLocation.TooltipText =
                "Set the location to save the cached copy of the thumbnails and preview images";
            this.labelCacheSize.TooltipText = this.labelCacheSizeValue.TooltipText =
                "Amount of space taken by the current cache folder";
            this.buttonEmptyCache.TooltipText = "Empty the cache folder if it is taking up too much space";
            this.buttonCancel.TooltipText =
                "Revert all the settings to their last saved values and go back to the login window";
            this.buttonDefaults.TooltipText = "Reset all the settings to their default values";
            this.buttonSave.TooltipText = "Save all the settings and continue to the photoset selection window";
            this.labelUpdate.TooltipText = this.checkbuttonUpdate.TooltipText =
                "Get notified automatically when there is an updated version available";
            this.labelLogLevel.TooltipText = this.comboboxLogLevel.TooltipText =
                "Set the level of diagnostic logging (Recommended: Off)";
            this.labelLogLocation.TooltipText = this.entryLogLocation.TooltipText = this.buttonLogLocation.TooltipText =
                "Set the location to save the log files with diagnostic information";
            this.labelRestartRequired.TooltipText = "The settings on the fields with red asterisk above, will work better after a restart";
        }

        private void SetCacheSize() {
            Log.Debug("SetCacheSize");
            this.labelCacheSizeValue.Text = this._presenter.GetCacheFolderSize(Preferences.CacheLocation);
            this.buttonEmptyCache.Visible =
                !(this.labelCacheSizeValue.Text == "0 B" || this.labelCacheSizeValue.Text == "-");
        }

        private void setFieldsFromModel(Preferences preferences) {
            Log.Debug("setFieldsFromModel");
            // Filename
            this.radioPhotoId.Active = !preferences.TitleAsFilename;
            this.radioPhotoTitle.Active = preferences.TitleAsFilename;

            // Download location
            this.entryDownloadLocation.Text = preferences.DownloadLocation;

            // Download size
            this.comboboxDownloadSize.Active = (int) preferences.DownloadSize;

            // Metadata
            this.checkbuttonTags.Active = preferences.Metadata.Contains(PhotoMetadata.Tags);
            this.checkbuttonDescription.Active = preferences.Metadata.Contains(PhotoMetadata.Description);
            this.checkbuttonTitle.Active = preferences.Metadata.Contains(PhotoMetadata.Title);

            // Photos per page
            var photosPerPageMap = new Dictionary<string, int> {
                {
                    "25", 0
                }, {
                    "50", 1
                }, {
                    "75", 2
                }, {
                    "100", 3
                }
            };
            this.comboboxPhotosPerPage.Active = photosPerPageMap[preferences.PhotosPerPage.ToString()];

            //Safety level
            this.comboboxSafetyLevel.Active = int.Parse(preferences.SafetyLevel) - 1;

            // Tags
            this.radioTagsInternal.Active = !preferences.NeedOriginalTags;
            this.radioTagsOriginal.Active = preferences.NeedOriginalTags;

            // Cache location
            this.entryCacheLocation.Text = preferences.CacheLocation;

            // Check for Update
            this.checkbuttonUpdate.Active = preferences.CheckForUpdates;

            // Log Level
            this.comboboxLogLevel.Active = (int) preferences.LogLevel;

            // Log location
            this.entryLogLocation.Text = preferences.LogLocation;
        }

        private Preferences getModelFromFields() {
            Log.Debug("getModelFromFields");
            var metadata = new List<string>();
            if (this.checkbuttonTags.Active) {
                metadata.Add(PhotoMetadata.Tags);
            }
            if (this.checkbuttonDescription.Active) {
                metadata.Add(PhotoMetadata.Description);
            }
            if (this.checkbuttonTitle.Active) {
                metadata.Add(PhotoMetadata.Title);
            }
            return new Preferences {
                TitleAsFilename = this.radioPhotoTitle.Active,
                DownloadLocation = this.entryDownloadLocation.Text,
                DownloadSize =
                    (PhotoDownloadSize) Enum.Parse(typeof (PhotoDownloadSize), this.comboboxDownloadSize.ActiveText),
                Metadata = metadata,
                PhotosPerPage = int.Parse(this.comboboxPhotosPerPage.ActiveText),
                SafetyLevel = (this.comboboxSafetyLevel.Active + 1).ToString(),
                NeedOriginalTags = this.radioTagsOriginal.Active,
                CacheLocation = this.entryCacheLocation.Text,
                CheckForUpdates = this.checkbuttonUpdate.Active,
                LogLevel =
                    (LogLevel) Enum.Parse(typeof (LogLevel), this.comboboxLogLevel.ActiveText),
                LogLocation = this.entryLogLocation.Text
            };
        }

        private void buttonCancelClick(object sender, EventArgs e) {
            Log.Debug("buttonCancelClick");
            var loginWindow = new LoginWindow(User);
            loginWindow.Show();
            Destroy();
        }

        private void buttonDefaultsClick(object sender, EventArgs e) {
            Log.Debug("buttonDefaultsClick");
            Preferences = Preferences.GetDefault();
        }

        private void buttonSaveClick(object sender, EventArgs e) {
            Log.Debug("buttonSaveClick");
            var preferences = getModelFromFields();

            FileCache.AppCacheDirectory = preferences.CacheLocation;

            Bootstrapper.ReconfigureLogging(preferences.LogLevel.ToString(), preferences.LogLocation);

            this._presenter.Save(preferences);
            var landingWindow = new LandingWindow(new Session(User, preferences));
            landingWindow.Show();
            Destroy();
        }

        // TODO: Refactor the below three event handlers that open a select folder dialog, to avoid duplication!
        private void buttonDownloadLocationClick(object sender, EventArgs e) {
            Log.Debug("buttonDownloadLocationClick");
            // Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
            var dialog = new FileChooserDialog("Select folder to save downloaded photos:",
                null, FileChooserAction.SelectFolder);

            var preferences = getModelFromFields();

            dialog.SetCurrentFolder(preferences.DownloadLocation);

            dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
            dialog.AddButton(Stock.Ok, ResponseType.Ok);

            var result = (ResponseType) dialog.Run();
            if (result == ResponseType.Ok) {
                preferences.DownloadLocation = dialog.CurrentFolder;
                setFieldsFromModel(preferences);
            }

            dialog.Destroy();
        }

        private void buttonCacheLocationClick(object sender, EventArgs e) {
            Log.Debug("buttonCacheLocationClick");
            // Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
            var dialog = new FileChooserDialog("Select folder to save the cached thumbnails:",
                null, FileChooserAction.SelectFolder);

            var preferences = getModelFromFields();

            dialog.SetCurrentFolder(preferences.CacheLocation);

            dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
            dialog.AddButton(Stock.Ok, ResponseType.Ok);

            var result = (ResponseType) dialog.Run();
            if (result == ResponseType.Ok) {
                Preferences.CacheLocation = preferences.CacheLocation = dialog.CurrentFolder;
                setFieldsFromModel(preferences);
                SetCacheSize();
            }

            dialog.Destroy();
        }

        private void buttonEmptyCacheClick(object sender, EventArgs e) {
            Log.Debug("buttonEmptyCacheClick");
            var result = MessageBox.Show(this, "Are you sure you want to empty the cache folder?",
                ButtonsType.YesNo, MessageType.Question);
            if (result == ResponseType.Yes) {
                this._presenter.EmptyCacheDirectory(Preferences.CacheLocation);
                SetCacheSize();
            }
        }

        private void buttonLogLocationClick(object sender, EventArgs e) {
            Log.Debug("buttonLogLocationClick");
            // Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
            var dialog = new FileChooserDialog("Select folder to save the log files:",
                null, FileChooserAction.SelectFolder);

            var preferences = getModelFromFields();

            dialog.SetCurrentFolder(preferences.LogLocation);

            dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
            dialog.AddButton(Stock.Ok, ResponseType.Ok);

            var result = (ResponseType) dialog.Run();
            if (result == ResponseType.Ok) {
                Preferences.LogLocation = preferences.LogLocation = dialog.CurrentFolder;
                setFieldsFromModel(preferences);
            }

            dialog.Destroy();
        }
    }
}
