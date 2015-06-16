namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using System;
    using Bootstrap;
    using Gtk;
    using Helpers;
    using Model;
    using Presentation;
    using Presentation.Views;
    using Widgets;

    public partial class LandingWindow : BaseWindow, ILandingView {
        private readonly ILandingPresenter _presenter;
        private SpinnerWidget spinner;

        public LandingWindow(User user, Preferences preferences) {
            Log.Debug("ctor");
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            Preferences = preferences;
            User = user;

            AddSpinnerWidget();

            this._presenter = Bootstrapper.GetPresenter<ILandingView, ILandingPresenter>(this);
            this._presenter.Initialize();
        }

        public User User { get; set; }
        public Preferences Preferences { get; set; }

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

        protected void buttonBackClick(object sender, EventArgs e) {
            Log.Debug("buttonBackClick");
            var loginWindow = new LoginWindow {
                User = User
            };
            loginWindow.Show();
            Destroy();
        }

        protected void buttonContinueClick(object sender, EventArgs e) {
            Log.Debug("buttonContinueClick");
            var browserWindow = new BrowserWindow(User, Preferences);
            browserWindow.Show();
            Destroy();
        }
    }
}
