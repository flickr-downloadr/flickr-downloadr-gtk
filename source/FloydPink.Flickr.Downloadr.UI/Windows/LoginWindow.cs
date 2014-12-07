using System;
using System.Diagnostics;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.CachedImage;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.UI.Widgets;
using Gtk;
using Mono.Unix;

namespace FloydPink.Flickr.Downloadr.UI.Windows {
    public partial class LoginWindow : BaseWindow, ILoginView {
        private User _user;
        private SpinnerWidget spinner;
        private readonly ILoginPresenter _presenter;

        public LoginWindow()
            : this(new User()) { }

        public LoginWindow(User user) {
            Log.Debug("ctor");
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            User = user;

            AddSpinnerControl();

            _presenter = Bootstrapper.GetPresenter<ILoginView, ILoginPresenter>(this);
            _presenter.InitializeScreen();
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
            buttonLogin.TooltipText = "Log in to flickr using OAuth";
            buttonPrefs.TooltipText = "Update the Preferences";
            buttonLogout.TooltipText = "Log out from the currently logged in account";
            buttonContinue.TooltipText = "Browse and download the photos from the logged in account";
        }

        private void AddSpinnerControl() {
            Log.Debug("AddSpinnerControl");
            spinner = new SpinnerWidget {
                Name = "loginSpinner",
                Cancellable = true,
                Operation = "Please wait...",
                Visible = false
            };
            spinner.SpinnerCanceled +=
                (object sender, EventArgs e) => { Application.Invoke(delegate { this.hboxLogin.Sensitive = true; }); };
            vbox2.Add(spinner);

            var spinnerSlot = ((Box.BoxChild) (vbox2[spinner]));
            spinnerSlot.Position = 0;
            spinnerSlot.Expand = true;
        }

        //		private void EditLogConfigClick(object sender, RoutedEventArgs e)
        //		{
        //			OpenInNotepad(Bootstrapper.GetLogConfigFile().FullName);
        //		}
        //
        //		private void ViewLogClick(object sender, RoutedEventArgs e)
        //		{
        //			OpenInNotepad(Bootstrapper.GetLogFile().FullName);
        //		}
        //
        //		private static void OpenInNotepad(string filepath)
        //		{
        //			Process.Start("notepad.exe", filepath);
        //		}
        //
        protected void buttonLoginClick(object sender, EventArgs e) {
            Log.Debug("buttonLoginClick");
            _presenter.Login();
        }

        protected void buttonLogoutClick(object sender, EventArgs e) {
            Log.Debug("buttonLogoutClick");
            _presenter.Logout();
        }

        protected void buttonContinueClick(object sender, EventArgs e) {
            Log.Debug("buttonContinueClick");
            _presenter.Continue();
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

        public User User {
            get { return _user; }
            set {
                _user = value;
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

        public void ShowSpinner(bool show) {
            Log.Debug("ShowSpinner");
            Application.Invoke(delegate {
                                   this.hboxLogin.Sensitive = !show;
                                   this.spinner.Visible = show;
                               });
        }

        public void OpenBrowserWindow() {
            Log.Debug("OpenBrowserWindow");
            var browserWindow = new BrowserWindow(User, Preferences);
            browserWindow.Show();
            Destroy();
        }

        public void OpenPreferencesWindow(Preferences preferences) {
            Log.Debug("OpenPreferencesWindow");
            var preferencesWindow = new PreferencesWindow(User, preferences);
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
