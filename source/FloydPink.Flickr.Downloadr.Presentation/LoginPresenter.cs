using System;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Presentation.Views;

namespace FloydPink.Flickr.Downloadr.Presentation
{
  public class LoginPresenter : PresenterBase, ILoginPresenter
  {
    private readonly ILoginLogic _logic;
    private readonly IPreferencesLogic _preferencesLogic;
    private readonly IUpdateCheckLogic _updateCheckLogic;
    private readonly ILoginView _view;
    private Preferences _preferences;

    public LoginPresenter(ILoginView view, ILoginLogic logic, IPreferencesLogic preferencesLogic,
      IUpdateCheckLogic updateCheckLogic)
    {
      _view = view;
      _logic = logic;
      _preferencesLogic = preferencesLogic;
      _updateCheckLogic = updateCheckLogic;
    }

    public async void InitializeScreen()
    {
      try
      {
        _view.ShowSpinner(true);
        _view.ShowLoggedOutControl();
        _preferences = _preferencesLogic.GetPreferences();
        if (!await _logic.IsUserLoggedInAsync(ApplyUser))
        {
          Logout();
        }
      } catch (Exception ex)
      {
        _view.HandleException(ex);
      }
    }

    public void Login()
    {
      try
      {
        _view.ShowSpinner(true);
        _logic.Login(ApplyUser);
      } catch (Exception ex)
      {
        _view.HandleException(ex);
      }
    }

    public void Logout()
    {
      _logic.Logout();
      if (_preferences != null)
      {
        _preferencesLogic.EmptyCacheDirectory(_preferences.CacheLocation);
      }
      _preferences = null;
      _view.ShowSpinner(false);
      _view.ShowLoggedOutControl();
    }

    public void Continue()
    {
      if (_preferences != null)
      {
        _view.OpenLandingWindow();
      }
      else
      {
        _view.OpenPreferencesWindow(Preferences.GetDefault());
      }
    }

    private void ApplyUser(User user)
    {
      if (_preferences != null)
      {
        if (_preferences.CheckForUpdates)
        {
          var update = _updateCheckLogic.UpdateAvailable(_preferences);
          if (update.Available)
          {
            _view.ShowUpdateAvailableNotification(update.LatestVersion);
          }
        }
      }
      _view.ShowLoggedInControl(_preferences);
      _view.User = user;
      _view.ShowSpinner(false);
    }
  }
}
