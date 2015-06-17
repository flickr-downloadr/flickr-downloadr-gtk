namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using Bootstrap;
    using Gtk;
    using Helpers;
    using Model;
    using Model.Enums;
    using Presentation;
    using Presentation.Views;
    using Widgets;

    public partial class BrowserWindow : BaseWindow, IBrowserView {
        private readonly IBrowserPresenter _presenter;
        private bool _doNotFireOnSelectionChanged;
        private string _page;
        private string _pages;
        private string _perPage;
        private IEnumerable<Photo> _photos;
        private string _total;
        private SpinnerWidget spinner;

        public BrowserWindow(Session session) {
            Log.Debug("ctor");
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            Preferences = session.Preferences;
            User = session.User;
            AllSelectedPhotos = new Dictionary<string, Dictionary<string, Photo>>();

            AddSpinnerWidget();

            this._presenter = Bootstrapper.GetPresenter<IBrowserView, IBrowserPresenter>(this);
            this._presenter.InitializePhotoset();
        }

        public int SelectedPhotosCount { get { return AllSelectedPhotos.Values.SelectMany(d => d.Values).Count(); } }

        public string SelectedPhotosCountText
        {
            get
            {
                var selectionCount = SelectedPhotosExist
                    ? SelectedPhotosCount.ToString(CultureInfo.InvariantCulture)
                    : string.Empty;
                return string.IsNullOrEmpty(selectionCount)
                    ? "Selection"
                    : string.Format("Selection ({0})", selectionCount);
            }
        }

        public bool SelectedPhotosExist { get { return SelectedPhotosCount != 0; } }

        public bool AreAnyPagePhotosSelected
        {
            get { return Page != null && AllSelectedPhotos.ContainsKey(Page) && AllSelectedPhotos[Page].Count != 0; }
        }

        public bool AreAllPagePhotosSelected
        {
            get
            {
                return Photos != null &&
                       (!AllSelectedPhotos.ContainsKey(Page) || Photos.Count() != AllSelectedPhotos[Page].Count);
            }
        }

        public string FirstPhoto
        {
            get
            {
                return (((int.Parse(Page) - 1) * int.Parse(PerPage)) + 1).
                    ToString(CultureInfo.InvariantCulture);
            }
        }

        public string LastPhoto
        {
            get
            {
                var maxLast = int.Parse(Page) * int.Parse(PerPage);
                return maxLast > int.Parse(Total) ? Total : maxLast.ToString(CultureInfo.InvariantCulture);
            }
        }

        public User User { get; set; }
        public Preferences Preferences { get; set; }

        public IEnumerable<Photo> Photos
        {
            get { return this._photos; }
            set
            {
                this._photos = value;
                UpdateUI();
                Application.Invoke(delegate {
                                       this._doNotFireOnSelectionChanged = true;
                                       SelectAlreadySelectedPhotos();
                                       this._doNotFireOnSelectionChanged = false;
                                   });
            }
        }

        public IDictionary<string, Dictionary<string, Photo>> AllSelectedPhotos { get; set; }
        public PhotosetType PhotosetType { get; set; }

        public string Page
        {
            get { return this._page; }
            set
            {
                this._page = value;
            }
        }

        public string Pages
        {
            get { return this._pages; }
            set
            {
                this._pages = value;
            }
        }

        public string PerPage
        {
            get { return this._perPage; }
            set
            {
                this._perPage = value;
            }
        }

        public string Total
        {
            get { return this._total; }
            set
            {
                this._total = value;
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

        protected void OnDeleteEvent(object sender, DeleteEventArgs args) {
            Log.Debug("OnDeleteEvent");
            MainClass.Quit();
            args.RetVal = true;
        }

        private void AddTooltips() {
            Log.Debug("AddTooltips");
            this.buttonBack.TooltipText = "Close this window and go back to the photoset selection window";
            this.buttonSelectAll.TooltipText = "Select all the photos on this page";
            this.buttonUnSelectAll.TooltipText = "Deselect all the photos on this page";
            this.buttonFirstPage.TooltipText = "Go to the first page of photos";
            this.buttonPreviousPage.TooltipText = "Go to the previous page of photos";
            this.comboboxPage.TooltipText = "Select a page to quickly jump there";
            this.buttonNextPage.TooltipText = "Go to the next page of photos";
            this.buttonLastPage.TooltipText = "Go the last page of photos";
            this.buttonDownloadSelection.TooltipText = "Download the selected photos from all pages";
            this.buttonDownloadThisPage.TooltipText = "Download all the photos from this page";
            this.buttonDownloadAllPages.TooltipText = "Download all the photos from all the pages";
        }

        private void AddSpinnerWidget() {
            Log.Debug("AddSpinnerWidget");
            this.spinner = new SpinnerWidget {
                Name = "browserSpinner",
                Cancellable = true,
                Operation = "Please wait...",
                Visible = false
            };
            this.spinner.SpinnerCanceled += (object sender, EventArgs e) => {
                                                this.scrolledwindowPhotos.Visible = true;
                                                this.hboxButtons.Sensitive = true;
                                                this._presenter.CancelDownload();
                                            };
            this.hboxSpinner.Add(this.spinner);
            var spinnerSlot = ((Box.BoxChild) (this.hboxSpinner[this.spinner]));
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
                Focus = this.buttonBack;
            }
        }

        private void ClearSelectedPhotos() {
            Log.Debug("ClearSelectedPhotos");
            AllSelectedPhotos.Clear();
            SetSelectionOnAllImages(false);
        }

        private void UpdateSelectionButtons() {
            Log.Debug("UpdateSelectionButtons");
            this.buttonSelectAll.Sensitive = AreAllPagePhotosSelected;
            this.buttonUnSelectAll.Sensitive = AreAnyPagePhotosSelected;

            this.buttonDownloadSelection.Label = SelectedPhotosCountText;
            this.buttonDownloadSelection.Sensitive = SelectedPhotosExist;
        }

        private void UpdateUI() {
            Log.Debug("UpdateUI");
            Application.Invoke(delegate {
                                   UpdateSelectionButtons();

                                   this.labelPhotos.Markup = string.Format("<small>{0} - {1} of {2} Photos</small>",
                                       FirstPhoto, LastPhoto, Total);
                                   this.labelPages.Markup = string.Format("<small>{0} of {1} Pages</small>", Page, Pages);

                                   var pages = new ListStore(typeof (string));
                                   this.comboboxPage.Model = pages;
                                   Enumerable.Range(1, int.Parse(Pages)).ToList().ForEach(p => pages.AppendValues(p.ToString()));
                                   this.comboboxPage.Active = int.Parse(Page) - 1;

                                   this.buttonPreviousPage.Sensitive = this.buttonFirstPage.Sensitive = Page != "1";
                                   this.buttonNextPage.Sensitive = this.buttonLastPage.Sensitive = Page != Pages;

                                   this.scrolledwindowPhotos.Vadjustment.Value = 0;

                                   var hasPhotos = Photos.Any();
                                   hbox5.Sensitive = hasPhotos;
                                   hboxCenter.Sensitive = hasPhotos;
                                   hboxRight.Sensitive = hasPhotos;
                               });
            SetupTheImageGrid(Photos);
        }

        #region PhotoGrid

        private const int NumberOfPhotosInARow = 5;

        private void OnSelectionChanged(object sender, EventArgs e) {
            Log.Debug("OnSelectionChanged");
            if (this._doNotFireOnSelectionChanged) {
                return;
            }
            var cachedImage = (PhotoWidget) sender;

            if (!AllSelectedPhotos.ContainsKey(Page)) {
                AllSelectedPhotos[Page] = new Dictionary<string, Photo>();
            }

            if (cachedImage.IsSelected) {
                AllSelectedPhotos[Page].Add(cachedImage.Photo.Id, (Photo)cachedImage.Photo);
            } else {
                AllSelectedPhotos[Page].Remove(cachedImage.Photo.Id);
            }

            UpdateSelectionButtons();
        }

        private HBox AddImageToRow(HBox hboxPhotoRow, int j, Photo photo, string rowId) {
            Log.Debug("AddImageToRow");
            Box.BoxChild hboxChild;
            if (photo != null) {
                var imageCell = new PhotoWidget();
                imageCell.Name = string.Format("{0}Image{1}", rowId, j);
                imageCell.ImageUrl = photo.LargeSquare150X150Url;
                imageCell.Photo = photo;
                imageCell.SelectionChanged += OnSelectionChanged;
                hboxPhotoRow.Add(imageCell);
                hboxChild = ((Box.BoxChild) (hboxPhotoRow[imageCell]));
            } else {
                var dummyImage = new Image();
                dummyImage.Name = string.Format("{0}Image{1}", rowId, j);
                hboxPhotoRow.Add(dummyImage);
                hboxChild = ((Box.BoxChild) (hboxPhotoRow[dummyImage]));
            }
            hboxPhotoRow.Homogeneous = true;
            hboxChild.Position = j;
            return hboxPhotoRow;
        }

        private void SetupTheImageRow(int i, IEnumerable<Photo> rowPhotos) {
            Log.Debug("SetupTheImageRow");
            var rowPhotosAsList = rowPhotos as IList<Photo> ?? rowPhotos.ToList();
            var rowPhotosCount = rowPhotosAsList.Count();

            var rowId = string.Format("hboxPhotoRow{0}", i);
            var hboxPhotoRow = new HBox();
            hboxPhotoRow.Name = rowId;
            hboxPhotoRow.Spacing = 6;

            for (var j = 0; j < NumberOfPhotosInARow; j++) {
                if (j < rowPhotosCount) {
                    hboxPhotoRow = AddImageToRow(hboxPhotoRow, j, rowPhotosAsList.ElementAt(j), rowId);
                } else {
                    hboxPhotoRow = AddImageToRow(hboxPhotoRow, j, null, rowId);
                }
            }

            Application.Invoke(delegate {
                                   this.vboxPhotos.Add(hboxPhotoRow);
                                   var vboxChild = ((Box.BoxChild) (this.vboxPhotos[hboxPhotoRow]));
                                   vboxChild.Position = i;
                                   vboxChild.Padding = 10;
                                   this.vboxPhotos.ShowAll();
                               });
        }

        private void SetupTheImageGrid(IEnumerable<Photo> photos) {
            Log.Debug("SetupTheImageGrid");
            var photosAsList = photos as IList<Photo> ?? photos.ToList();
            var photosCount = photosAsList.Count();
            var numberOfRows = photosCount / NumberOfPhotosInARow;
            if (photosCount % NumberOfPhotosInARow > 0) {
                numberOfRows += 1; // add an additional row for remainder of the images that won't reach full row
            }
            numberOfRows = numberOfRows < 3 ? 3 : numberOfRows; // render a minimum of 3 rows

            foreach (var child in this.vboxPhotos.Children) {
                Application.Invoke(delegate { this.vboxPhotos.Remove(child); });
            }

            if (photosCount == 0) {
                return;
            }

            for (var i = 0; i < numberOfRows; i++) {
                var rowPhotos = photosAsList.Skip(i * NumberOfPhotosInARow).Take(NumberOfPhotosInARow);
                SetupTheImageRow(i, rowPhotos);
            }
        }

        private void SetSelectionOnAllImages(bool selected) {
            Log.Debug("SetSelectionOnAllImages");
            foreach (var box in this.vboxPhotos.AllChildren) {
                var hbox = box as HBox;
                if (hbox == null) {
                    continue;
                }
                foreach (var image in hbox.AllChildren) {
                    var cachedImage = image as PhotoWidget;
                    if (cachedImage != null) {
                        cachedImage.IsSelected = selected;
                    }
                }
            }
        }

        private void FindAndSelectPhoto(Photo photo) {
            Log.Debug("FindAndSelectPhoto");
            foreach (var box in this.vboxPhotos.AllChildren) {
                var hbox = box as HBox;
                if (hbox == null) {
                    continue;
                }
                foreach (var image in hbox.AllChildren) {
                    var cachedImage = image as PhotoWidget;
                    if (cachedImage != null && cachedImage.Photo.Id == photo.Id) {
                        cachedImage.IsSelected = true;
                        return;
                    }
                }
            }
        }

        private void SelectPhotos(List<Photo> photos) {
            Log.Debug("SelectPhotos");
            foreach (var photo in photos) {
                FindAndSelectPhoto(photo);
            }
        }

        #endregion

        #region "Button Events"

        protected void buttonBackClick(object sender, EventArgs e) {
            Log.Debug("buttonBackClick");
            var landingWindow = new LandingWindow(new Session(User, Preferences));
            landingWindow.Show();
            Destroy();
        }

        protected async void buttonNextPageClick(object sender, EventArgs e) {
            Log.Debug("buttonNextPageClick");
            LoseFocus((Button) sender);
            await this._presenter.NavigateTo(PhotoOrAlbumPage.Next);
        }

        protected async void buttonLastPageClick(object sender, EventArgs e) {
            Log.Debug("buttonLastPageClick");
            LoseFocus((Button) sender);
            await this._presenter.NavigateTo(PhotoOrAlbumPage.Last);
        }

        protected async void buttonFirstPageClick(object sender, EventArgs e) {
            Log.Debug("buttonFirstPageClick");
            LoseFocus((Button) sender);
            await this._presenter.NavigateTo(PhotoOrAlbumPage.First);
        }

        protected async void buttonPreviousPageClick(object sender, EventArgs e) {
            Log.Debug("buttonPreviousPageClick");
            LoseFocus((Button) sender);
            await this._presenter.NavigateTo(PhotoOrAlbumPage.Previous);
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

        protected async void buttonDownloadSelectionClick(object sender, EventArgs e) {
            Log.Debug("buttonDownloadSelectionClick");
            LoseFocus((Button) sender);
            await this._presenter.DownloadSelection();
        }

        protected async void buttonDownloadThisPageClick(object sender, EventArgs e) {
            Log.Debug("buttonDownloadThisPageClick");
            LoseFocus((Button) sender);
            await this._presenter.DownloadThisPage();
        }

        protected async void buttonDownloadAllPagesClick(object sender, EventArgs e) {
            Log.Debug("buttonDownloadAllPagesClick");
            LoseFocus((Button) sender);
            await this._presenter.DownloadAllPages();
        }

        protected async void comboboxPageChange(object sender, EventArgs e) {
            Log.Debug("comboboxPageChange");
            var jumpToPage = ((ComboBox) sender).ActiveText;
            if (jumpToPage != null && jumpToPage != Page) {
                await this._presenter.NavigateTo(int.Parse(jumpToPage));
            }
        }

        #endregion
    }
}
