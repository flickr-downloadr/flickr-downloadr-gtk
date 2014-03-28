using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Presentation.Views;

namespace FloydPink.Flickr.Downloadr.Presentation
{
    public class LoginPresenter : PresenterBase, ILoginPresenter
    {
        private readonly ILoginLogic _logic;
        private readonly IPreferencesLogic _preferencesLogic;
        private readonly ILoginView _view;
        private Preferences _preferences;

        public LoginPresenter(ILoginView view, ILoginLogic logic, IPreferencesLogic preferencesLogic)
        {
            _view = view;
            _logic = logic;
            _preferencesLogic = preferencesLogic;
        }

        public async void InitializeScreen()
        {
            _view.ShowSpinner(true);
            _view.ShowLoggedOutControl();
            _preferences = _preferencesLogic.GetPreferences();
            if (!await _logic.IsUserLoggedInAsync(ApplyUser))
            {
                Logout();
            }
        }

        public void Login()
        {
            _view.ShowSpinner(true);
            _logic.Login(ApplyUser);
        }

        public void Logout()
        {
            _preferences = null;
            _logic.Logout();
            _view.ShowSpinner(false);
            _view.ShowLoggedOutControl();
        }

        public void Continue()
        {
            if (_preferences != null)
            {
                _view.OpenBrowserWindow();
            }
            else
            {
                _view.OpenPreferencesWindow(Preferences.GetDefault());
            }
        }

        private void ApplyUser(User user)
        {
            _view.ShowLoggedInControl(_preferences);
            _view.User = user;
            _view.ShowSpinner(false);
        }
    }
}