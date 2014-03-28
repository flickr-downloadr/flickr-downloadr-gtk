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

		public LoginWindow()
			: this(new User())
		{
		}

		public LoginWindow(User user) :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			Title += VersionHelper.GetVersionString();
			User = user;

			_presenter = Bootstrapper.GetPresenter<ILoginView, ILoginPresenter>(this);
			_presenter.InitializeScreen();
		}

		#region ILoginView Members

		protected Preferences Preferences { get; set; }

		public User User
		{
			get { return _user; }
			set
			{
				_user = value;
				//SetWelcomeLabel(value);
			}
		}

		public void ShowLoggedInControl(Preferences preferences)
		{
			Preferences = preferences;
//			FileCache.AppCacheDirectory = Preferences != null
//				? Preferences.CacheLocation
//				: Preferences.GetDefault().CacheLocation;
//			PreferencesButton.Dispatch(
//				p => p.Visibility = (Preferences != null ? Visibility.Visible : Visibility.Collapsed));
//			LoggedInCanvas.Dispatch(c => c.Visibility = Visibility.Visible);
//			LoggedOutCanvas.Dispatch(c => c.Visibility = Visibility.Collapsed);
			Console.WriteLine ("In ShowLoggedInControl");
		}

		public void ShowLoggedOutControl()
		{
//			LoggedOutCanvas.Dispatch(c => c.Visibility = Visibility.Visible);
//			LoggedInCanvas.Dispatch(c => c.Visibility = Visibility.Collapsed);
			Console.WriteLine ("In ShowLoggedOutControl");
		}

		#endregion

		public void ShowSpinner(bool show)
		{
//			Visibility visibility = show ? Visibility.Visible : Visibility.Collapsed;
//			Spinner.Dispatch(s => s.Visibility = visibility);
		}

		public void OpenBrowserWindow()
		{
//			var browserWindow = new BrowserWindow(User, Preferences);
//			browserWindow.Show();
//			Close();
		}

		public void OpenPreferencesWindow(Preferences preferences)
		{
//			var preferencesWindow = new PreferencesWindow(User, preferences);
//			preferencesWindow.Show();
//			Close();
		}
//
//		private void LoginButtonClick(object sender, RoutedEventArgs e)
//		{
//			_presenter.Login();
//		}
//
//		private void LogoutButtonClick(object sender, RoutedEventArgs e)
//		{
//			_presenter.Logout();
//		}
//
//		private void SetWelcomeLabel(User user)
//		{
//			WelcomeUserLabel.Dispatch(
//				l => l.Content = string.IsNullOrEmpty(user.UserNsId) ? string.Empty : user.WelcomeMessage);
//			if (user.Info == null) return;
//			BuddyIcon.Dispatch(i => i.ImageUrl = user.Info.BuddyIconUrl);
//		}
//
//		private void ContinueButtonClick(object sender, RoutedEventArgs e)
//		{
//			_presenter.Continue();
//		}
//
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
//		private void PreferencesButtonClick(object sender, RoutedEventArgs e)
//		{
//			var preferencesWindow = new PreferencesWindow(User, Preferences);
//			preferencesWindow.Show();
//			Close();
//		}
//
//		private void AboutButtonClick(object sender, RoutedEventArgs e)
//		{
//			var aboutWindow = new AboutWindow();
//			aboutWindow.ShowDialog();
//		}

		protected void buttonLoginClick (object sender, EventArgs e)
		{
			_presenter.Login();
		}
	}
}

