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
        private Action<User> _applyUser;
        private readonly IOAuthManager _oAuthManager;
        private readonly IRepository<Preferences> _preferencesRepository;
        private readonly IRepository<Token> _tokenRepository;
        private readonly IRepository<Update> _updateRepository;
        private readonly IUserInfoLogic _userInfoLogic;
        private readonly IRepository<User> _userRepository;

        public LoginLogic(IOAuthManager oAuthManager, IUserInfoLogic userInfoLogic, IRepository<Token> tokenRepository,
                          IRepository<User> userRepository, IRepository<Preferences> preferencesRepository,
                          IRepository<Update> updateRepository) {
            _oAuthManager = oAuthManager;
            _userInfoLogic = userInfoLogic;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _preferencesRepository = preferencesRepository;
            _updateRepository = updateRepository;
        }

        private void OAuthManagerAuthenticated(object sender, AuthenticatedEventArgs e) {
            var authenticatedUser = e.AuthenticatedUser;
            _userRepository.Save(authenticatedUser);
            CallApplyUser(authenticatedUser);
        }

        private async void CallApplyUser(User authenticatedUser) {
            _applyUser(await _userInfoLogic.PopulateUserInfo(authenticatedUser));
        }

        #region ILoginLogic Members

        public void Login(Action<User> applyUser) {
            _applyUser = applyUser;
            _oAuthManager.Authenticated += OAuthManagerAuthenticated;
            Process.Start(new ProcessStartInfo {
                FileName = _oAuthManager.BeginAuthorization()
            });
        }

        public void Logout() {
            _tokenRepository.Delete();
            _userRepository.Delete();
            _preferencesRepository.Delete();
            _updateRepository.Delete();
        }

        public async Task<bool> IsUserLoggedInAsync(Action<User> applyUser) {
            _applyUser = applyUser;
            var token = _tokenRepository.Get();
            var user = _userRepository.Get();
            if (string.IsNullOrEmpty(token.TokenString)) {
                return false;
            }

            _oAuthManager.AccessToken = token.TokenString;
            var testLogin =
                (Dictionary<string, object>) await _oAuthManager.MakeAuthenticatedRequestAsync(Methods.TestLogin);
            var userIsLoggedIn = (string) testLogin.GetSubValue("user", "id") == user.UserNsId;

            if (userIsLoggedIn) {
                CallApplyUser(user);
            }
            return userIsLoggedIn;
        }

        #endregion
    }
}
