using System;
using System.Diagnostics;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Presentation;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.CachedImage;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.UI.Widgets;
using Gtk;
using Mono.Unix;

namespace FloydPink.Flickr.Downloadr.UI.Windows
{
  public partial class LoginWindow : BaseWindow, ILoginView
  {
    private readonly ILoginPresenter _presenter;
    private User _user;
    private SpinnerWidget spinner;

    public LoginWindow()
      : this(new User()) {}

    public LoginWindow(User user)
    {
      Log.Debug("ctor");
      Build();

      AddTooltips();

      Title += VersionHelper.GetVersionString();
      User = user;

      AddSpinnerControl();

      _presenter = Bootstrapper.GetPresenter<ILoginView, ILoginPresenter>(this);
      _presenter.InitializeScreen();
    }

    public override void ClearSelectedPhotos()
    {
      throw new NotImplementedException();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs args)
    {
      Log.Debug("OnDeleteEvent");
      MainClass.Quit();
      args.RetVal = true;
    }

    private void SetWelcomeLabel(User user)
    {
      Log.Debug("SetWelcomeLabel");
      Application.Invoke(delegate
      {
        var welcomeMessage = string.IsNullOrEmpty(user.UserNsId)
          ? string.Empty
          : user.WelcomeMessage;
        labelWelcomeUsername.LabelProp = string.Format("<b><big>{0}</big></b>",
          welcomeMessage);
        if (user.Info == null)
        {
          return;
        }
        imageBuddyIcon.SetCachedImage(user.Info.BuddyIconUrl);
      });
    }

    private void AddTooltips()
    {
      Log.Debug("AddTooltips");
      buttonLogin.TooltipText = "Log in to flickr using OAuth";
      buttonPrefs.TooltipText = "Update the Preferences";
      buttonLogout.TooltipText = "Log out from the currently logged in account";
      buttonContinue.TooltipText = "Browse and download the photos from the logged in account";
    }

    private void AddSpinnerControl()
    {
      Log.Debug("AddSpinnerControl");
      spinner = new SpinnerWidget
      {
        Name = "loginSpinner",
        Cancellable = true,
        Operation = "Please wait...",
        Visible = false
      };
      spinner.SpinnerCanceled +=
        (sender, e) => { Application.Invoke(delegate { hboxLogin.Sensitive = true; }); };
      vbox2.Add(spinner);

      var spinnerSlot = (Box.BoxChild) vbox2[spinner];
      spinnerSlot.Position = 0;
      spinnerSlot.Expand = true;
    }

    protected void buttonLoginClick(object sender, EventArgs e)
    {
      Log.Debug("buttonLoginClick");
      _presenter.Login();
    }

    protected void buttonLogoutClick(object sender, EventArgs e)
    {
      Log.Debug("buttonLogoutClick");
      _presenter.Logout();
    }

    protected void buttonContinueClick(object sender, EventArgs e)
    {
      Log.Debug("buttonContinueClick");
      _presenter.Continue();
    }

    protected void buttonAboutClick(object sender, EventArgs e)
    {
      Log.Debug("buttonAboutClick");
      var aboutWindow = new AboutWindow();
      aboutWindow.ShowAll();
    }

    protected void buttonPrefsClick(object sender, EventArgs e)
    {
      Log.Debug("buttonPrefsClick");
      OpenPreferencesWindow(Preferences);
    }

    #region ILoginView Members

    protected Preferences Preferences { get; set; }

    public User User
    {
      get
      {
        return _user;
      }
      set
      {
        _user = value;
        SetWelcomeLabel(value);
      }
    }

    public void ShowLoggedInControl(Preferences preferences)
    {
      Log.Debug("ShowLoggedInControl");

      if (preferences != null)
      {
        Bootstrapper.ReconfigureLogging(preferences.LogLevel.ToString(), preferences.LogLocation);
      }

      Application.Invoke(delegate
      {
        Preferences = preferences;
        FileCache.AppCacheDirectory = Preferences != null
          ? Preferences.CacheLocation
          : Preferences.GetDefault().CacheLocation;

        buttonPrefs.Visible = Preferences != null;
        hboxBottomButtons.Visible = true;
        hboxAvatar.Visible = true;

        hboxLogin.Visible = false;
        labelMessage.LabelProp =
          Catalog.GetString("Click 'Continue' to browse and download photos...");
      });
    }

    public void ShowLoggedOutControl()
    {
      Log.Debug("ShowLoggedOutControl");
      Application.Invoke(delegate
      {
        hboxLogin.Visible = true;
        labelMessage.LabelProp =
          Catalog.GetString("Welcome to flickr downloadr. Click 'Login' to continue.");

        buttonPrefs.Visible = false;
        hboxBottomButtons.Visible = false;
        hboxAvatar.Visible = false;
        hboxUpdate.Visible = true;
      });
    }

    public override void ShowSpinner(bool show)
    {
      Log.Debug("ShowSpinner");
      Application.Invoke(delegate
      {
        hboxLogin.Sensitive = !show;
        spinner.Visible = show;
      });
    }

    public void OpenLandingWindow()
    {
      Log.Debug("OpenBrowserWindow");
      var landingWindow = new LandingWindow(new Session(User, Preferences));
      landingWindow.Show();
      Destroy();
    }

    public void OpenPreferencesWindow(Preferences preferences)
    {
      Log.Debug("OpenPreferencesWindow");
      var preferencesWindow = new PreferencesWindow(new Session(User, preferences));
      preferencesWindow.Show();
      Destroy();
    }

    public void ShowUpdateAvailableNotification(string latestVersion)
    {
      Log.Debug("ShowUpdateAvailableNotification");
      Application.Invoke(delegate
      {
        labelUpdate.LabelProp =
          string.Format(
            "<a href=\"#\"><span color=\"red\">Update Available ( v{0} )</span></a>",
            latestVersion);
        labelUpdate.TooltipText =
          "Click here to download the latest version of the application";
        hboxUpdate.Visible = true;
      });
    }


    protected void UpdateNotificationClick(object o, ButtonPressEventArgs args)
    {
      Log.Debug("UpdateNotificationClick");
      Process.Start(VersionHelper.GetUpdateUrl());
      MainClass.Quit();
    }

    #endregion
  }
}
