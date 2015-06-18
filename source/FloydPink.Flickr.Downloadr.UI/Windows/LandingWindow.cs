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
        private IEnumerable<Photoset> _albums;
        private Photoset _privatePhotoset;
        private Photoset _publicPhotoset;
        private bool _unselectingPrivatePhotoset;
        private bool _unselectingPublicPhotoset;
        private SpinnerWidget spinner;

        public LandingWindow(Session session) {
            Log.Debug("ctor");
            Build();

            AddTooltips();

            this.photowidgetPublic.SelectionChanged += OnPublicSetSelectionChanged;
            this.photowidgetPrivate.SelectionChanged += OnPrivateSetSelectionChanged;
            this.albumsGrid.OnSelectionChanged += OnAlbumsSelectionChanged;

            this.hboxPublicPrivate.Visible = false;
            this.vbox1.Visible = false; // All Public Photos
            this.vbox3.Visible = false; // All Photos
            this.scrolledwindowPhotos.Visible = this.labelSets.Visible = false; // Albums;

            Title += VersionHelper.GetVersionString();
            Preferences = session.Preferences;
            User = session.User;

            AddSpinnerWidget();

            this._presenter = Bootstrapper.GetPresenter<ILandingView, ILandingPresenter>(this);
            this._presenter.Initialize();
        }

        private Photoset SelectedPhotoset { get; set; }

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
        public string Page { get; set; }
        public string Pages { get; set; }
        public string PerPage { get; set; }
        public string Total { get; set; }

        public Photoset PublicPhotoset
        {
            get { return this._publicPhotoset; }
            set
            {
                this._publicPhotoset = value;
                this.photowidgetPublic.WidgetItem = value;
                this.vbox1.Visible = value != null;
            }
        }

        public Photoset PrivatePhotoset
        {
            get { return this._privatePhotoset; }
            set
            {
                this._privatePhotoset = value;
                this.photowidgetPrivate.WidgetItem = value;
                this.vbox3.Visible = value != null;
            }
        }

        public IEnumerable<Photoset> Albums
        {
            get { return this._albums ?? new List<Photoset>(); }
            set
            {
                this._albums = value;
                UpdateUI();
            }
        }

        public override void ShowSpinner(bool show) {
            Log.Debug("ShowSpinner");
            Application.Invoke(delegate {
                                   this.hboxButtons.Sensitive = !show;
                                   this.hboxPublicPrivate.Visible = !show;
                                   this.labelSets.Visible = Albums.Any() && !show;
                                   this.scrolledwindowPhotos.Visible = Albums.Any() && !show;
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

                                   if (SelectedPhotoset != null && SelectedPhotoset.Type == PhotosetType.Album) {
                                       SelectedPhotoset = null;
                                   }

                                   UpdateSelectionUI();
                               });
            this.albumsGrid.Items = Albums;
        }

        private void OnPublicSetSelectionChanged(object sender, EventArgs e) {
            Log.Debug("OnPublicSetSelectionChanged");
            if (this._unselectingPublicPhotoset) {
                return;
            }
            var photoWidget = (PhotoWidget) sender;

            if (photoWidget.IsSelected) {
                SelectedPhotoset = PublicPhotoset;
            } else {
                SelectedPhotoset = null;
            }
            UpdateSelectionUI();
        }

        private void OnPrivateSetSelectionChanged(object sender, EventArgs e) {
            Log.Debug("OnPrivateSetSelectionChanged");
            if (this._unselectingPrivatePhotoset) {
                return;
            }
            var photoWidget = (PhotoWidget) sender;

            if (photoWidget.IsSelected) {
                SelectedPhotoset = PrivatePhotoset;
            } else {
                SelectedPhotoset = null;
            }
            UpdateSelectionUI();
        }

        private void OnAlbumsSelectionChanged(object sender, EventArgs e) {
            Log.Debug("OnAlbumsSelectionChanged");
            var photoWidget = (PhotoWidget) sender;

            if (photoWidget.IsSelected) {
                SelectedPhotoset = (Photoset) photoWidget.WidgetItem;
            } else {
                SelectedPhotoset = null;
            }
            UpdateSelectionUI();
        }

        private void UpdateSelectionUI() {
            var isPhotosetSelected = SelectedPhotoset != null;

            this.labelSelectedPhotoset.Visible = isPhotosetSelected;
            this.buttonContinue.Sensitive = isPhotosetSelected;

            if (SelectedPhotoset == null) {
                return;
            }

            var selectedPhotosetLabelColor = SelectedPhotoset.Type == PhotosetType.Album ? "red" : "gray";
            this.labelSelectedPhotoset.LabelProp = isPhotosetSelected
                ? string.Format("<span color=\"{0}\"><b>{1}</b></span>",
                    selectedPhotosetLabelColor, SelectedPhotoset.HtmlEncodedTitle)
                : "";

            this._unselectingPublicPhotoset = true;
            this.photowidgetPublic.IsSelected = SelectedPhotoset.Type == PhotosetType.Public;
            this._unselectingPublicPhotoset = false;

            this._unselectingPrivatePhotoset = true;
            this.photowidgetPrivate.IsSelected = SelectedPhotoset.Type == PhotosetType.All;
            this._unselectingPrivatePhotoset = false;

            this.albumsGrid.DoNotFireSelectionChanged = true;
            foreach (var box in this.albumsGrid.AllItems) {
                var hbox = box as HBox;
                if (hbox == null) {
                    continue;
                }
                foreach (var child in hbox.AllChildren) {
                    var widget = child as PhotoWidget;
                    if (widget != null && (SelectedPhotoset.Type != PhotosetType.Album || widget.WidgetItem.Id != SelectedPhotoset.Id)) {
                        widget.IsSelected = false;
                    }
                }
            }
            this.albumsGrid.DoNotFireSelectionChanged = false;
        }

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
            var browserWindow = new BrowserWindow(new Session(User, Preferences, SelectedPhotoset));
            browserWindow.Show();
            Destroy();
        }
    }
}
