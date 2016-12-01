using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FloydPink.Flickr.Downloadr.Logic;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.OAuth;
using FloydPink.Flickr.Downloadr.Repository;
using NUnit.Framework;
using Rhino.Mocks;

namespace FloydPink.Flickr.Downloadr.UnitTests.LogicTests
{
  [TestFixture]
  public class LoginLogicTests
  {
    [SetUp]
    public void Setup()
    {
      _oAuthManager = MockRepository.GenerateMock<IOAuthManager>();
      _userInfoLogic = MockRepository.GenerateMock<IUserInfoLogic>();
      _systemProcess = MockRepository.GenerateMock<ISystemProcess>();
      _tokenRepository = MockRepository.GenerateMock<IRepository<Token>>();
      _userRepository = MockRepository.GenerateMock<IRepository<User>>();
      _preferencesRepository = MockRepository.GenerateStub<IRepository<Preferences>>();
      _updateRepository = MockRepository.GenerateStub<IRepository<Update>>();
    }

    private IOAuthManager _oAuthManager;
    private IRepository<Preferences> _preferencesRepository;
    private IRepository<Token> _tokenRepository;
    private IRepository<Update> _updateRepository;
    private IUserInfoLogic _userInfoLogic;
    private IRepository<User> _userRepository;
    private ISystemProcess _systemProcess;

    [Test]
    public void WillCallBeginAuthorizationOnOAuthManagerOnLogin()
    {
      _oAuthManager.Expect(o => o.BeginAuthorization()).Return(string.Empty);
      _systemProcess.Expect(s => s.Start(Arg<ProcessStartInfo>.Is.Anything));
      var logic = new LoginLogic(_oAuthManager, null, _systemProcess, null, null, null, null);

      logic.Login(null);

      _oAuthManager.AssertWasCalled(o => o.BeginAuthorization());
      _systemProcess.AssertWasCalled(o => o.Start(Arg<ProcessStartInfo>.Is.Anything));
    }

    [Test]
    public void WillCallDeleteOnAllRepositoriesOnLogout()
    {
      var logic = new LoginLogic(null, null, null, _tokenRepository, _userRepository, _preferencesRepository,
        _updateRepository);
      logic.Logout();

      _tokenRepository.AssertWasCalled(t => t.Delete());
      _userRepository.AssertWasCalled(u => u.Delete());
      _preferencesRepository.AssertWasCalled(u => u.Delete());
      _updateRepository.AssertWasCalled(u => u.Delete());
    }

    [Test]
    public async Task WillCallTestLoginMethodOnIsUserLoggedInAsync()
    {
      var logic = new LoginLogic(_oAuthManager, null, null, _tokenRepository, _userRepository, _preferencesRepository,
        _updateRepository);

      const string tokenString = "Some String";
      _tokenRepository.Expect(t => t.Get()).Return(new Token
      {
        TokenString = tokenString,
        Secret = null
      });
      _userRepository.Expect(t => t.Get()).Return(new User());

      _oAuthManager.Expect(o => o.MakeAuthenticatedRequestAsync(Arg<string>.Is.Anything, Arg<IDictionary<string, string>>.Is.Anything))
        .Return(Task.FromResult<dynamic>((dynamic) new Dictionary<string, object>()));

      var applyUser = new Action<User>(delegate { });

      await logic.IsUserLoggedInAsync(applyUser);

      _tokenRepository.VerifyAllExpectations();
      _userRepository.VerifyAllExpectations();
      _oAuthManager.VerifyAllExpectations();
    }

    [Test]
    public async Task WillReturnFalseOnIsUserLoggedInAsync()
    {
      const string nsId = "some nsid";
      const string userName = "some username";
      const string mockJsonResponse = "{\"user\":{\"id\":\"" + nsId + "\",\"username\":{\"_content\":\"" + userName +
                                      "\"}},\"stat\":\"ok\"}";
      dynamic deserializedJson = new JavaScriptSerializer().Deserialize<dynamic>(mockJsonResponse);

      var logic = new LoginLogic(_oAuthManager, null, null, _tokenRepository, _userRepository, _preferencesRepository,
        _updateRepository);

      _tokenRepository.Expect(t => t.Get()).Return(new Token
      {
        TokenString = "Some String",
        Secret = null
      });
      _userRepository.Expect(t => t.Get()).Return(new User
      {
        Name = userName,
        UserNsId = "some other NsId"
      });

      _oAuthManager.Stub(o => o.MakeAuthenticatedRequestAsync(null, null))
        .IgnoreArguments()
        .Return(Task.FromResult<dynamic>(deserializedJson));

      var applyUser = new Action<User>(delegate { });

      Assert.IsFalse(await logic.IsUserLoggedInAsync(applyUser));

      _tokenRepository.VerifyAllExpectations();
      _userRepository.VerifyAllExpectations();
      _oAuthManager.VerifyAllExpectations();
    }

    [Test]
    public async Task WillReturnFalseWhenTokenRepositoryReturnsEmptyTokenStringOnIsUserLoggedInAsync()
    {
      var logic = new LoginLogic(_oAuthManager, null, null, _tokenRepository, _userRepository, _preferencesRepository,
        _updateRepository);
      _tokenRepository.Expect(t => t.Get()).Return(new Token
      {
        TokenString = null,
        Secret = null
      });
      _userRepository.Expect(t => t.Get()).Return(null);

      var applyUser = new Action<User>(delegate { });

      Assert.IsFalse(await logic.IsUserLoggedInAsync(applyUser));

      _tokenRepository.VerifyAllExpectations();
      _userRepository.VerifyAllExpectations();
    }

    [Test]
    public async Task WillReturnTrueOnIsUserLoggedInAsync()
    {
      const string nsId = "some nsid";
      const string userName = "some username";
      const string mockJsonResponse = "{\"user\":{\"id\":\"" + nsId + "\",\"username\":{\"_content\":\"" + userName +
                                      "\"}},\"stat\":\"ok\"}";
      dynamic deserializedJson = new JavaScriptSerializer().Deserialize<dynamic>(mockJsonResponse);

      var logic = new LoginLogic(_oAuthManager, _userInfoLogic, null, _tokenRepository, _userRepository,
        _preferencesRepository, _updateRepository);

      _tokenRepository.Expect(t => t.Get()).Return(new Token
      {
        TokenString = "Some String",
        Secret = null
      });
      _userRepository.Expect(t => t.Get()).Return(new User
      {
        Name = userName,
        UserNsId = nsId
      });

      _userInfoLogic.Expect(u => u.PopulateUserInfo(null)).IgnoreArguments().Return(Task.FromResult(new User()));

      _oAuthManager.Stub(o => o.MakeAuthenticatedRequestAsync(null, null))
        .IgnoreArguments()
        .Return(Task.FromResult<dynamic>(deserializedJson));

      var applyUser = new Action<User>(delegate { });

      Assert.IsTrue(await logic.IsUserLoggedInAsync(applyUser));

      _tokenRepository.VerifyAllExpectations();
      _userRepository.VerifyAllExpectations();
      _oAuthManager.VerifyAllExpectations();
    }
  }
}
