using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Presentation.Views;

namespace FloydPink.Flickr.Downloadr.Presentation {
    public class LoginPresenter : PresenterBase, ILoginPresenter {
        private readonly ILoginLogic _logic;
        private readonly IPreferencesLogic _preferencesLogic;
        private readonly ILoginView _view;
        private Preferences _preferences;

        public LoginPresenter(ILoginView view, ILoginLogic logic, IPreferencesLogic preferencesLogic) {
            this._view = view;
            this._logic = logic;
            this._preferencesLogic = preferencesLogic;
        }

        public async void InitializeScreen() {
            this._view.ShowSpinner(true);
            this._view.ShowLoggedOutControl();
            this._preferences = this._preferencesLogic.GetPreferences();
            if (!await this._logic.IsUserLoggedInAsync(ApplyUser)) {
                Logout();
            }
        }

        public void Login() {
            this._view.ShowSpinner(true);
            this._logic.Login(ApplyUser);
        }

        public void Logout() {
            this._preferences = null;
            this._logic.Logout();
            this._view.ShowSpinner(false);
            this._view.ShowLoggedOutControl();
        }

        public void Continue() {
            if (this._preferences != null) {
                this._view.OpenBrowserWindow();
            } else {
                this._view.OpenPreferencesWindow(Preferences.GetDefault());
            }
        }

        private void ApplyUser(User user) {
            this._view.ShowLoggedInControl(this._preferences);
            this._view.User = user;
            this._view.ShowSpinner(false);
        }
    }
}
