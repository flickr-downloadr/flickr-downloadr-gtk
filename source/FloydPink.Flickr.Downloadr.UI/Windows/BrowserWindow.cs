using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.UI.Widgets;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Windows
{
  public partial class BrowserWindow : BaseWindow, IBrowserView
  {
    private readonly IBrowserPresenter _presenter;
    private IEnumerable<Photo> _photos;
    private SpinnerWidget _spinner;

    public BrowserWindow(Session session)
    {
      Log.Debug("ctor");
      Build();

      AddTooltips();

      photosGrid.OnSelectionChanged += OnSelectionChanged;

      Title += VersionHelper.GetVersionString();

      User = session.User;
      Preferences = session.Preferences;
      CurrentPhotoset = session.SelectedPhotoset;
      CurrentAlbumPage = session.CurrentAlbumPageNumber;

      AllSelectedPhotos = new Dictionary<string, Dictionary<string, Photo>>();

      AddSpinnerWidget();

      _presenter = Bootstrapper.GetPresenter<IBrowserView, IBrowserPresenter>(this);
      _presenter.InitializePhotoset();
    }

    public int SelectedPhotosCount
    {
      get
      {
        return AllSelectedPhotos.Values.SelectMany(d => d.Values).Count();
      }
    }

    public string SelectedPhotosCountText
    {
      get
      {
        var selectionCount = SelectedPhotosExist
          ? SelectedPhotosCount.ToString(CultureInfo.InvariantCulture)
          : string.Empty;
        return string.IsNullOrEmpty(selectionCount)
          ? "Selection"
          : string.Format("Selection ({0})", selectionCount);
      }
    }

    public bool SelectedPhotosExist
    {
      get
      {
        return SelectedPhotosCount != 0;
      }
    }

    public bool AreAnyPagePhotosSelected
    {
      get
      {
        return (Page != null) && AllSelectedPhotos.ContainsKey(Page) && (AllSelectedPhotos[Page].Count != 0);
      }
    }

    public bool AreAllPagePhotosSelected
    {
      get
      {
        return (Photos != null) &&
               (!AllSelectedPhotos.ContainsKey(Page) || (Photos.Count() != AllSelectedPhotos[Page].Count));
      }
    }

    public string FirstPhoto
    {
      get
      {
        return ((int.Parse(Page) - 1)*int.Parse(PerPage) + 1).
          ToString(CultureInfo.InvariantCulture);
      }
    }

    public string LastPhoto
    {
      get
      {
        var maxLast = int.Parse(Page)*int.Parse(PerPage);
        return maxLast > int.Parse(Total) ? Total : maxLast.ToString(CultureInfo.InvariantCulture);
      }
    }

    public User User { get; set; }
    public Preferences Preferences { get; set; }

    public IEnumerable<Photo> Photos
    {
      get
      {
        return _photos;
      }
      set
      {
        _photos = value;
        UpdateUI();
        Application.Invoke(delegate
        {
          photosGrid.DoNotFireSelectionChanged = true;
          SelectAlreadySelectedPhotos();
          photosGrid.DoNotFireSelectionChanged = false;
        });
      }
    }

    public IDictionary<string, Dictionary<string, Photo>> AllSelectedPhotos { get; set; }
    public Photoset CurrentPhotoset { get; set; }
    public int CurrentAlbumPage { get; set; }
    public string Page { get; set; }
    public string Pages { get; set; }
    public string PerPage { get; set; }
    public string Total { get; set; }

    public override void ShowSpinner(bool show)
    {
      Log.Debug("ShowSpinner");
      Application.Invoke(delegate
      {
        hboxButtons.Sensitive = !show;
        scrolledwindowPhotos.Visible = !show;
        _spinner.Visible = show;
      });
    }

    public void UpdateProgress(string percentDone, string operationText, bool cancellable)
    {
      Log.Debug("UpdateProgress");
      Application.Invoke(delegate
      {
        _spinner.Cancellable = cancellable;
        _spinner.Operation = operationText;
        _spinner.PercentDone = percentDone;
      });
    }

    public bool ShowWarning(string warningMessage)
    {
      Log.Debug("ShowWarning");
      var result = MessageBox.Show(this, warningMessage, ButtonsType.YesNo, MessageType.Question);
      return result != ResponseType.Yes;
    }

    public void DownloadComplete(string downloadedLocation, bool downloadComplete)
    {
      Log.Debug("DownloadComplete");
      Application.Invoke(delegate
      {
        var message = downloadComplete
          ? "Download completed to the directory"
          : "Incomplete download could be found at";
        if (downloadComplete)
        {
          ClearSelectedPhotos();
        }
        MessageBox.Show(this,
          string.Format("{0}: {1}{1}{2}", message, Environment.NewLine, downloadedLocation),
          ButtonsType.Ok, MessageType.Info);
      });
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs args)
    {
      Log.Debug("OnDeleteEvent");
      MainClass.Quit();
      args.RetVal = true;
    }

    private void AddTooltips()
    {
      Log.Debug("AddTooltips");
      buttonBack.TooltipText = "Close this window and go back to the photoset selection window";
      buttonSelectAll.TooltipText = "Select all the photos on this page";
      buttonUnSelectAll.TooltipText = "Deselect all the photos on this page";
      buttonFirstPage.TooltipText = "Go to the first page of photos";
      buttonPreviousPage.TooltipText = "Go to the previous page of photos";
      comboboxPage.TooltipText = "Select a page to quickly jump there";
      buttonNextPage.TooltipText = "Go to the next page of photos";
      buttonLastPage.TooltipText = "Go the last page of photos";
      buttonDownloadSelection.TooltipText = "Download the selected photos from all pages";
      buttonDownloadThisPage.TooltipText = "Download all the photos from this page";
      buttonDownloadAllPages.TooltipText = "Download all the photos from all the pages";
    }

    private void AddSpinnerWidget()
    {
      Log.Debug("AddSpinnerWidget");
      _spinner = new SpinnerWidget
      {
        Name = "browserSpinner",
        Cancellable = true,
        Operation = "Please wait...",
        Visible = false
      };
      _spinner.SpinnerCanceled += (sender, e) =>
      {
        scrolledwindowPhotos.Visible = true;
        hboxButtons.Sensitive = true;
        _presenter.CancelDownload();
      };
      hboxSpinner.Add(_spinner);
      var spinnerSlot = (Box.BoxChild) hboxSpinner[_spinner];
      spinnerSlot.Position = 0;
      spinnerSlot.Expand = true;
    }

    private void SelectAlreadySelectedPhotos()
    {
      Log.Debug("SelectAlreadySelectedPhotos");
      if (!AllSelectedPhotos.ContainsKey(Page) || (AllSelectedPhotos[Page].Count <= 0))
      {
        return;
      }

      var photos = Photos.Where(photo => AllSelectedPhotos[Page].ContainsKey(photo.Id)).ToList();
      SelectPhotos(photos);
    }

    private void LoseFocus(Button element)
    {
      Log.Debug("LoseFocus");
      if (element.HasFocus)
      {
        Focus = buttonBack;
      }
    }

    private void ClearSelectedPhotos()
    {
      Log.Debug("ClearSelectedPhotos");
      AllSelectedPhotos.Clear();
      SetSelectionOnAllImages(false);
    }

    private void UpdateSelectionButtons()
    {
      Log.Debug("UpdateSelectionButtons");
      buttonSelectAll.Sensitive = AreAllPagePhotosSelected;
      buttonUnSelectAll.Sensitive = AreAnyPagePhotosSelected;

      buttonDownloadSelection.Label = SelectedPhotosCountText;
      buttonDownloadSelection.Sensitive = SelectedPhotosExist;
    }

    private void UpdateUI()
    {
      Log.Debug("UpdateUI");
      Application.Invoke(delegate
      {
        UpdateSelectionButtons();

        var selectedPhotosetLabelColor = CurrentPhotoset.Type == PhotosetType.Album ? "red" : "gray";
        labelSelectedPhotoset.LabelProp = string.Format("<span color=\"{0}\"><b>{1}</b></span>",
          selectedPhotosetLabelColor, CurrentPhotoset.HtmlEncodedTitle);

        labelPhotos.Markup = string.Format("<small>{0} - {1} of {2} Photos</small>",
          FirstPhoto, LastPhoto, Total);
        labelPages.Markup = string.Format("<small>{0} of {1} Pages</small>", Page, Pages);

        var pages = new ListStore(typeof(string));
        comboboxPage.Model = pages;
        Enumerable.Range(1, int.Parse(Pages)).ToList().ForEach(p => pages.AppendValues(p.ToString()));
        comboboxPage.Active = int.Parse(Page) - 1;

        buttonPreviousPage.Sensitive = buttonFirstPage.Sensitive = Page != "1";
        buttonNextPage.Sensitive = buttonLastPage.Sensitive = Page != Pages;

        buttonDownloadAllPages.Sensitive = Pages != "1";

        scrolledwindowPhotos.Vadjustment.Value = 0;

        var hasPhotos = Photos.Any();
        hbox5.Sensitive = hasPhotos;
        hboxCenter.Sensitive = hasPhotos;
        hboxRight.Sensitive = hasPhotos;
      });
      photosGrid.Items = Photos;
    }

    #region PhotoGrid

    private void OnSelectionChanged(object sender, EventArgs e)
    {
      Log.Debug("OnSelectionChanged");
      var photoWidget = (PhotoWidget) sender;

      if (!AllSelectedPhotos.ContainsKey(Page))
      {
        AllSelectedPhotos[Page] = new Dictionary<string, Photo>();
      }

      if (photoWidget.IsSelected)
      {
        AllSelectedPhotos[Page].Add(photoWidget.WidgetItem.Id, (Photo) photoWidget.WidgetItem);
      }
      else
      {
        AllSelectedPhotos[Page].Remove(photoWidget.WidgetItem.Id);
      }

      UpdateSelectionButtons();
    }

    private void SetSelectionOnAllImages(bool selected)
    {
      Log.Debug("SetSelectionOnAllImages");
      foreach (var box in photosGrid.AllItems)
      {
        var hbox = box as HBox;
        if (hbox == null)
        {
          continue;
        }
        foreach (var child in hbox.AllChildren)
        {
          var photoWidget = child as PhotoWidget;
          if (photoWidget != null)
          {
            photoWidget.IsSelected = selected;
          }
        }
      }
    }

    private void FindAndSelectPhoto(Photo photo)
    {
      Log.Debug("FindAndSelectPhoto");
      foreach (var box in photosGrid.AllItems)
      {
        var hbox = box as HBox;
        if (hbox == null)
        {
          continue;
        }
        foreach (var child in hbox.AllChildren)
        {
          var photoWidget = child as PhotoWidget;
          if ((photoWidget != null) && (photoWidget.WidgetItem.Id == photo.Id))
          {
            photoWidget.IsSelected = true;
            return;
          }
        }
      }
    }

    private void SelectPhotos(List<Photo> photos)
    {
      Log.Debug("SelectPhotos");
      foreach (var photo in photos)
      {
        FindAndSelectPhoto(photo);
      }
    }

    #endregion

    #region "Button Events"

    protected void buttonBackClick(object sender, EventArgs e)
    {
      Log.Debug("buttonBackClick");
      var landingWindow = new LandingWindow(new Session(User, Preferences, CurrentAlbumPage));
      landingWindow.Show();
      Destroy();
    }

    protected async void buttonNextPageClick(object sender, EventArgs e)
    {
      Log.Debug("buttonNextPageClick");
      LoseFocus((Button) sender);
      await _presenter.NavigateTo(PhotoOrAlbumPage.Next);
    }

    protected async void buttonLastPageClick(object sender, EventArgs e)
    {
      Log.Debug("buttonLastPageClick");
      LoseFocus((Button) sender);
      await _presenter.NavigateTo(PhotoOrAlbumPage.Last);
    }

    protected async void buttonFirstPageClick(object sender, EventArgs e)
    {
      Log.Debug("buttonFirstPageClick");
      LoseFocus((Button) sender);
      await _presenter.NavigateTo(PhotoOrAlbumPage.First);
    }

    protected async void buttonPreviousPageClick(object sender, EventArgs e)
    {
      Log.Debug("buttonPreviousPageClick");
      LoseFocus((Button) sender);
      await _presenter.NavigateTo(PhotoOrAlbumPage.Previous);
    }

    protected void buttonSelectAllClick(object sender, EventArgs e)
    {
      Log.Debug("buttonSelectAllClick");
      LoseFocus((Button) sender);
      SetSelectionOnAllImages(true);
    }

    protected void buttonUnSelectAllClick(object sender, EventArgs e)
    {
      Log.Debug("buttonUnSelectAllClick");
      LoseFocus((Button) sender);
      SetSelectionOnAllImages(false);
    }

    protected async void buttonDownloadSelectionClick(object sender, EventArgs e)
    {
      Log.Debug("buttonDownloadSelectionClick");
      LoseFocus((Button) sender);
      await _presenter.DownloadSelection();
    }

    protected async void buttonDownloadThisPageClick(object sender, EventArgs e)
    {
      Log.Debug("buttonDownloadThisPageClick");
      LoseFocus((Button) sender);
      await _presenter.DownloadThisPage();
    }

    protected async void buttonDownloadAllPagesClick(object sender, EventArgs e)
    {
      Log.Debug("buttonDownloadAllPagesClick");
      LoseFocus((Button) sender);
      await _presenter.DownloadAllPages();
    }

    protected async void comboboxPageChange(object sender, EventArgs e)
    {
      Log.Debug("comboboxPageChange");
      var jumpToPage = ((ComboBox) sender).ActiveText;
      if ((jumpToPage != null) && (jumpToPage != Page))
      {
        await _presenter.NavigateTo(int.Parse(jumpToPage));
      }
    }

    #endregion
  }
}
