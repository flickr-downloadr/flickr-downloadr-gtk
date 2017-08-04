using System;
using System.Collections.Generic;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.CachedImage;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Windows
{
  public partial class PreferencesWindow : BaseWindow, IPreferencesView
  {
    private readonly IPreferencesPresenter _presenter;
    private Preferences _preferences;

    public PreferencesWindow(Session session)
    {
      Log.Debug("ctor");
      Build();

      AddTooltips();

      Title += VersionHelper.GetVersionString();
      Preferences = session.Preferences;
      User = session.User;

      _presenter = Bootstrapper.GetPresenter<IPreferencesView, IPreferencesPresenter>(this);

      SetCacheSize();
    }

    private User User { get; }

    public Preferences Preferences
    {
      get
      {
        return _preferences;
      }
      set
      {
        _preferences = value;
        setFieldsFromModel(Preferences);
      }
    }

    public override void ShowSpinner(bool show)
    {
      //			Visibility visibility = show ? Visibility.Visible : Visibility.Collapsed;
      //			Spinner.Dispatch(s => s.Visibility = visibility);
    }

    private void OnDeleteEvent(object sender, DeleteEventArgs args)
    {
      Log.Debug("OnDeleteEvent");
      MainClass.Quit();
      args.RetVal = true;
    }

    private void AddTooltips()
    {
      Log.Debug("AddTooltips");
      labelFilename.TooltipText = radioPhotoId.TooltipText = radioPhotoTitle.TooltipText = radioOriginalOrder.TooltipText =
        "Choose to name the downloaded photos with its internal photo id (a unique number) or its order as defined inside the album or its title (Untitled images will be assigned random unique names)";
      labelDownloadLocation.TooltipText = entryDownloadLocation.TooltipText = buttonDownloadLocation.TooltipText =
        "Set the location to save the downloaded photos and metadata";
      labelAlbumSearchName.TooltipText = entryAlbumSearchName.TooltipText = 
        "Search for albums containing this text in their name";
      labelDownloadSize.TooltipText = comboboxDownloadSize.TooltipText =
        "Set the size of the photos to download";
      labelMetadata.TooltipText =
        checkbuttonTags.TooltipText = checkbuttonDescription.TooltipText = checkbuttonTitle.TooltipText =
          "Select the attributes of the metadata to be downloaded";
      labelPhotosPerPage.TooltipText = comboboxPhotosPerPage.TooltipText =
        "Set the number of photos to be displayed in a page on the browser window";
      labelSafetyLevel.TooltipText = comboboxSafetyLevel.TooltipText =
        "Set the safety level of the photos to be downloaded";
      labelTags.TooltipText = radioTagsInternal.TooltipText = radioTagsOriginal.TooltipText =
        "Choose the type of tags to be downloaded - internal tags does not preserve the space, original will be exactly as it were entered";
      labelCacheLocation.TooltipText = entryCacheLocation.TooltipText = buttonCacheLocation.TooltipText =
        "Set the location to save the cached copy of the thumbnails and preview images";
      labelCacheSize.TooltipText = labelCacheSizeValue.TooltipText =
        "Amount of space taken by the current cache folder";
      buttonEmptyCache.TooltipText = "Empty the cache folder if it is taking up too much space";
      buttonCancel.TooltipText =
        "Revert all the settings to their last saved values and go back to the login window";
      buttonDefaults.TooltipText = "Reset all the settings to their default values";
      buttonSave.TooltipText = "Save all the settings and continue to the photoset selection window";
      labelUpdate.TooltipText = checkbuttonUpdate.TooltipText =
        "Get notified automatically when there is an updated version available";
      labelLogLevel.TooltipText = comboboxLogLevel.TooltipText =
        "Set the level of diagnostic logging (Recommended: Off)";
      labelLogLocation.TooltipText = entryLogLocation.TooltipText = buttonLogLocation.TooltipText =
        "Set the location to save the log files with diagnostic information";
      labelRestartRequired.TooltipText = "The settings on the fields with red asterisk above, will work better after a restart";
    }

    private void SetCacheSize()
    {
      Log.Debug("SetCacheSize");
      labelCacheSizeValue.Text = _presenter.GetCacheFolderSize(Preferences.CacheLocation);
      buttonEmptyCache.Visible =
        !((labelCacheSizeValue.Text == "0 B") || (labelCacheSizeValue.Text == "-"));
    }

    private void setFieldsFromModel(Preferences preferences)
    {
      Log.Debug("setFieldsFromModel");
      // Filename
      radioPhotoId.Active = preferences.FileNameMode == FileNameMode.PhotoId;
      radioPhotoTitle.Active = preferences.FileNameMode == FileNameMode.Title;
      radioOriginalOrder.Active = preferences.FileNameMode == FileNameMode.OriginalOrder;

      entryAlbumSearchName.Text = preferences.AlbumSearchName;

      // Download location
      entryDownloadLocation.Text = preferences.DownloadLocation;

      // Download size
      comboboxDownloadSize.Active = (int) preferences.DownloadSize;

      // Metadata
      checkbuttonTags.Active = preferences.Metadata.Contains(PhotoMetadata.Tags);
      checkbuttonDescription.Active = preferences.Metadata.Contains(PhotoMetadata.Description);
      checkbuttonTitle.Active = preferences.Metadata.Contains(PhotoMetadata.Title);

      // Photos per page
      var photosPerPageMap = new Dictionary<string, int>
      {
        {
          "25", 0
        },
        {
          "50", 1
        },
        {
          "75", 2
        },
        {
          "100", 3
        }
      };
      comboboxPhotosPerPage.Active = photosPerPageMap[preferences.PhotosPerPage.ToString()];

      //Safety level
      comboboxSafetyLevel.Active = int.Parse(preferences.SafetyLevel) - 1;

      // Tags
      radioTagsInternal.Active = !preferences.NeedOriginalTags;
      radioTagsOriginal.Active = preferences.NeedOriginalTags;

      // Cache location
      entryCacheLocation.Text = preferences.CacheLocation;

      // Check for Update
      checkbuttonUpdate.Active = preferences.CheckForUpdates;

      // Log Level
      comboboxLogLevel.Active = (int) preferences.LogLevel;

      // Log location
      entryLogLocation.Text = preferences.LogLocation;
    }

    private Preferences getModelFromFields()
    {
      Log.Debug("getModelFromFields");
      var metadata = new List<string>();
      if (checkbuttonTags.Active)
      {
        metadata.Add(PhotoMetadata.Tags);
      }
      if (checkbuttonDescription.Active)
      {
        metadata.Add(PhotoMetadata.Description);
      }
      if (checkbuttonTitle.Active)
      {
        metadata.Add(PhotoMetadata.Title);
      }
      return new Preferences
      {

        FileNameMode =
        radioPhotoTitle.Active ? FileNameMode.Title :
        radioOriginalOrder.Active ? FileNameMode.OriginalOrder : FileNameMode.PhotoId,

        DownloadLocation = entryDownloadLocation.Text,
        AlbumSearchName = entryAlbumSearchName.Text,
        DownloadSize =
          (PhotoDownloadSize) Enum.Parse(typeof(PhotoDownloadSize), comboboxDownloadSize.ActiveText),
        Metadata = metadata,
        PhotosPerPage = int.Parse(comboboxPhotosPerPage.ActiveText),
        SafetyLevel = (comboboxSafetyLevel.Active + 1).ToString(),
        NeedOriginalTags = radioTagsOriginal.Active,
        CacheLocation = entryCacheLocation.Text,
        CheckForUpdates = checkbuttonUpdate.Active,
        LogLevel =
          (LogLevel) Enum.Parse(typeof(LogLevel), comboboxLogLevel.ActiveText),
        LogLocation = entryLogLocation.Text
      };
    }

    private void buttonCancelClick(object sender, EventArgs e)
    {
      Log.Debug("buttonCancelClick");
      var loginWindow = new LoginWindow(User);
      loginWindow.Show();
      Destroy();
    }

    private void buttonDefaultsClick(object sender, EventArgs e)
    {
      Log.Debug("buttonDefaultsClick");
      Preferences = Preferences.GetDefault();

      Preferences.Visited = true;
    }

    private void buttonSaveClick(object sender, EventArgs e)
    {
      Log.Debug("buttonSaveClick");
      var preferences = getModelFromFields();

      FileCache.AppCacheDirectory = preferences.CacheLocation;

      Bootstrapper.ReconfigureLogging(preferences.LogLevel.ToString(), preferences.LogLocation);

      _presenter.Save(preferences);

      preferences.Visited = true;

      var landingWindow = new LandingWindow(new Session(User, preferences));
      landingWindow.Show();
      Destroy();
    }

    // TODO: Refactor the below three event handlers that open a select folder dialog, to avoid duplication!
    private void buttonDownloadLocationClick(object sender, EventArgs e)
    {
      Log.Debug("buttonDownloadLocationClick");
      // Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
      var dialog = new FileChooserDialog("Select folder to save downloaded photos:",
        null, FileChooserAction.SelectFolder);

      var preferences = getModelFromFields();

      dialog.SetCurrentFolder(preferences.DownloadLocation);

      dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
      dialog.AddButton(Stock.Ok, ResponseType.Ok);

      var result = (ResponseType) dialog.Run();
      if (result == ResponseType.Ok)
      {
        preferences.DownloadLocation = dialog.CurrentFolder;
        setFieldsFromModel(preferences);
      }

      dialog.Destroy();
    }

    private void buttonCacheLocationClick(object sender, EventArgs e)
    {
      Log.Debug("buttonCacheLocationClick");
      // Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
      var dialog = new FileChooserDialog("Select folder to save the cached thumbnails:",
        null, FileChooserAction.SelectFolder);

      var preferences = getModelFromFields();

      dialog.SetCurrentFolder(preferences.CacheLocation);

      dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
      dialog.AddButton(Stock.Ok, ResponseType.Ok);

      var result = (ResponseType) dialog.Run();
      if (result == ResponseType.Ok)
      {
        Preferences.CacheLocation = preferences.CacheLocation = dialog.CurrentFolder;
        setFieldsFromModel(preferences);
        SetCacheSize();
      }

      dialog.Destroy();
    }

    private void buttonEmptyCacheClick(object sender, EventArgs e)
    {
      Log.Debug("buttonEmptyCacheClick");
      var result = MessageBox.Show(this, "Are you sure you want to empty the cache folder?",
        ButtonsType.YesNo, MessageType.Question);
      if (result == ResponseType.Yes)
      {
        _presenter.EmptyCacheDirectory(Preferences.CacheLocation);
        SetCacheSize();
      }
    }

    private void buttonLogLocationClick(object sender, EventArgs e)
    {
      Log.Debug("buttonLogLocationClick");
      // Thanks Petteri Kautonen - http://mono.1490590.n4.nabble.com/Gtk-sharp-list-FileOpenDialog-td1544553.html
      var dialog = new FileChooserDialog("Select folder to save the log files:",
        null, FileChooserAction.SelectFolder);

      var preferences = getModelFromFields();

      dialog.SetCurrentFolder(preferences.LogLocation);

      dialog.AddButton(Stock.Cancel, ResponseType.Cancel);
      dialog.AddButton(Stock.Ok, ResponseType.Ok);

      var result = (ResponseType) dialog.Run();
      if (result == ResponseType.Ok)
      {
        Preferences.LogLocation = preferences.LogLocation = dialog.CurrentFolder;
        setFieldsFromModel(preferences);
      }

      dialog.Destroy();
    }
  }
}
