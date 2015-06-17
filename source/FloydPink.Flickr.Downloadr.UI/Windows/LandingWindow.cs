namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using System;
    using System.Collections.Generic;
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

    public partial class LandingWindow : BaseWindow, ILandingView {
        private readonly ILandingPresenter _presenter;
        private SpinnerWidget spinner;

        private string _page;
        private string _pages;
        private string _perPage;
        private string _total;
        private IEnumerable<Photoset> _albums;

        public LandingWindow(Session session) {
            Log.Debug("ctor");
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            Preferences = session.Preferences;
            User = session.User;

            AddSpinnerWidget();

            this._presenter = Bootstrapper.GetPresenter<ILandingView, ILandingPresenter>(this);
            this._presenter.Initialize();
        }

        public string FirstAlbum
        {
            get
            {
                return (((int.Parse(Page) - 1) * int.Parse(PerPage)) + 1).
                    ToString(CultureInfo.InvariantCulture);
            }
        }

        public string LastAlbum
        {
            get
            {
                var maxLast = int.Parse(Page) * int.Parse(PerPage);
                return maxLast > int.Parse(Total) ? Total : maxLast.ToString(CultureInfo.InvariantCulture);
            }
        }
        public User User { get; set; }

        public Preferences Preferences { get; set; }

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

        public IEnumerable<Photoset> Albums { 
            get { 
                return _albums ?? new List<Photoset>();
            } 
            set { 
                _albums = value;
                UpdateUI();
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
            Application.Invoke(delegate { this.spinner.Operation = operationText; });
        }

        private void AddSpinnerWidget() {
            Log.Debug("AddSpinnerWidget");
            this.spinner = new SpinnerWidget {
                Name = "landingSpinner",
                Cancellable = false,
                Operation = "Please wait...",
                Visible = false
            };
            this.hboxSpinner.Add(this.spinner);
            var spinnerSlot = ((Box.BoxChild) (this.hboxSpinner[this.spinner]));
            spinnerSlot.Position = 0;
            spinnerSlot.Expand = true;
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs args) {
            Log.Debug("OnDeleteEvent");
            MainClass.Quit();
            args.RetVal = true;
        }

        private void AddTooltips() {
            Log.Debug("AddTooltips");
            this.buttonBack.TooltipText = "Close this window and go back to the login window";
            this.buttonFirstPage.TooltipText = "Go to the first page of albums";
            this.buttonPreviousPage.TooltipText = "Go to the previous page of albums";
            this.comboboxPage.TooltipText = "Select a page to quickly jump there";
            this.buttonNextPage.TooltipText = "Go to the next page of albums";
            this.buttonLastPage.TooltipText = "Go the last page of albums";
            this.buttonContinue.TooltipText = "Browse and download photos from the selected photoset";
        }

        private void UpdateUI() {
            Log.Debug("UpdateUI");
            Application.Invoke(delegate {
                                   this.labelPhotos.Markup = string.Format("<small>{0} - {1} of {2} Albums</small>",
                                       FirstAlbum, LastAlbum, Total);
                                   this.labelPages.Markup = string.Format("<small>{0} of {1} Pages</small>", Page, Pages);

                                   var pages = new ListStore(typeof (string));
                                   this.comboboxPage.Model = pages;
                                   Enumerable.Range(1, int.Parse(Pages)).ToList().ForEach(p => pages.AppendValues(p.ToString()));
                                   this.comboboxPage.Active = int.Parse(Page) - 1;

                                   this.buttonPreviousPage.Sensitive = this.buttonFirstPage.Sensitive = Page != "1";
                                   this.buttonNextPage.Sensitive = this.buttonLastPage.Sensitive = Page != Pages;

                                   this.scrolledwindowPhotos.Vadjustment.Value = 0;

                                   hboxCenter.Sensitive = Albums.Any();
                               });
             SetupTheAlbumGrid(Albums);
        }

        #region PhotoGrid

        private const int NumberOfAlbumsInARow = 5;

        private void OnSelectionChanged(object sender, EventArgs e) {
            Log.Debug("OnSelectionChanged");
//            if (this._doNotFireOnSelectionChanged) {
//                return;
//            }
//            var cachedImage = (PhotoWidget) sender;
//
//            if (!AllSelectedPhotos.ContainsKey(Page)) {
//                AllSelectedPhotos[Page] = new Dictionary<string, Photo>();
//            }
//
//            if (cachedImage.IsSelected) {
//                AllSelectedPhotos[Page].Add(cachedImage.Photo.Id, cachedImage.Photo);
//            } else {
//                AllSelectedPhotos[Page].Remove(cachedImage.Photo.Id);
//            }
//
//            UpdateSelectionButtons();
        }

        private HBox AddAlbumToRow(HBox hboxPhotoRow, int j, Photoset album, string rowId) {
            Log.Debug("AddAlbumToRow");
            Box.BoxChild hboxChild;
            if (album != null) {
                var imageCell = new PhotoWidget();
                imageCell.Name = string.Format("{0}Image{1}", rowId, j);
                imageCell.ImageUrl = album.CoverPhotoUrl;
                imageCell.Photo = album;
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

        private void SetupTheAlbumRow(int i, IEnumerable<Photoset> rowAlbums) {
            Log.Debug("SetupTheAlbumRow");
            var rowAlbumsAsList = rowAlbums as IList<Photoset> ?? rowAlbums.ToList();
            var rowAlbumsCount = rowAlbumsAsList.Count();

            var rowId = string.Format("hboxPhotoRow{0}", i);
            var hboxPhotoRow = new HBox();
            hboxPhotoRow.Name = rowId;
            hboxPhotoRow.Spacing = 6;

            for (var j = 0; j < NumberOfAlbumsInARow; j++) {
                if (j < rowAlbumsCount) {
                    hboxPhotoRow = AddAlbumToRow(hboxPhotoRow, j, rowAlbumsAsList.ElementAt(j), rowId);
                } else {
                    hboxPhotoRow = AddAlbumToRow(hboxPhotoRow, j, null, rowId);
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

        private void SetupTheAlbumGrid(IEnumerable<Photoset> albums) {
            Log.Debug("SetupTheAlbumGrid");
            var albumsAsList = albums as IList<Photoset> ?? albums.ToList();
            var albumsCount = albumsAsList.Count();
            var numberOfRows = albumsCount / NumberOfAlbumsInARow;
            if (albumsCount % NumberOfAlbumsInARow > 0) {
                numberOfRows += 1; // add an additional row for remainder of the images that won't reach full row
            }
            numberOfRows = numberOfRows < 3 ? 3 : numberOfRows; // render a minimum of 3 rows

            foreach (var child in this.vboxPhotos.Children) {
                Application.Invoke(delegate { this.vboxPhotos.Remove(child); });
            }

            if (albumsCount == 0) {
                return;
            }

            for (var i = 0; i < numberOfRows; i++) {
                var rowAlbums = albumsAsList.Skip(i * NumberOfAlbumsInARow).Take(NumberOfAlbumsInARow);
                SetupTheAlbumRow(i, rowAlbums);
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

        private void LoseFocus(Button element) {
            Log.Debug("LoseFocus");
            if (element.HasFocus) {
                Focus = this.buttonBack;
            }
        }

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

        protected async void comboboxPageChange(object sender, EventArgs e) {
            Log.Debug("comboboxPageChange");
            var jumpToPage = ((ComboBox) sender).ActiveText;
            if (jumpToPage != null && jumpToPage != Page) {
                await this._presenter.NavigateTo(int.Parse(jumpToPage));
            }
        }

        protected void buttonContinueClick(object sender, EventArgs e) {
            Log.Debug("buttonContinueClick");
            var browserWindow = new BrowserWindow(new Session(User, Preferences));
            browserWindow.Show();
            Destroy();
        }
    }
}
