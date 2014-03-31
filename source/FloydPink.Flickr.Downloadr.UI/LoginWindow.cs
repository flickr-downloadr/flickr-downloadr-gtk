using System;
using Gtk;
using System.Reflection;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.Helpers;

namespace FloydPink.Flickr.Downloadr
{
	public partial class LoginWindow : Gtk.Window, ILoginView
	{
		private readonly ILoginPresenter _presenter;
		private User _user;

		public LoginWindow ()
			: this (new User ())
		{
		}

		public LoginWindow (User user) :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			Title += VersionHelper.GetVersionString ();
			User = user;

			_presenter = Bootstrapper.GetPresenter<ILoginView, ILoginPresenter> (this);
			_presenter.InitializeScreen ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}

		#region ILoginView Members

		protected Preferences Preferences { get; set; }

		public User User {
			get { return _user; }
			set {
				_user = value;
				SetWelcomeLabel (value);
			}
		}

		public void ShowLoggedInControl (Preferences preferences)
		{
			Application.Invoke (delegate {
				Preferences = preferences;
				//			FileCache.AppCacheDirectory = Preferences != null
				//				? Preferences.CacheLocation
				//				: Preferences.GetDefault().CacheLocation;
				//			PreferencesButton.Dispatch(
				//				p => p.Visibility = (Preferences != null ? Visibility.Visible : Visibility.Collapsed));

				hboxBottomButtons.Visible = true;
				hboxAvatar.Visible = true;

				hboxLogin.Visible = false;
				this.labelMessage.LabelProp = global::Mono.Unix.Catalog.GetString ("Click 'Continue' to browse and download photos...");
			});
		}

		public void ShowLoggedOutControl ()
		{
			Application.Invoke (delegate {
				hboxLogin.Visible = true;
				this.labelMessage.LabelProp = global::Mono.Unix.Catalog.GetString ("Welcome to Flickr Downloadr. Click 'Login' to continue.");

				hboxBottomButtons.Visible = false;
				hboxAvatar.Visible = false;
			});
		}

		public void ShowSpinner (bool show)
		{
			Application.Invoke (delegate {
//			Visibility visibility = show ? Visibility.Visible : Visibility.Collapsed;
//			Spinner.Dispatch(s => s.Visibility = visibility);
			});
		}

		public void OpenBrowserWindow ()
		{
//			var browserWindow = new BrowserWindow(User, Preferences);
//			browserWindow.Show();
//			Close();
		}

		public void OpenPreferencesWindow (Preferences preferences)
		{
			var preferencesWindow = new PreferencesWindow (User, preferences);
			preferencesWindow.Show ();
			Destroy ();
		}

		#endregion

		private void SetWelcomeLabel (User user)
		{
			var welcomeMessage = string.IsNullOrEmpty (user.UserNsId) ? string.Empty : user.WelcomeMessage;
			labelWelcomeUsername.LabelProp = string.Format("<b><big>{0}</big></b>", welcomeMessage);
			if (user.Info == null)
				return;
			//BuddyIcon.Dispatch(i => i.ImageUrl = user.Info.BuddyIconUrl);
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
		protected void buttonLoginClick (object sender, EventArgs e)
		{
			_presenter.Login ();
		}

		protected void buttonLogoutClick (object sender, EventArgs e)
		{
			_presenter.Logout ();
		}

		protected void buttonContinueClick (object sender, EventArgs e)
		{
			_presenter.Continue ();
		}

		protected void buttonAboutClick (object sender, EventArgs e)
		{
			var aboutWindow = new AboutWindow ();
			aboutWindow.ShowAll ();
		}

		protected void buttonPrefsClick (object sender, EventArgs e)
		{
			OpenPreferencesWindow (Preferences);
		}
	}
}

