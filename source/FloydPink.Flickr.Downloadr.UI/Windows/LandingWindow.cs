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
    private SpinnerWidget spinner;

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
      scrolledwindowPhotos.Visible = labelSets.Visible = false; // Albums;

      Title += VersionHelper.GetVersionString();
      Preferences = session.Preferences;
      User = session.User;
      Page = session.CurrentAlbumPageNumber.ToString();

      AddSpinnerWidget();

      _presenter = Bootstrapper.GetPresenter<ILandingView, ILandingPresenter>(this);
      _presenter.Initialize(session.CurrentAlbumPageNumber);
    }

    private Photoset SelectedPhotoset { get; set; }

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
      }
    }

    public override void ShowSpinner(bool show)
    {
      Log.Debug("ShowSpinner");
      Application.Invoke(delegate
      {
        hboxButtons.Sensitive = !show;
        hboxPublicPrivate.Visible = !show;
        labelSets.Visible = Albums.Any() && !show;
        scrolledwindowPhotos.Visible = Albums.Any() && !show;
        spinner.Visible = show;
      });
    }

    public void UpdateProgress(string percentDone, string operationText, bool cancellable)
    {
      Log.Debug("UpdateProgress");
      Application.Invoke(delegate { spinner.Operation = operationText; });
    }

    private void AddSpinnerWidget()
    {
      Log.Debug("AddSpinnerWidget");
      spinner = new SpinnerWidget
      {
        Name = "landingSpinner",
        Cancellable = false,
        Operation = "Please wait...",
        Visible = false
      };
      hboxSpinner.Add(spinner);
      var spinnerSlot = (Box.BoxChild) hboxSpinner[spinner];
      spinnerSlot.Position = 0;
      spinnerSlot.Expand = true;
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
      buttonBack.TooltipText = "Close this window and go back to the login window";
      buttonFirstPage.TooltipText = "Go to the first page of albums";
      buttonPreviousPage.TooltipText = "Go to the previous page of albums";
      comboboxPage.TooltipText = "Select a page to quickly jump there";
      buttonNextPage.TooltipText = "Go to the next page of albums";
      buttonLastPage.TooltipText = "Go the last page of albums";
      buttonContinue.TooltipText = "Browse and download photos from the selected photoset";
    }

    private void UpdateUI()
    {
      Log.Debug("UpdateUI");
      Application.Invoke(delegate
      {
        labelPhotos.Markup = string.Format("<small>{0} - {1} of {2} Albums</small>",
          FirstAlbum, LastAlbum, Total);
        labelPages.Markup = string.Format("<small>{0} of {1} Pages</small>", Page, Pages);

        var pages = new ListStore(typeof(string));
        comboboxPage.Model = pages;
        Enumerable.Range(1, int.Parse(Pages)).ToList().ForEach(p => pages.AppendValues(p.ToString()));
        comboboxPage.Active = int.Parse(Page) - 1;

        buttonPreviousPage.Sensitive = buttonFirstPage.Sensitive = Page != "1";
        buttonNextPage.Sensitive = buttonLastPage.Sensitive = Page != Pages;

        scrolledwindowPhotos.Vadjustment.Value = 0;

        hboxCenter.Sensitive = Albums.Any();

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

      labelSelectedPhotoset.Visible = isPhotosetSelected;
      buttonContinue.Sensitive = isPhotosetSelected;

      if (SelectedPhotoset == null)
      {
        return;
      }

      var selectedPhotosetLabelColor = SelectedPhotoset.Type == PhotosetType.Album ? "red" : "gray";
      labelSelectedPhotoset.LabelProp = isPhotosetSelected
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

    private void LoseFocus(Button element)
    {
      Log.Debug("LoseFocus");
      if (element.HasFocus)
      {
        Focus = buttonBack;
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
    }

    protected void buttonContinueClick(object sender, EventArgs e)
    {
      Log.Debug("buttonContinueClick");
      var browserWindow = new BrowserWindow(new Session(User, Preferences, SelectedPhotoset, int.Parse(Page)));
      browserWindow.Show();
      Destroy();
    }
  }
}
