using System;
using System.ComponentModel;
using Gtk;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Extensions;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;

namespace FloydPink.Flickr.Downloadr
{
	public partial class PreferencesWindow : Window, IPreferencesView, INotifyPropertyChanged
	{
		private readonly IPreferencesPresenter _presenter;
		private Preferences _preferences;

		public PreferencesWindow (User user, Preferences preferences) :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
			Title += VersionHelper.GetVersionString ();
			Preferences = preferences;
			User = user;

			_presenter = Bootstrapper.GetPresenter<IPreferencesView, IPreferencesPresenter> (this);

			//SetCacheSize();
		}

		protected User User { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public Preferences Preferences {
			get { return _preferences; }
			set {
				_preferences = value;
				PropertyChanged.Notify (() => Preferences);
			}
		}

		public void ShowSpinner (bool show)
		{
//			Visibility visibility = show ? Visibility.Visible : Visibility.Collapsed;
//			Spinner.Dispatch(s => s.Visibility = visibility);
		}
		//		private void SaveButtonClick(object sender, RoutedEventArgs e)
		//		{
		//			_presenter.Save(Preferences);
		//			var browserWindow = new BrowserWindow(User, Preferences);
		//			browserWindow.Show();
		//			Close();
		//		}
		//
		//		private void SelectDownloadFolderButtonClick(object sender, RoutedEventArgs e)
		//		{
		//			var dialog = new FolderBrowserDialog
		//			{
		//				Description = "Select folder to save downloaded photos:",
		//				SelectedPath = Preferences.DownloadLocation
		//			};
		//			DialogResult result = dialog.ShowDialog();
		//			if (result == System.Windows.Forms.DialogResult.OK)
		//			{
		//				Preferences.DownloadLocation = dialog.SelectedPath;
		//			}
		//		}
		//
		//		private void SelectCacheFolderButtonClick(object sender, RoutedEventArgs e)
		//		{
		//			var dialog = new FolderBrowserDialog
		//			{
		//				Description = "Select folder to save the cached thumbnails:",
		//				SelectedPath = Preferences.CacheLocation
		//			};
		//			DialogResult result = dialog.ShowDialog();
		//			if (result == System.Windows.Forms.DialogResult.OK)
		//			{
		//				Preferences.CacheLocation = dialog.SelectedPath;
		//			}
		//		}
		//
		//		private void DefaultsButtonClick(object sender, RoutedEventArgs e)
		//		{
		//			Preferences = Preferences.GetDefault();
		//		}
		//
		//		private void EmptyCacheClick(object sender, RoutedEventArgs e)
		//		{
		//			ResponseType result = MessageBox.Show(this, "Are you sure you want to empty the cache folder?",
		//				ButtonsType.YesNo, MessageType.Question);
		//			if (result == ResponseType.Yes)
		//			{
		//				_presenter.EmptyCacheDirectory(Preferences.CacheLocation);
		//				SetCacheSize();
		//			}
		//		}
		//
		//		private void SetCacheSize()
		//		{
		////			CacheSize.Content = _presenter.GetCacheFolderSize(Preferences.CacheLocation);
		////			EmptyCacheButton.Visibility = (CacheSize.Content.ToString() == "0 B" || CacheSize.Content.ToString() == "-")
		////				? Visibility.Collapsed
		////				: Visibility.Visible;
		//		}

		protected void buttonCancelClick (object sender, EventArgs e)
		{
			var loginWindow = new LoginWindow (User);
			loginWindow.Show ();
			Destroy ();
		}
	}
}

