using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Model.Extensions;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.UI.Widgets;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Windows {
    public partial class BrowserWindow : BaseWindow, IBrowserView {
        private bool _doNotFireOnSelectionChanged;
        private string _page;
        private string _pages;
        private string _perPage;
        private IEnumerable<Photo> _photos;
        private string _total;
        private SpinnerWidget spinner;
        private readonly IBrowserPresenter _presenter;

        public BrowserWindow(User user, Preferences preferences) {
            Log.Debug("ctor");
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            Preferences = preferences;
            User = user;
            AllSelectedPhotos = new Dictionary<string, Dictionary<string, Photo>>();

            AddSpinnerWidget();

            _presenter = Bootstrapper.GetPresenter<IBrowserView, IBrowserPresenter>(this);
            _presenter.InitializePhotoset();
        }

        public int SelectedPhotosCount { get { return AllSelectedPhotos.Values.SelectMany(d => d.Values).Count(); } }

        public string SelectedPhotosCountText {
            get {
                var selectionCount = SelectedPhotosExist
                    ? SelectedPhotosCount.ToString(CultureInfo.InvariantCulture)
                    : string.Empty;
                return string.IsNullOrEmpty(selectionCount)
                    ? "Selection"
                    : string.Format("Selection ({0})", selectionCount);
            }
        }

        public bool SelectedPhotosExist { get { return SelectedPhotosCount != 0; } }

        public bool AreAnyPagePhotosSelected {
            get { return Page != null && AllSelectedPhotos.ContainsKey(Page) && AllSelectedPhotos[Page].Count != 0; }
        }

        public bool AreAllPagePhotosSelected {
            get {
                return Photos != null &&
                       (!AllSelectedPhotos.ContainsKey(Page) || Photos.Count() != AllSelectedPhotos[Page].Count);
            }
        }

        public string FirstPhoto {
            get {
                return (((Convert.ToInt32(Page) - 1) * Convert.ToInt32(PerPage)) + 1).
                    ToString(CultureInfo.InvariantCulture);
            }
        }

        public string LastPhoto {
            get {
                var maxLast = Convert.ToInt32(Page) * Convert.ToInt32(PerPage);
                return maxLast > Convert.ToInt32(Total) ? Total : maxLast.ToString(CultureInfo.InvariantCulture);
            }
        }

        public User User { get; set; }
        public Preferences Preferences { get; set; }

        public IEnumerable<Photo> Photos {
            get { return _photos; }
            set {
                _photos = value;
                PropertyChanged.Notify(() => AreAllPagePhotosSelected);
                UpdateUI();
                Application.Invoke(delegate {
                                       this._doNotFireOnSelectionChanged = true;
                                       SelectAlreadySelectedPhotos();
                                       this._doNotFireOnSelectionChanged = false;
                                   });
            }
        }

        public IDictionary<string, Dictionary<string, Photo>> AllSelectedPhotos { get; set; }
        public bool ShowAllPhotos { get { return togglebuttonShowAllPhotos.Active; } }

        public string Page {
            get { return _page; }
            set {
                _page = value;
                PropertyChanged.Notify(() => Page);
                PropertyChanged.Notify(() => AreAnyPagePhotosSelected);
            }
        }

        public string Pages {
            get { return _pages; }
            set {
                _pages = value;
                PropertyChanged.Notify(() => Pages);
            }
        }

        public string PerPage {
            get { return _perPage; }
            set {
                _perPage = value;
                PropertyChanged.Notify(() => PerPage);
            }
        }

        public string Total {
            get { return _total; }
            set {
                _total = value;
                PropertyChanged.Notify(() => Total);
                PropertyChanged.Notify(() => FirstPhoto);
                PropertyChanged.Notify(() => LastPhoto);
            }
        }

        public void ShowSpinner(bool show) {
            Log.Debug("ShowSpinner");
            Application.Invoke(delegate {
                                   this.hboxButtons.Sensitive = !show;
                                   this.scrolledwindowPhotos.Visible = !show;
                                   this.spinner.Visible = show;
                               });
        }

        public void UpdateProgress(string percentDone, string operationText, bool cancellable) {
            Log.Debug("UpdateProgress");
            Application.Invoke(delegate {
                                   this.spinner.Cancellable = cancellable;
                                   this.spinner.Operation = operationText;
                                   this.spinner.PercentDone = percentDone;
                               });
        }

        public bool ShowWarning(string warningMessage) {
            Log.Debug("ShowWarning");
            var result = MessageBox.Show(this, warningMessage, ButtonsType.YesNo, MessageType.Question);
            return result != ResponseType.Yes;
        }

        public void DownloadComplete(string downloadedLocation, bool downloadComplete) {
            Log.Debug("DownloadComplete");
            Application.Invoke(delegate {
                                   var message = downloadComplete
                                       ? "Download completed to the directory"
                                       : "Incomplete download could be found at";
                                   if (downloadComplete) {
                                       ClearSelectedPhotos();
                                   }
                                   MessageBox.Show(this,
                                       string.Format("{0}: {1}{1}{2}", message, Environment.NewLine, downloadedLocation),
                                       ButtonsType.Ok, MessageType.Info);
                               });
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnDeleteEvent(object sender, DeleteEventArgs args) {
            Log.Debug("OnDeleteEvent");
            MainClass.Quit();
            args.RetVal = true;
        }

        private void AddTooltips() {
            Log.Debug("AddTooltips");
            buttonBack.TooltipText = "Close this window and go back to the login window";
            togglebuttonShowAllPhotos.TooltipText =
                "Click to toggle seeing all the photos (including those marked private) or only the public ones";
            buttonSelectAll.TooltipText = "Select all the photos on this page";
            buttonUnSelectAll.TooltipText = "Deselect all the photos on this page";
            buttonFirstPage.TooltipText = "Go to the first page of photos";
            buttonPreviousPage.TooltipText = "Go to the previous page of photos";
            buttonNextPage.TooltipText = "Go to the next page of photos";
            buttonLastPage.TooltipText = "Go the last page of photos";
            buttonDownloadSelection.TooltipText = "Download the selected photos from all pages";
            buttonDownloadThisPage.TooltipText = "Download all the photos from this page";
            buttonDownloadAllPages.TooltipText = "Download all the photos from all the pages";
        }

        private void AddSpinnerWidget() {
            Log.Debug("AddSpinnerWidget");
            spinner = new SpinnerWidget {
                Name = "browserSpinner",
                Cancellable = true,
                Operation = "Please wait...",
                Visible = false
            };
            spinner.SpinnerCanceled += (object sender, EventArgs e) => {
                                           scrolledwindowPhotos.Visible = true;
                                           hboxButtons.Sensitive = true;
                                           _presenter.CancelDownload();
                                       };
            hboxSpinner.Add(spinner);
            var spinnerSlot = ((Box.BoxChild) (hboxSpinner[spinner]));
            spinnerSlot.Position = 0;
            spinnerSlot.Expand = true;
        }

        private void SelectAlreadySelectedPhotos() {
            Log.Debug("SelectAlreadySelectedPhotos");
            if (!AllSelectedPhotos.ContainsKey(Page) || AllSelectedPhotos[Page].Count <= 0) {
                return;
            }

            var photos = Photos.Where(photo => AllSelectedPhotos[Page].ContainsKey(photo.Id)).ToList();
            SelectPhotos(photos);
        }

        private void LoseFocus(Button element) {
            Log.Debug("LoseFocus");
            if (element.HasFocus) {
                Focus = buttonBack;
            }
        }

        private void ClearSelectedPhotos() {
            Log.Debug("ClearSelectedPhotos");
            AllSelectedPhotos.Clear();
            SetSelectionOnAllImages(false);
            PropertyChanged.Notify(() => SelectedPhotosExist);
            PropertyChanged.Notify(() => SelectedPhotosCountText);
        }

        private void UpdateSelectionButtons() {
            Log.Debug("UpdateSelectionButtons");
            buttonSelectAll.Sensitive = AreAllPagePhotosSelected;
            buttonUnSelectAll.Sensitive = AreAnyPagePhotosSelected;

            buttonDownloadSelection.Label = SelectedPhotosCountText;
            buttonDownloadSelection.Sensitive = SelectedPhotosExist;
        }

        private void UpdateUI() {
            Log.Debug("UpdateUI");
            Application.Invoke(delegate {
                                   UpdateSelectionButtons();

                                   this.labelPhotos.Markup = string.Format("<small>{0} - {1} of {2} Photos</small>",
                                       FirstPhoto, LastPhoto, Total);
                                   this.labelPages.Markup = string.Format("<small>{0} of {1} Pages</small>", Page, Pages);

                                   this.buttonPreviousPage.Sensitive = this.buttonFirstPage.Sensitive = Page != "1";
                                   this.buttonNextPage.Sensitive = this.buttonLastPage.Sensitive = Page != Pages;

                                   this.scrolledwindowPhotos.Vadjustment.Value = 0;
                               });
            SetupTheImageGrid(Photos);
        }

        #region "Button Events"

        protected void buttonBackClick(object sender, EventArgs e) {
            Log.Debug("buttonBackClick");
            var loginWindow = new LoginWindow {
                User = User
            };
            loginWindow.Show();
            Destroy();
        }

        protected async void buttonNextPageClick(object sender, EventArgs e) {
            Log.Debug("buttonNextPageClick");
            LoseFocus((Button) sender);
            await _presenter.NavigateTo(PhotoPage.Next);
        }

        protected async void buttonLastPageClick(object sender, EventArgs e) {
            Log.Debug("buttonLastPageClick");
            LoseFocus((Button) sender);
            await _presenter.NavigateTo(PhotoPage.Last);
        }

        protected async void buttonFirstPageClick(object sender, EventArgs e) {
            Log.Debug("buttonFirstPageClick");
            LoseFocus((Button) sender);
            await _presenter.NavigateTo(PhotoPage.First);
        }

        protected async void buttonPreviousPageClick(object sender, EventArgs e) {
            Log.Debug("buttonPreviousPageClick");
            LoseFocus((Button) sender);
            await _presenter.NavigateTo(PhotoPage.Previous);
        }

        protected void buttonSelectAllClick(object sender, EventArgs e) {
            Log.Debug("buttonSelectAllClick");
            LoseFocus((Button) sender);
            SetSelectionOnAllImages(true);
        }

        protected void buttonUnSelectAllClick(object sender, EventArgs e) {
            Log.Debug("buttonUnSelectAllClick");
            LoseFocus((Button) sender);
            SetSelectionOnAllImages(false);
        }

        protected async void togglebuttonShowAllPhotosClick(object sender, EventArgs e) {
            Log.Debug("togglebuttonShowAllPhotosClick");
            var toggleButton = (ToggleButton) sender;
            toggleButton.Label = toggleButton.Active ? "Show Only Public Photos" : "Show All Photos";

            LoseFocus((Button) sender);
            ClearSelectedPhotos();
            await _presenter.InitializePhotoset();
        }

        protected async void buttonDownloadSelectionClick(object sender, EventArgs e) {
            Log.Debug("buttonDownloadSelectionClick");
            LoseFocus((Button) sender);
            await _presenter.DownloadSelection();
        }

        protected async void buttonDownloadThisPageClick(object sender, EventArgs e) {
            Log.Debug("buttonDownloadThisPageClick");
            LoseFocus((Button) sender);
            await _presenter.DownloadThisPage();
        }

        protected async void buttonDownloadAllPagesClick(object sender, EventArgs e) {
            Log.Debug("buttonDownloadAllPagesClick");
            LoseFocus((Button) sender);
            await _presenter.DownloadAllPages();
        }

        #endregion
    }
}
