using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Extensions;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.OAuth;
using FloydPink.Flickr.Downloadr.Repository;

namespace FloydPink.Flickr.Downloadr.Logic
{
  public class LoginLogic : ILoginLogic
  {
    private readonly IOAuthManager _oAuthManager;
    private readonly IRepository<Preferences> _preferencesRepository;
    private readonly ISystemProcess _process;
    private readonly IRepository<Token> _tokenRepository;
    private readonly IRepository<Update> _updateRepository;
    private readonly IUserInfoLogic _userInfoLogic;
    private readonly IRepository<User> _userRepository;
    private Action<User> _applyUser;

    public LoginLogic(IOAuthManager oAuthManager, IUserInfoLogic userInfoLogic, ISystemProcess process,
      IRepository<Token> tokenRepository, IRepository<User> userRepository,
      IRepository<Preferences> preferencesRepository, IRepository<Update> updateRepository)
    {
      _oAuthManager = oAuthManager;
      _userInfoLogic = userInfoLogic;
      _process = process;
      _tokenRepository = tokenRepository;
      _userRepository = userRepository;
      _preferencesRepository = preferencesRepository;
      _updateRepository = updateRepository;
    }

    private void OAuthManagerAuthenticated(object sender, AuthenticatedEventArgs e)
    {
      var authenticatedUser = e.AuthenticatedUser;
      _userRepository.Save(authenticatedUser);
      CallApplyUser(authenticatedUser);
    }

    private async void CallApplyUser(User authenticatedUser)
    {
      _applyUser(await _userInfoLogic.PopulateUserInfo(authenticatedUser));
    }

    #region ILoginLogic Members

    public void Login(Action<User> applyUser)
    {
      _applyUser = applyUser;
      _oAuthManager.Authenticated += OAuthManagerAuthenticated;
      _process.Start(new ProcessStartInfo
      {
        FileName = _oAuthManager.BeginAuthorization()
      });
    }

    public void Logout()
    {
      _tokenRepository.Delete();
      _userRepository.Delete();
      _preferencesRepository.Delete();
      _updateRepository.Delete();
    }

    public async Task<bool> IsUserLoggedInAsync(Action<User> applyUser)
    {
      _applyUser = applyUser;
      var token = _tokenRepository.Get();
      var user = _userRepository.Get();
      if (string.IsNullOrEmpty(token.TokenString))
      {
        return false;
      }

      _oAuthManager.AccessToken = token.TokenString;
      var testLogin =
        (Dictionary<string, object>) await _oAuthManager.MakeAuthenticatedRequestAsync(Methods.TestLogin);
      var userIsLoggedIn = (string) testLogin.GetSubValue("user", "id") == user.UserNsId;

      if (userIsLoggedIn)
      {
        CallApplyUser(user);
      }
      return userIsLoggedIn;
    }

    #endregion
  }
}
