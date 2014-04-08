using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Gtk;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Model.Extensions;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.Helpers;

namespace FloydPink.Flickr.Downloadr
{
	public partial class BrowserWindow : Window, IBrowserView
	{
		private readonly IBrowserPresenter _presenter;
		private bool _doNotSyncSelectedItems;
		private string _page;
		private string _pages;
		private string _perPage;
		private IEnumerable<Photo> _photos;
		private string _total;

		public BrowserWindow (User user, Preferences preferences) : 
			base (WindowType.Toplevel)
		{
			this.Build ();

			Title += VersionHelper.GetVersionString ();
			Preferences = preferences;
			User = user;
			AllSelectedPhotos = new Dictionary<string, Dictionary<string, Photo>> ();

//			PagePhotoList.SelectionChanged += (sender, args) =>
//			{
//				if (_doNotSyncSelectedItems) return;
//				AllSelectedPhotos[Page] = PagePhotoList.SelectedItems.Cast<Photo>().
//					ToDictionary(p => p.Id, p => p);
//				PropertyChanged.Notify(() => SelectedPhotosExist);
//				PropertyChanged.Notify(() => SelectedPhotosCountText);
//				PropertyChanged.Notify(() => AreAnyPagePhotosSelected);
//				PropertyChanged.Notify(() => AreAllPagePhotosSelected);
//			};
//
//			SpinnerInner.SpinnerCanceled += (sender, args) => _presenter.CancelDownload();
//
//			FileCache.AppCacheDirectory = Preferences.CacheLocation;

			labelPhotos.Markup = "<small>                       </small>";
			labelPages.Markup = "<small>                       </small>";

			_presenter = Bootstrapper.GetPresenter<IBrowserView, IBrowserPresenter> (this);
			_presenter.InitializePhotoset ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs args)
		{
			Application.Quit ();
			args.RetVal = true;
		}

		public int SelectedPhotosCount {
			get { return AllSelectedPhotos.Values.SelectMany (d => d.Values).Count (); }
		}

		public string SelectedPhotosCountText {
			get {
				string selectionCount = SelectedPhotosExist
					? SelectedPhotosCount.ToString (CultureInfo.InvariantCulture)
					: string.Empty;
				return string.IsNullOrEmpty (selectionCount)
					? "Selection"
						: string.Format ("Selection ({0})", selectionCount);
			}
		}

		public bool SelectedPhotosExist {
			get { return SelectedPhotosCount != 0; }
		}

		public bool AreAnyPagePhotosSelected {
			get { return Page != null && AllSelectedPhotos.ContainsKey (Page) && AllSelectedPhotos [Page].Count != 0; }
		}

		public bool AreAllPagePhotosSelected {
			get {
				return Photos != null &&
				(!AllSelectedPhotos.ContainsKey (Page) || Photos.Count () != AllSelectedPhotos [Page].Count);
			}
		}

		public string FirstPhoto {
			get {
				return (((Convert.ToInt32 (Page) - 1) * Convert.ToInt32 (PerPage)) + 1).
					ToString (CultureInfo.InvariantCulture);
			}
		}

		public string LastPhoto {
			get {
				int maxLast = Convert.ToInt32 (Page) * Convert.ToInt32 (PerPage);
				return maxLast > Convert.ToInt32 (Total) ? Total : maxLast.ToString (CultureInfo.InvariantCulture);
			}
		}

		public User User { get; set; }

		public Preferences Preferences { get; set; }

		public IEnumerable<Photo> Photos {
			get { return _photos; }
			set {
				_photos = value;
				PropertyChanged.Notify (() => AreAllPagePhotosSelected);
				Application.Invoke (delegate {
					_doNotSyncSelectedItems = true;
					UpdateUI ();
					SelectAlreadySelectedPhotos ();
					_doNotSyncSelectedItems = false;
				});
			}
		}

		public IDictionary<string, Dictionary<string, Photo>> AllSelectedPhotos { get; set; }

		public bool ShowAllPhotos {
			get { return togglebuttonShowAllPhotos.Active; }
		}

		public string Page {
			get { return _page; }
			set {
				_page = value;
				PropertyChanged.Notify (() => Page);
				PropertyChanged.Notify (() => AreAnyPagePhotosSelected);
			}
		}

		public string Pages {
			get { return _pages; }
			set {
				_pages = value;
				PropertyChanged.Notify (() => Pages);
			}
		}

		public string PerPage {
			get { return _perPage; }
			set {
				_perPage = value;
				PropertyChanged.Notify (() => PerPage);
			}
		}

		public string Total {
			get { return _total; }
			set {
				_total = value;
				PropertyChanged.Notify (() => Total);
				PropertyChanged.Notify (() => FirstPhoto);
				PropertyChanged.Notify (() => LastPhoto);
			}
		}

		public void ShowSpinner (bool show)
		{
//			Visibility visibility = show ? Visibility.Visible : Visibility.Collapsed;
//			Spinner.Dispatch(s => s.Visibility = visibility);
		}

		public void UpdateProgress (string percentDone, string operationText, bool cancellable)
		{
//			SpinnerInner.Dispatch(sc => sc.Cancellable = cancellable);
//			SpinnerInner.Dispatch(sc => sc.OperationText = operationText);
//			SpinnerInner.Dispatch(sc => sc.PercentDone = percentDone);
		}

		public bool ShowWarning (string warningMessage)
		{
			ResponseType result = MessageBox.Show (this, warningMessage, ButtonsType.YesNo, MessageType.Question);
			return result != ResponseType.Yes;
		}

		public void DownloadComplete (string downloadedLocation, bool downloadComplete)
		{
			const string proTip = "\r\n\r\n(ProTip™: CTRL+C will copy all of this message with the location.)";
			if (downloadComplete) {
				ClearSelectedPhotos ();
				MessageBox.Show (this,
					string.Format ("Download completed to the directory: \r\n{0}{1}",
						downloadedLocation, proTip), ButtonsType.Ok, MessageType.Info);
			} else {
				MessageBox.Show (this,
					string.Format ("Incomplete download could be found at: \r\n{0}{1}",
						downloadedLocation, proTip), ButtonsType.Ok, MessageType.Info);
			}
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void SelectAlreadySelectedPhotos ()
		{
			if (!AllSelectedPhotos.ContainsKey (Page) || AllSelectedPhotos [Page].Count <= 0)
				return;

			List<Photo> photos = Photos.Where (photo => AllSelectedPhotos [Page].ContainsKey (photo.Id)).ToList ();
			SelectPhotos (photos);
		}

		private async void DownloadSelectionButtonClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			await _presenter.DownloadSelection ();
		}

		private async void DownloadThisPageButtonClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			await _presenter.DownloadThisPage ();
		}

		private async void DownloadAllPagesButtonClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			await _presenter.DownloadAllPages ();
		}

		private void LoseFocus (Button element)
		{
			if (element.HasFocus) {
				this.Focus = buttonBack;
			}
		}

		private void ClearSelectedPhotos ()
		{
			AllSelectedPhotos.Clear ();
			SetSelectionOnAllImages (false);
			PropertyChanged.Notify (() => SelectedPhotosExist);
			PropertyChanged.Notify (() => SelectedPhotosCountText);
		}

		void UpdateUI ()
		{
			labelPhotos.Markup = string.Format ("<small>{0} - {1} of {2} Photos</small>", 
				FirstPhoto, LastPhoto, Total);
			labelPages.Markup = string.Format ("<small>{0} of {1} Pages</small>", Page, Pages);
			buttonPreviousPage.Sensitive = buttonFirstPage.Sensitive = Page != "1";
			buttonNextPage.Sensitive = buttonLastPage.Sensitive = Page != Pages;
			SetupTheImageGrid (Photos);
		}

		protected void buttonBackClick (object sender, EventArgs e)
		{
			var loginWindow = new LoginWindow { User = User };
			loginWindow.Show ();
			Destroy ();
		}

		protected async void buttonNextPageClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			await _presenter.NavigateTo (PhotoPage.Next);
		}

		protected async void buttonLastPageClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			await _presenter.NavigateTo (PhotoPage.Last);
		}

		protected async void buttonFirstPageClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			await _presenter.NavigateTo (PhotoPage.First);
		}

		protected async void buttonPreviousPageClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			await _presenter.NavigateTo (PhotoPage.Previous);
		}

		protected void buttonSelectAllClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			SetSelectionOnAllImages (true);
		}

		protected void buttonUnSelectAllClick (object sender, EventArgs e)
		{
			LoseFocus ((Button)sender);
			SetSelectionOnAllImages (false);
		}

		protected async void togglebuttonShowAllPhotosClick (object sender, EventArgs e)
		{
			var toggleButton = (ToggleButton)sender;
			toggleButton.Label = toggleButton.Active ? "Show Only Public Photos" : "Show All Photos";

			LoseFocus ((Button)sender);
			ClearSelectedPhotos ();
			await _presenter.InitializePhotoset ();
		}
	}
}

