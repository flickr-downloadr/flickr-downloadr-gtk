namespace FloydPink.Flickr.Downloadr.Logic {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Extensions;
    using Interfaces;
    using Model;
    using Model.Constants;
    using OAuth;
    using Repository;

    public class LoginLogic : ILoginLogic {
        private readonly IOAuthManager _oAuthManager;
        private readonly IRepository<Preferences> _preferencesRepository;
        private readonly IRepository<Token> _tokenRepository;
        private readonly IRepository<Update> _updateRepository;
        private readonly IUserInfoLogic _userInfoLogic;
        private readonly IRepository<User> _userRepository;
        private Action<User> _applyUser;

        public LoginLogic(IOAuthManager oAuthManager, IUserInfoLogic userInfoLogic, IRepository<Token> tokenRepository,
                          IRepository<User> userRepository, IRepository<Preferences> preferencesRepository,
                          IRepository<Update> updateRepository) {
            this._oAuthManager = oAuthManager;
            this._userInfoLogic = userInfoLogic;
            this._tokenRepository = tokenRepository;
            this._userRepository = userRepository;
            this._preferencesRepository = preferencesRepository;
            this._updateRepository = updateRepository;
        }

        private void OAuthManagerAuthenticated(object sender, AuthenticatedEventArgs e) {
            var authenticatedUser = e.AuthenticatedUser;
            this._userRepository.Save(authenticatedUser);
            CallApplyUser(authenticatedUser);
        }

        private async void CallApplyUser(User authenticatedUser) {
            this._applyUser(await this._userInfoLogic.PopulateUserInfo(authenticatedUser));
        }

        #region ILoginLogic Members

        public void Login(Action<User> applyUser) {
            this._applyUser = applyUser;
            this._oAuthManager.Authenticated += OAuthManagerAuthenticated;
            Process.Start(new ProcessStartInfo {
                FileName = this._oAuthManager.BeginAuthorization()
            });
        }

        public void Logout() {
            this._tokenRepository.Delete();
            this._userRepository.Delete();
            this._preferencesRepository.Delete();
            this._updateRepository.Delete();
        }

        public async Task<bool> IsUserLoggedInAsync(Action<User> applyUser) {
            this._applyUser = applyUser;
            var token = this._tokenRepository.Get();
            var user = this._userRepository.Get();
            if (string.IsNullOrEmpty(token.TokenString)) {
                return false;
            }

            this._oAuthManager.AccessToken = token.TokenString;
            var testLogin =
                (Dictionary<string, object>) await this._oAuthManager.MakeAuthenticatedRequestAsync(Methods.TestLogin);
            var userIsLoggedIn = (string) testLogin.GetSubValue("user", "id") == user.UserNsId;

            if (userIsLoggedIn) {
                CallApplyUser(user);
            }
            return userIsLoggedIn;
        }

        #endregion
    }
}
