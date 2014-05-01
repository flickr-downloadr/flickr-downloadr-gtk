using System;
using System.Collections.Generic;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using Gtk;

namespace FloydPink.Flickr.Downloadr {
	public partial class PreferencesWindow : BaseWindow, IPreferencesView {
        private readonly IPreferencesPresenter _presenter;
        private Preferences _preferences;

        public PreferencesWindow(User user, Preferences preferences) {
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            Preferences = preferences;
            User = user;

            this._presenter = Bootstrapper.GetPresenter<IPreferencesView, IPreferencesPresenter>(this);

            SetCacheSize();
        }

        protected User User { get; set; }

        public Preferences Preferences {
            get { return this._preferences; }
            set {
                this._preferences = value;
                setFieldsFromModel(Preferences);
            }
        }

        public void ShowSpinner(bool show) {
            //			Visibility visibility = show ? Visibility.Visible : Visibility.Collapsed;
            //			Spinner.Dispatch(s => s.Visibility = visibility);
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs args) {
            Application.Quit();
            args.RetVal = true;
        }

        private void AddTooltips() {
            this.labelFilename.TooltipText = this.radioPhotoId.TooltipText = this.radioPhotoTitle.TooltipText =
                "Choose to name the downloaded photos with its internal photo id (a unique number) or its title (Untitled images will be assigned random unique names)";
            this.labelDownloadLocation.TooltipText =
                this.entryDownloadLocation.TooltipText = this.buttonDownloadLocation.TooltipText =
                    "Set the location to save the downloaded photos and metadata";
            this.labelDownloadSize.TooltipText = this.comboboxDownloadSize.TooltipText =
                "Set the size of the photos to download";
            this.labelMetadata.TooltipText =
                this.checkbuttonTags.TooltipText =
                    this.checkbuttonDescription.TooltipText = this.checkbuttonTitle.TooltipText =
                        "Select the attributes of the metadata to be downloaded";
            this.labelPhotosPerPage.TooltipText = this.comboboxPhotosPerPage.TooltipText =
                "Set the number of photos to be displayed in a page on the browser window";
            this.labelSafetyLevel.TooltipText = this.comboboxSafetyLevel.TooltipText =
                "Set the safety level of the photos to be downloaded";
            this.labelTags.TooltipText = this.radioTagsInternal.TooltipText = this.radioTagsOriginal.TooltipText =
                "Choose the type of tags to be downloaded - internal tags does not preserve the space, original will be exactly as it were entered";
            this.labelCacheLocation.TooltipText =
                this.entryCacheLocation.TooltipText = this.buttonCacheLocation.TooltipText =
                    "Set the location to save the cached copy of the thumbnails and preview images";
            this.labelCacheSize.TooltipText = this.labelCacheSizeValue.TooltipText =
                "Amount of space taken by the current cache folder";
            this.buttonEmptyCache.TooltipText = "Empty the cache folder if it is taking up too much space";
            this.buttonCancel.TooltipText =
                "Revert all the settings to their last saved values and go back to the login window";
            this.buttonDefaults.TooltipText = "Reset all the settings to their default values";
            this.buttonSave.TooltipText = "Save all the settings and continue to the browser window";
        }

        private void SetCacheSize() {
            this.labelCacheSizeValue.Text = this._presenter.GetCacheFolderSize(Preferences.CacheLocation);
            this.buttonEmptyCache.Visible =
                !(this.labelCacheSizeValue.Text == "0 B" || this.labelCacheSizeValue.Text == "-");
        }

        private void setFieldsFromModel(Preferences preferences) {
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
                },
            };
            this.comboboxPhotosPerPage.Active = photosPerPageMap[preferences.PhotosPerPage.ToString()];

            //Safety level
            this.comboboxSafetyLevel.Active = int.Parse(preferences.SafetyLevel) - 1;

            // Tags
            this.radioTagsInternal.Active = !preferences.NeedOriginalTags;
            this.radioTagsOriginal.Active = preferences.NeedOriginalTags;

            // Cache location
            this.entryCacheLocation.Text = preferences.CacheLocation;
        }

        private Preferences getModelFromFields() {
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
                CacheLocation = this.entryCacheLocation.Text
            };
        }

        protected void buttonCancelClick(object sender, EventArgs e) {
            var loginWindow = new LoginWindow(User);
            loginWindow.Show();
            Destroy();
        }

        protected void buttonDefaultsClick(object sender, EventArgs e) {
            Preferences = Preferences.GetDefault();
        }

        protected void buttonSaveClick(object sender, EventArgs e) {
            Preferences preferences = getModelFromFields();
            this._presenter.Save(preferences);
            var browserWindow = new BrowserWindow(User, preferences);
            browserWindow.Show();
            Destroy();
        }

        protected void buttonDownloadLocationClick(object sender, EventArgs e) {
            // Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
            var dialog = new FileChooserDialog("Select folder to save downloaded photos:",
                null, FileChooserAction.SelectFolder);

            Preferences preferences = getModelFromFields();

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

        protected void buttonCacheLocationClick(object sender, EventArgs e) {
            // Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
            var dialog = new FileChooserDialog("Select folder to save the cached thumbnails:",
                null, FileChooserAction.SelectFolder);

            Preferences preferences = getModelFromFields();

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

        protected void buttonEmptyCacheClick(object sender, EventArgs e) {
            ResponseType result = MessageBox.Show(this, "Are you sure you want to empty the cache folder?",
                ButtonsType.YesNo, MessageType.Question);
            if (result == ResponseType.Yes) {
                this._presenter.EmptyCacheDirectory(Preferences.CacheLocation);
                SetCacheSize();
            }
        }
    }
}
