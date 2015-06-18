namespace FloydPink.Flickr.Downloadr.Presentation {
    using System;
    using Logic.Interfaces;
    using Model;
    using Views;

    public class LoginPresenter : PresenterBase, ILoginPresenter {
        private readonly ILoginLogic _logic;
        private readonly IPreferencesLogic _preferencesLogic;
        private readonly IUpdateCheckLogic _updateCheckLogic;
        private readonly ILoginView _view;
        private Preferences _preferences;

        public LoginPresenter(ILoginView view, ILoginLogic logic, IPreferencesLogic preferencesLogic,
                              IUpdateCheckLogic updateCheckLogic) {
            this._view = view;
            this._logic = logic;
            this._preferencesLogic = preferencesLogic;
            this._updateCheckLogic = updateCheckLogic;
        }

        public async void InitializeScreen() {
            try {
                this._view.ShowSpinner(true);
                this._view.ShowLoggedOutControl();
                this._preferences = this._preferencesLogic.GetPreferences();
                if (!await this._logic.IsUserLoggedInAsync(ApplyUser)) {
                    Logout();
                }
            }
            catch (Exception ex) {
                _view.HandleException(ex);
            }
        }

        public void Login() {
            try {
                this._view.ShowSpinner(true);
                this._logic.Login(ApplyUser);
            }
            catch (Exception ex) {
                _view.HandleException(ex);
            }
        }

        public void Logout() {
            this._logic.Logout();
            if (this._preferences != null) {
                this._preferencesLogic.EmptyCacheDirectory(this._preferences.CacheLocation);
            }
            this._preferences = null;
            this._view.ShowSpinner(false);
            this._view.ShowLoggedOutControl();
        }

        public void Continue() {
            if (this._preferences != null) {
                this._view.OpenLandingWindow();
            } else {
                this._view.OpenPreferencesWindow(Preferences.GetDefault());
            }
        }

        private void ApplyUser(User user) {
            if (this._preferences != null) {
                if (this._preferences.CheckForUpdates) {
                    var update = this._updateCheckLogic.UpdateAvailable(this._preferences);
                    if (update.Available) {
                        this._view.ShowUpdateAvailableNotification(update.LatestVersion);
                    }
                }
            }
            this._view.ShowLoggedInControl(this._preferences);
            this._view.User = user;
            this._view.ShowSpinner(false);
        }
    }
}
