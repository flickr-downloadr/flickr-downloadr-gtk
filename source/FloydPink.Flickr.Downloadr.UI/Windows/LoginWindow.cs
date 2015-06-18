namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using System;
    using System.Diagnostics;
    using Bootstrap;
    using CachedImage;
    using Gtk;
    using Helpers;
    using Model;
    using Mono.Unix;
    using Presentation;
    using Presentation.Views;
    using Widgets;

    public partial class LoginWindow : BaseWindow, ILoginView {
        private readonly ILoginPresenter _presenter;
        private User _user;
        private SpinnerWidget spinner;

        public LoginWindow()
            : this(new User()) { }

        public LoginWindow(User user) {
            Log.Debug("ctor");
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            User = user;

            AddSpinnerControl();

            this._presenter = Bootstrapper.GetPresenter<ILoginView, ILoginPresenter>(this);
            this._presenter.InitializeScreen();
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs args) {
            Log.Debug("OnDeleteEvent");
            MainClass.Quit();
            args.RetVal = true;
        }

        private void SetWelcomeLabel(User user) {
            Log.Debug("SetWelcomeLabel");
            Application.Invoke(delegate {
                                   var welcomeMessage = string.IsNullOrEmpty(user.UserNsId)
                                       ? string.Empty
                                       : user.WelcomeMessage;
                                   this.labelWelcomeUsername.LabelProp = string.Format("<b><big>{0}</big></b>",
                                       welcomeMessage);
                                   if (user.Info == null) {
                                       return;
                                   }
                                   this.imageBuddyIcon.SetCachedImage(user.Info.BuddyIconUrl);
                               });
        }

        private void AddTooltips() {
            Log.Debug("AddTooltips");
            this.buttonLogin.TooltipText = "Log in to flickr using OAuth";
            this.buttonPrefs.TooltipText = "Update the Preferences";
            this.buttonLogout.TooltipText = "Log out from the currently logged in account";
            this.buttonContinue.TooltipText = "Browse and download the photos from the logged in account";
        }

        private void AddSpinnerControl() {
            Log.Debug("AddSpinnerControl");
            this.spinner = new SpinnerWidget {
                Name = "loginSpinner",
                Cancellable = true,
                Operation = "Please wait...",
                Visible = false
            };
            this.spinner.SpinnerCanceled +=
                (object sender, EventArgs e) => { Application.Invoke(delegate { this.hboxLogin.Sensitive = true; }); };
            this.vbox2.Add(this.spinner);

            var spinnerSlot = ((Box.BoxChild) (this.vbox2[this.spinner]));
            spinnerSlot.Position = 0;
            spinnerSlot.Expand = true;
        }

        protected void buttonLoginClick(object sender, EventArgs e) {
            Log.Debug("buttonLoginClick");
            this._presenter.Login();
        }

        protected void buttonLogoutClick(object sender, EventArgs e) {
            Log.Debug("buttonLogoutClick");
            this._presenter.Logout();
        }

        protected void buttonContinueClick(object sender, EventArgs e) {
            Log.Debug("buttonContinueClick");
            this._presenter.Continue();
        }

        protected void buttonAboutClick(object sender, EventArgs e) {
            Log.Debug("buttonAboutClick");
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowAll();
        }

        protected void buttonPrefsClick(object sender, EventArgs e) {
            Log.Debug("buttonPrefsClick");
            OpenPreferencesWindow(Preferences);
        }

        #region ILoginView Members

        protected Preferences Preferences { get; set; }

        public User User
        {
            get { return this._user; }
            set
            {
                this._user = value;
                SetWelcomeLabel(value);
            }
        }

        public void ShowLoggedInControl(Preferences preferences) {
            Log.Debug("ShowLoggedInControl");

            if (preferences != null) {
                Bootstrapper.ReconfigureLogging(preferences.LogLevel.ToString(), preferences.LogLocation);
            }

            Application.Invoke(delegate {
                                   Preferences = preferences;
                                   FileCache.AppCacheDirectory = Preferences != null
                                       ? Preferences.CacheLocation
                                       : Preferences.GetDefault().CacheLocation;

                                   this.buttonPrefs.Visible = Preferences != null;
                                   this.hboxBottomButtons.Visible = true;
                                   this.hboxAvatar.Visible = true;

                                   this.hboxLogin.Visible = false;
                                   this.labelMessage.LabelProp =
                                       Catalog.GetString("Click 'Continue' to browse and download photos...");
                               });
        }

        public void ShowLoggedOutControl() {
            Log.Debug("ShowLoggedOutControl");
            Application.Invoke(delegate {
                                   this.hboxLogin.Visible = true;
                                   this.labelMessage.LabelProp =
                                       Catalog.GetString("Welcome to flickr downloadr. Click 'Login' to continue.");

                                   this.buttonPrefs.Visible = false;
                                   this.hboxBottomButtons.Visible = false;
                                   this.hboxAvatar.Visible = false;
                                   this.hboxUpdate.Visible = true;
                               });
        }

        public override void ShowSpinner(bool show) {
            Log.Debug("ShowSpinner");
            Application.Invoke(delegate {
                                   this.hboxLogin.Sensitive = !show;
                                   this.spinner.Visible = show;
                               });
        }

        public void OpenLandingWindow() {
            Log.Debug("OpenBrowserWindow");
            var landingWindow = new LandingWindow(new Session(User, Preferences));
            landingWindow.Show();
            Destroy();
        }

        public void OpenPreferencesWindow(Preferences preferences) {
            Log.Debug("OpenPreferencesWindow");
            var preferencesWindow = new PreferencesWindow(new Session(User, preferences));
            preferencesWindow.Show();
            Destroy();
        }

        public void ShowUpdateAvailableNotification(string latestVersion) {
            Log.Debug("ShowUpdateAvailableNotification");
            Application.Invoke(delegate {
                                   this.labelUpdate.LabelProp =
                                       string.Format(
                                           "<a href=\"#\"><span color=\"red\">Update Available ( v{0} )</span></a>",
                                           latestVersion);
                                   this.labelUpdate.TooltipText =
                                       "Click here to download the latest version of the application";
                                   this.hboxUpdate.Visible = true;
                               });
        }


        protected void UpdateNotificationClick(object o, ButtonPressEventArgs args) {
            Log.Debug("UpdateNotificationClick");
            Process.Start(VersionHelper.GetUpdateUrl());
            MainClass.Quit();
        }

        #endregion
    }
}
