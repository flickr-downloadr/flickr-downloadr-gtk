using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using FloydPink.Flickr.Downloadr.Logic;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.OAuth;
using FloydPink.Flickr.Downloadr.Repository;
using NUnit.Framework;
using Rhino.Mocks;

namespace FloydPink.Flickr.Downloadr.UnitTests.LogicTests {
    [TestFixture]
    public class LoginLogicTests {
        private IOAuthManager _oAuthManager;
        private IUserInfoLogic _userInfoLogic;
        private IRepository<Preferences> _preferencesRepository;
        private IRepository<Token> _tokenRepository;
        private IRepository<Update> _updateRepository;
        private IRepository<User> _userRepository;

        [SetUp]
        public void Setup() {
            _oAuthManager = MockRepository.GenerateMock<IOAuthManager>();
            _userInfoLogic = MockRepository.GenerateMock<IUserInfoLogic>();
            _tokenRepository = MockRepository.GenerateMock<IRepository<Token>>();
            _userRepository = MockRepository.GenerateMock<IRepository<User>>();
            _preferencesRepository = MockRepository.GenerateStub<IRepository<Preferences>>();
            _updateRepository = MockRepository.GenerateStub<IRepository<Update>>();
        }

        [Test]
        public void WillCallDeleteOnAllRepositoriesOnLogout() {
            var logic = new LoginLogic(null, null, _tokenRepository, _userRepository, _preferencesRepository,
                _updateRepository);
            logic.Logout();

            _tokenRepository.AssertWasCalled(t => t.Delete());
            _userRepository.AssertWasCalled(u => u.Delete());
            _preferencesRepository.AssertWasCalled(u => u.Delete());
            _updateRepository.AssertWasCalled(u => u.Delete());
        }

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void WillCallBeginAuthorizationOnOAuthManagerOnLogin() {
            _oAuthManager.Expect(o => o.BeginAuthorization()).Return(string.Empty);
            var logic = new LoginLogic(_oAuthManager, null, null, null, null, null);

            logic.Login(null);

            _oAuthManager.AssertWasCalled(o => o.BeginAuthorization());
        }

        [Test]
        public async void WillReturnFalseWhenTokenRepositoryReturnsEmptyTokenStringOnIsUserLoggedInAsync() {
            var logic = new LoginLogic(_oAuthManager, null, _tokenRepository, _userRepository, _preferencesRepository, _updateRepository);
            _tokenRepository.Expect(t => t.Get()).Return(new Token {
                TokenString = null,
                Secret = null
            });
            _userRepository.Expect(t => t.Get()).Return(null);

            var applyUser = new Action<User>(delegate { });

            Assert.IsFalse(await logic.IsUserLoggedInAsync(applyUser));

            _tokenRepository.VerifyAllExpectations();
            _userRepository.VerifyAllExpectations();
        }

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public async void WillCallTestLoginMethodOnIsUserLoggedInAsync() {
            var logic = new LoginLogic(_oAuthManager, null, _tokenRepository, _userRepository, _preferencesRepository, _updateRepository);

            const string tokenString = "Some String";
            _tokenRepository.Expect(t => t.Get()).Return(new Token {
                TokenString = tokenString,
                Secret = null
            });
            _userRepository.Expect(t => t.Get()).Return(new User());

            _oAuthManager.Expect(o => o.MakeAuthenticatedRequestAsync(null, null)).IgnoreArguments();

            var applyUser = new Action<User>(delegate { });

            await logic.IsUserLoggedInAsync(applyUser);

            _tokenRepository.VerifyAllExpectations();
            _userRepository.VerifyAllExpectations();

            Assert.Equals(_oAuthManager.AccessToken, tokenString);

            _oAuthManager.VerifyAllExpectations();
        }

        [Test]
        public async void WillReturnTrueOnIsUserLoggedInAsync() {
            const string nsId = "some nsid";
            const string userName = "some username";
            const string mockJsonResponse = "{\"user\":{\"id\":\"" + nsId + "\",\"username\":{\"_content\":\"" + userName +
                                            "\"}},\"stat\":\"ok\"}";
            dynamic deserializedJson = (new JavaScriptSerializer()).Deserialize<dynamic>(mockJsonResponse);

            var logic = new LoginLogic(_oAuthManager, _userInfoLogic, _tokenRepository, _userRepository, _preferencesRepository, _updateRepository);

            _tokenRepository.Expect(t => t.Get()).Return(new Token {
                TokenString = "Some String",
                Secret = null
            });
            _userRepository.Expect(t => t.Get()).Return(new User {
                Name = userName,
                UserNsId = nsId
            });

            _userInfoLogic.Expect(u => u.PopulateUserInfo(null)).IgnoreArguments().Return(Task.FromResult(new User()));

            _oAuthManager.Stub(o => o.MakeAuthenticatedRequestAsync(null, null))
                         .IgnoreArguments()
                         .Return(Task.FromResult<dynamic>(deserializedJson));

            var applyUser = new Action<User>(delegate(User u) { });

            Assert.IsTrue(await logic.IsUserLoggedInAsync(applyUser));

            _tokenRepository.VerifyAllExpectations();
            _userRepository.VerifyAllExpectations();
            _oAuthManager.VerifyAllExpectations();
        }

        [Test]
        public async void WillReturnFalseOnIsUserLoggedInAsync() {
            const string nsId = "some nsid";
            const string userName = "some username";
            const string mockJsonResponse = "{\"user\":{\"id\":\"" + nsId + "\",\"username\":{\"_content\":\"" + userName +
                                            "\"}},\"stat\":\"ok\"}";
            dynamic deserializedJson = (new JavaScriptSerializer()).Deserialize<dynamic>(mockJsonResponse);

            var logic = new LoginLogic(_oAuthManager, null, _tokenRepository, _userRepository, _preferencesRepository, _updateRepository);

            _tokenRepository.Expect(t => t.Get()).Return(new Token {
                TokenString = "Some String",
                Secret = null
            });
            _userRepository.Expect(t => t.Get()).Return(new User {
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
    }
}
