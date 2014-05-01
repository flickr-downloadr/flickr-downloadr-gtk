using System;
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
        private readonly ILoginPresenter _presenter;
        private User _user;

        private SpinnerWidget spinner;

        public LoginWindow()
            : this(new User()) { }

        public LoginWindow(User user) {
            Build();

            AddTooltips();

            Title += VersionHelper.GetVersionString();
            User = user;

            AddSpinnerControl();

            this._presenter = Bootstrapper.GetPresenter<ILoginView, ILoginPresenter>(this);
            this._presenter.InitializeScreen();
        }

        protected void OnDeleteEvent(object sender, DeleteEventArgs args) {
            Application.Quit();
            args.RetVal = true;
        }

        private void SetWelcomeLabel(User user) {
            Application.Invoke(delegate {
                                   string welcomeMessage = string.IsNullOrEmpty(user.UserNsId)
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
            this.buttonLogin.TooltipText = "Log in to flickr using OAuth";
            this.buttonPrefs.TooltipText = "Update the Preferences";
            this.buttonLogout.TooltipText = "Log out from the currently logged in account";
            this.buttonContinue.TooltipText = "Browse and download the photos from the logged in account";
        }

        private void AddSpinnerControl() {
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
            this._presenter.Login();
        }

        protected void buttonLogoutClick(object sender, EventArgs e) {
            this._presenter.Logout();
        }

        protected void buttonContinueClick(object sender, EventArgs e) {
            this._presenter.Continue();
        }

        protected void buttonAboutClick(object sender, EventArgs e) {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowAll();
        }

        protected void buttonPrefsClick(object sender, EventArgs e) {
            OpenPreferencesWindow(Preferences);
        }

        #region ILoginView Members

        protected Preferences Preferences { get; set; }

        public User User {
            get { return this._user; }
            set {
                this._user = value;
                SetWelcomeLabel(value);
            }
        }

        public void ShowLoggedInControl(Preferences preferences) {
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
            Application.Invoke(delegate {
                                   this.hboxLogin.Visible = true;
                                   this.labelMessage.LabelProp =
                                       Catalog.GetString("Welcome to Flickr Downloadr. Click 'Login' to continue.");

                                   this.buttonPrefs.Visible = false;
                                   this.hboxBottomButtons.Visible = false;
                                   this.hboxAvatar.Visible = false;
                               });
        }

        public void ShowSpinner(bool show) {
            Application.Invoke(delegate {
                                   this.hboxLogin.Sensitive = !show;
                                   this.spinner.Visible = show;
                               });
        }

        public void OpenBrowserWindow() {
            var browserWindow = new BrowserWindow(User, Preferences);
            browserWindow.Show();
            Destroy();
        }

        public void OpenPreferencesWindow(Preferences preferences) {
            var preferencesWindow = new PreferencesWindow(User, preferences);
            preferencesWindow.Show();
            Destroy();
        }

        #endregion
    }
}
