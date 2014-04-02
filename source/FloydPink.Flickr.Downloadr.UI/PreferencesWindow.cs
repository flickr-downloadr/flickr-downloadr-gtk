using System;
using System.Collections.Generic;
using System.ComponentModel;
using Gtk;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Enums;
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

			SetCacheSize ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}

		protected User User { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public Preferences Preferences {
			get { return _preferences; }
			set {
				_preferences = value;
				setFieldsFromModel (Preferences);
			}
		}

		public void ShowSpinner (bool show)
		{
//			Visibility visibility = show ? Visibility.Visible : Visibility.Collapsed;
//			Spinner.Dispatch(s => s.Visibility = visibility);
		}

		private void SetCacheSize ()
		{
			labelCacheSize.Text = _presenter.GetCacheFolderSize (Preferences.CacheLocation);
			buttonEmptyCache.Visible = !(labelCacheSize.Text == "0 B" || labelCacheSize.Text == "-");
		}

		private void setFieldsFromModel (Preferences preferences)
		{
			// Filename
			radioPhotoId.Active = !preferences.TitleAsFilename;
			radioPhotoTitle.Active = preferences.TitleAsFilename;

			// Download location
			entryDownloadLocation.Text = preferences.DownloadLocation;

			// Download size
			comboboxDownloadSize.Active = (int)preferences.DownloadSize;

			// Metadata
			checkbuttonTags.Active = preferences.Metadata.Contains (PhotoMetadata.Tags);
			checkbuttonDescription.Active = preferences.Metadata.Contains (PhotoMetadata.Description);
			checkbuttonTitle.Active = preferences.Metadata.Contains (PhotoMetadata.Title);

			// Photos per page
			var photosPerPageMap = new Dictionary<string,int> () { 
				{ "25", 0 },
				{ "50", 1 },
				{ "75", 2 },
				{ "100", 3 },
			};
			comboboxPhotosPerPage.Active = photosPerPageMap [preferences.PhotosPerPage.ToString ()];

			//Safety level
			comboboxSafetyLevel.Active = int.Parse (preferences.SafetyLevel) - 1;

			// Tags
			radioTagsInternal.Active = !preferences.NeedOriginalTags;
			radioTagsOriginal.Active = preferences.NeedOriginalTags;

			// Cache location
			entryCacheLocation.Text = preferences.CacheLocation;
		}

		private Preferences getModelFromFields ()
		{
			var metadata = new List<string> ();
			if (checkbuttonTags.Active)
				metadata.Add (PhotoMetadata.Tags);
			if (checkbuttonDescription.Active)
				metadata.Add (PhotoMetadata.Description);
			if (checkbuttonTitle.Active)
				metadata.Add (PhotoMetadata.Title);
			return new Preferences () {
				TitleAsFilename = radioPhotoTitle.Active,
				DownloadLocation = entryDownloadLocation.Text,
				DownloadSize = (PhotoDownloadSize)Enum.Parse (typeof(PhotoDownloadSize), comboboxDownloadSize.ActiveText),
				Metadata = metadata,
				PhotosPerPage = int.Parse (comboboxPhotosPerPage.ActiveText),
				SafetyLevel = (comboboxSafetyLevel.Active + 1).ToString (),
				NeedOriginalTags = radioTagsOriginal.Active,
				CacheLocation = entryCacheLocation.Text
			};
		}

		protected void buttonCancelClick (object sender, EventArgs e)
		{
			var loginWindow = new LoginWindow (User);
			loginWindow.Show ();
			Destroy ();
		}

		protected void buttonDefaultsClick (object sender, EventArgs e)
		{
			Preferences = Preferences.GetDefault ();
		}

		protected void buttonSaveClick (object sender, EventArgs e)
		{
			var preferences = getModelFromFields ();
			_presenter.Save (preferences);
//			var browserWindow = new BrowserWindow (User, preferences);
//			browserWindow.Show ();
//			Destroy ();
		}

		protected void buttonDownloadLocationClick (object sender, EventArgs e)
		{
			// Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
			FileChooserDialog dialog = new FileChooserDialog ("Select folder to save downloaded photos:", 
				                           null, FileChooserAction.SelectFolder);

			var preferences = getModelFromFields ();

			dialog.SetCurrentFolder (preferences.DownloadLocation);

			dialog.AddButton (Stock.Cancel, ResponseType.Cancel);
			dialog.AddButton (Stock.Ok, ResponseType.Ok);

			ResponseType result = (ResponseType)dialog.Run ();
			if (result == ResponseType.Ok) {
				preferences.DownloadLocation = dialog.CurrentFolder;
				setFieldsFromModel (preferences);
			}

			dialog.Destroy ();
		}

		protected void buttonCacheLocationClick (object sender, EventArgs e)
		{
			// Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
			FileChooserDialog dialog = new FileChooserDialog ("Select folder to save the cached thumbnails:", 
				                           null, FileChooserAction.SelectFolder);

			var preferences = getModelFromFields ();

			dialog.SetCurrentFolder (preferences.CacheLocation);

			dialog.AddButton (Stock.Cancel, ResponseType.Cancel);
			dialog.AddButton (Stock.Ok, ResponseType.Ok);

			ResponseType result = (ResponseType)dialog.Run ();
			if (result == ResponseType.Ok) {
				preferences.CacheLocation = dialog.CurrentFolder;
				setFieldsFromModel (preferences);
			}

			dialog.Destroy ();
		}

		protected void buttonEmptyCacheClick (object sender, EventArgs e)
		{
			ResponseType result = MessageBox.Show (this, "Are you sure you want to empty the cache folder?",
				                         ButtonsType.YesNo, MessageType.Question);
			if (result == ResponseType.Yes) {
				_presenter.EmptyCacheDirectory (Preferences.CacheLocation);
				SetCacheSize ();
			}
		}
	}
}

