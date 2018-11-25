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
  public partial class LandingWindow : BaseWindow, ILandingView
  {
    private readonly ILandingPresenter _presenter;
    private IEnumerable<Photoset> _albums;
    private Photoset _privatePhotoset;
    private Photoset _publicPhotoset;
    private bool _unselectingPrivatePhotoset;
    private bool _unselectingPublicPhotoset;
    private SpinnerWidget _spinner;

    public LandingWindow(Session session)
    {
      Log.Debug("ctor");
      Build();

      AddTooltips();

      photowidgetPublic.SelectionChanged += OnPublicSetSelectionChanged;
      photowidgetPrivate.SelectionChanged += OnPrivateSetSelectionChanged;
      albumsGrid.OnSelectionChanged += OnAlbumsSelectionChanged;

      hboxPublicPrivate.Visible = false;
      vbox1.Visible = false; // All Public Photos
      vbox3.Visible = false; // All Photos
      scrolledwindowPhotos.Visible = hboxAlbums.Visible = false; // Albums;

      Title += VersionHelper.GetVersionString();
      Preferences = session.Preferences;
      User = session.User;
      Page = session.CurrentAlbumPageNumber.ToString();

      AllSelectedAlbums = new Dictionary<string, Dictionary<string, Photoset>>();

      AddSpinnerWidget();

      _presenter = Bootstrapper.GetPresenter<ILandingView, ILandingPresenter>(this);
      _presenter.Initialize(session.CurrentAlbumPageNumber);
    }

    private Photoset SelectedPhotoset { get; set; }

    public int SelectedAlbumsCount
    {
      get
      {
        return AllSelectedAlbums.Values.SelectMany(d => d.Values).Count();
      }
    }

    public string SelectedAlbumsCountText
    {
      get
      {
        var selectionCount = SelectedAlbumsExist
          ? SelectedAlbumsCount.ToString(CultureInfo.InvariantCulture)
          : string.Empty;
        return string.IsNullOrEmpty(selectionCount)
          ? "Download selected albums"
          : string.Format("Download selected albums ({0})", selectionCount);
      }
    }

    public bool SelectedAlbumsExist
    {
      get
      {
        return SelectedAlbumsCount != 0;
      }
    }

    public bool AreAnyPageAlbumsSelected
    {
      get
      {
        return (Page != null) && AllSelectedAlbums.ContainsKey(Page) && (AllSelectedAlbums[Page].Count != 0);
      }
    }

    public bool AreAllPageAlbumsSelected
    {
      get
      {
        return (Albums != null) &&
          (!AllSelectedAlbums.ContainsKey(Page) || (Albums.Count() != AllSelectedAlbums[Page].Count));
      }
    }

    public string FirstAlbum
    {
      get
      {
        return ((int.Parse(Page) - 1)*int.Parse(PerPage) + 1).
          ToString(CultureInfo.InvariantCulture);
      }
    }

    public string LastAlbum
    {
      get
      {
        var maxLast = int.Parse(Page)*int.Parse(PerPage);
        return maxLast > int.Parse(Total) ? Total : maxLast.ToString(CultureInfo.InvariantCulture);
      }
    }

    public User User { get; set; }
    public Preferences Preferences { get; set; }
    public string Page { get; set; }
    public string Pages { get; set; }
    public string PerPage { get; set; }
    public string Total { get; set; }

    public IDictionary<string, Dictionary<string, Photoset>> AllSelectedAlbums { get; set; }

    public Photoset PublicPhotoset
    {
      get
      {
        return _publicPhotoset;
      }
      set
      {
        _publicPhotoset = value;
        photowidgetPublic.WidgetItem = value;
        vbox1.Visible = value != null;
      }
    }

    public Photoset PrivatePhotoset
    {
      get
      {
        return _privatePhotoset;
      }
      set
      {
        _privatePhotoset = value;
        photowidgetPrivate.WidgetItem = value;
        vbox3.Visible = value != null;
      }
    }

    public IEnumerable<Photoset> Albums
    {
      get
      {
        return _albums ?? new List<Photoset>();
      }
      set
      {
        _albums = value;
        UpdateUI();
        Application.Invoke(delegate
        {
          albumsGrid.DoNotFireSelectionChanged = true;
          SelectAlreadySelectedAlbums();
          albumsGrid.DoNotFireSelectionChanged = false;
        });
      }
    }

    public bool DownloadMultipleAlbums { get; set; }

    public override void ShowSpinner(bool show)
    {
      Log.Debug("ShowSpinner");
      Application.Invoke(delegate
      {
        hboxButtons1.Sensitive = hboxButtons2.Sensitive = !show;
        hboxPublicPrivate.Visible = !show;
        hboxAlbums.Visible = Albums.Any() && !show;
        scrolledwindowPhotos.Visible = Albums.Any() && !show;
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

    private void AddSpinnerWidget()
    {
      Log.Debug("AddSpinnerWidget");
      _spinner = new SpinnerWidget
      {
        Name = "landingSpinner",
        Cancellable = false,
        Operation = "Please wait...",
        Visible = false
      };
      hboxSpinner.Add(_spinner);
      var spinnerSlot = (Box.BoxChild) hboxSpinner[_spinner];
      spinnerSlot.Position = 0;
      spinnerSlot.Expand = true;
    }

    private void SelectAlreadySelectedAlbums()
    {
      Log.Debug("SelectAlreadySelectedAlbums");
      if (!AllSelectedAlbums.ContainsKey(Page) || (AllSelectedAlbums[Page].Count <= 0))
      {
        return;
      }

      var albums = Albums.Where(album => AllSelectedAlbums[Page].ContainsKey(album.Id)).ToList();
      SelectAlbums(albums);
    }

    public override void ClearSelectedPhotos()
    {
      Log.Debug("ClearSelectedPhotos");
      AllSelectedAlbums.Clear();
      SetSelectionOnAllAlbums(false);
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
      buttonBack1.TooltipText = buttonBack2.TooltipText = "Close this window and go back to the login window";
      buttonFirstPage1.TooltipText = buttonFirstPage2.TooltipText = "Go to the first page of albums";
      buttonPreviousPage1.TooltipText = buttonPreviousPage2.TooltipText = "Go to the previous page of albums";
      comboboxPage1.TooltipText = comboboxPage2.TooltipText = "Select a page to quickly jump there";
      buttonNextPage1.TooltipText = buttonNextPage2.TooltipText = "Go to the next page of albums";
      buttonLastPage1.TooltipText = buttonLastPage2.TooltipText = "Go the last page of albums";
      buttonContinue.TooltipText = "Browse and download photos from the selected photoset";
    }

    private void UpdateUI()
    {
      Log.Debug("UpdateUI");
      Application.Invoke(delegate
      {
        labelPhotos1.Markup = labelPhotos2.Markup = string.Format("<small>{0} - {1} of {2} Albums</small>",
          FirstAlbum, LastAlbum, Total);
        labelPages1.Markup = labelPages2.Markup = string.Format("<small>{0} of {1} Pages</small>", Page, Pages);

        var pages = new ListStore(typeof(string));
        comboboxPage1.Model = comboboxPage2.Model = pages;
        Enumerable.Range(1, int.Parse(Pages)).ToList().ForEach(p => pages.AppendValues(p.ToString()));
        comboboxPage1.Active = comboboxPage2.Active = int.Parse(Page) - 1;

        buttonPreviousPage1.Sensitive = buttonFirstPage1.Sensitive = buttonPreviousPage2.Sensitive = buttonFirstPage2.Sensitive = Page != "1";
        buttonNextPage1.Sensitive = buttonLastPage1.Sensitive = buttonNextPage2.Sensitive = buttonLastPage2.Sensitive = Page != Pages;

        scrolledwindowPhotos.Vadjustment.Value = 0;

        hboxCenter.Sensitive = Albums.Any();

        checkbuttonDownloadMultipleAlbums.Active = DownloadMultipleAlbums;

        hboxBottom1.Visible = !DownloadMultipleAlbums;
        hboxBottom2.Visible = DownloadMultipleAlbums;
        vbox1.Sensitive = vbox3.Sensitive = !DownloadMultipleAlbums;

        if ((SelectedPhotoset != null) && (SelectedPhotoset.Type == PhotosetType.Album))
        {
          SelectedPhotoset = null;
        }

        UpdateSelectionUI();
      });

      albumsGrid.Items = Albums;
    }

    private void OnPublicSetSelectionChanged(object sender, EventArgs e)
    {
      Log.Debug("OnPublicSetSelectionChanged");
      if (_unselectingPublicPhotoset)
      {
        return;
      }
      var photoWidget = (PhotoWidget) sender;

      if (photoWidget.IsSelected)
      {
        SelectedPhotoset = PublicPhotoset;
      }
      else
      {
        SelectedPhotoset = null;
      }
      UpdateSelectionUI();
    }

    private void OnPrivateSetSelectionChanged(object sender, EventArgs e)
    {
      Log.Debug("OnPrivateSetSelectionChanged");
      if (_unselectingPrivatePhotoset)
      {
        return;
      }
      var photoWidget = (PhotoWidget) sender;

      if (photoWidget.IsSelected)
      {
        SelectedPhotoset = PrivatePhotoset;
      }
      else
      {
        SelectedPhotoset = null;
      }
      UpdateSelectionUI();
    }

    private void OnAlbumsSelectionChanged(object sender, EventArgs e)
    {
      Log.Debug("OnAlbumsSelectionChanged");
      var photoWidget = (PhotoWidget) sender;

      if (photoWidget.IsSelected)
      {
        SelectedPhotoset = (Photoset) photoWidget.WidgetItem;
      }
      else
      {
        SelectedPhotoset = null;
      }
      UpdateSelectionUI();
    }

    private void UpdateSelectionUI()
    {
      var isPhotosetSelected = SelectedPhotoset != null;

      labelSelectedPhotoset1.Visible = isPhotosetSelected;
      buttonContinue.Sensitive = isPhotosetSelected;

      if (SelectedPhotoset == null || DownloadMultipleAlbums)
      {
        return;
      }

      var selectedPhotosetLabelColor = SelectedPhotoset.Type == PhotosetType.Album ? "red" : "gray";
      labelSelectedPhotoset1.LabelProp = isPhotosetSelected
        ? string.Format("<span color=\"{0}\"><b>{1}</b></span>",
          selectedPhotosetLabelColor, SelectedPhotoset.HtmlEncodedTitle)
        : "";

      _unselectingPublicPhotoset = true;
      photowidgetPublic.IsSelected = SelectedPhotoset.Type == PhotosetType.Public;
      _unselectingPublicPhotoset = false;

      _unselectingPrivatePhotoset = true;
      photowidgetPrivate.IsSelected = SelectedPhotoset.Type == PhotosetType.All;
      _unselectingPrivatePhotoset = false;

      albumsGrid.DoNotFireSelectionChanged = true;
      foreach (var box in albumsGrid.AllItems)
      {
        var hbox = box as HBox;
        if (hbox == null)
        {
          continue;
        }
        foreach (var child in hbox.AllChildren)
        {
          var widget = child as PhotoWidget;
          if ((widget != null) && ((SelectedPhotoset.Type != PhotosetType.Album) || (widget.WidgetItem.Id != SelectedPhotoset.Id)))
          {
            widget.IsSelected = false;
          }
        }
      }
      albumsGrid.DoNotFireSelectionChanged = false;
    }

    private void UpdateSelectionButtons()
    {
      Log.Debug("UpdateSelectionButtons");
      buttonSelectAll.Sensitive = AreAllPageAlbumsSelected;
      buttonUnSelectAll.Sensitive = AreAnyPageAlbumsSelected;

      buttonDownloadSelection.Label = SelectedAlbumsCountText;
      buttonDownloadSelection.Sensitive = SelectedAlbumsExist;
    }

    private void LoseFocus(Button element)
    {
      Log.Debug("LoseFocus");
      if (element.HasFocus)
      {
        Focus = DownloadMultipleAlbums ? buttonBack2 : buttonBack1;
      }
    }

    protected void buttonBackClick(object sender, EventArgs e)
    {
      Log.Debug("buttonBackClick");
      var loginWindow = new LoginWindow
      {
        User = User
      };
      loginWindow.Show();
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

    protected async void comboboxPageChange(object sender, EventArgs e)
    {
      Log.Debug("comboboxPageChange");
      var jumpToPage = ((ComboBox) sender).ActiveText;
      if ((jumpToPage != null) && (jumpToPage != Page))
      {
        await _presenter.NavigateTo(int.Parse(jumpToPage));
      }
      UpdateSelectionButtons();
    }

    protected void buttonContinueClick(object sender, EventArgs e)
    {
      Log.Debug("buttonContinueClick");
      var browserWindow = new BrowserWindow(new Session(User, Preferences, SelectedPhotoset, int.Parse(Page)));
      browserWindow.Show();
      Destroy();
    }

    protected void checkbuttonDownloadMultipleAlbumsToggled(object sender, EventArgs e)
    {
      DownloadMultipleAlbums = !DownloadMultipleAlbums;

      if (!DownloadMultipleAlbums) {
        AllSelectedAlbums = new Dictionary<string, Dictionary<string, Photoset>>();
      }

      if (DownloadMultipleAlbums)
      {
        albumsGrid.OnSelectionChanged -= OnAlbumsSelectionChanged;
        albumsGrid.OnSelectionChanged += OnSelectionChanged;
      }
      else
      {
        albumsGrid.OnSelectionChanged -= OnSelectionChanged;
        albumsGrid.OnSelectionChanged += OnAlbumsSelectionChanged;
      }

      UpdateUI();
      UpdateSelectionButtons();
    }

    #region PhotoGrid

    private void OnSelectionChanged(object sender, EventArgs e)
    {
      Log.Debug("OnSelectionChanged");
      var photoWidget = (PhotoWidget)sender;

      if (!AllSelectedAlbums.ContainsKey(Page))
      {
        AllSelectedAlbums[Page] = new Dictionary<string, Photoset>();
      }

      if (photoWidget.IsSelected)
      {
        AllSelectedAlbums[Page].Add(photoWidget.WidgetItem.Id, (Photoset)photoWidget.WidgetItem);
      }
      else
      {
        AllSelectedAlbums[Page].Remove(photoWidget.WidgetItem.Id);
      }

      UpdateSelectionButtons();
    }

    private void SetSelectionOnAllAlbums(bool selected)
    {
      Log.Debug("SetSelectionOnAllAlbums");
      foreach (var box in albumsGrid.AllItems)
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

    private void FindAndSelectAlbum(Photoset photoset)
    {
      Log.Debug("FindAndSelectAlbum");
      foreach (var box in albumsGrid.AllItems)
      {
        var hbox = box as HBox;
        if (hbox == null)
        {
          continue;
        }
        foreach (var child in hbox.AllChildren)
        {
          var photoWidget = child as PhotoWidget;
          if ((photoWidget != null) && (photoWidget.WidgetItem.Id == photoset.Id))
          {
            photoWidget.IsSelected = true;
            return;
          }
        }
      }
    }

    private void SelectAlbums(List<Photoset> photosets)
    {
      Log.Debug("SelectAlbums");
      foreach (var photoset in photosets)
      {
        FindAndSelectAlbum(photoset);
      }
    }

    #endregion

    #region "Button Events"

    protected void buttonSelectAllClick(object sender, EventArgs e)
    {
      Log.Debug("buttonSelectAllClick");
      LoseFocus((Button)sender);
      SetSelectionOnAllAlbums(true);
    }

    protected void buttonUnSelectAllClick(object sender, EventArgs e)
    {
      Log.Debug("buttonUnSelectAllClick");
      LoseFocus((Button)sender);
      SetSelectionOnAllAlbums(false);
    }

    protected async void buttonDownloadSelectionClick(object sender, EventArgs e)
    {
      Log.Debug("buttonDownloadSelectionClick");
      LoseFocus((Button)sender);
      await _presenter.DownloadSelection();
    }

    #endregion

  }
}
