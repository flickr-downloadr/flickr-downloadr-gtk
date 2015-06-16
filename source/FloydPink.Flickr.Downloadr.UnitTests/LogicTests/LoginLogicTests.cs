namespace FloydPink.Flickr.Downloadr.UnitTests.LogicTests {
    using System;
    using System.Threading.Tasks;
    using System.Web.Script.Serialization;
    using Logic;
    using Logic.Interfaces;
    using Model;
    using NUnit.Framework;
    using OAuth;
    using Repository;
    using Rhino.Mocks;

    [TestFixture]
    public class LoginLogicTests {
        [SetUp]
        public void Setup() {
            this._oAuthManager = MockRepository.GenerateMock<IOAuthManager>();
            this._userInfoLogic = MockRepository.GenerateMock<IUserInfoLogic>();
            this._tokenRepository = MockRepository.GenerateMock<IRepository<Token>>();
            this._userRepository = MockRepository.GenerateMock<IRepository<User>>();
            this._preferencesRepository = MockRepository.GenerateStub<IRepository<Preferences>>();
            this._updateRepository = MockRepository.GenerateStub<IRepository<Update>>();
        }

        private IOAuthManager _oAuthManager;
        private IRepository<Preferences> _preferencesRepository;
        private IRepository<Token> _tokenRepository;
        private IRepository<Update> _updateRepository;
        private IUserInfoLogic _userInfoLogic;
        private IRepository<User> _userRepository;

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public void WillCallBeginAuthorizationOnOAuthManagerOnLogin() {
            this._oAuthManager.Expect(o => o.BeginAuthorization()).Return(string.Empty);
            var logic = new LoginLogic(this._oAuthManager, null, null, null, null, null);

            logic.Login(null);

            this._oAuthManager.AssertWasCalled(o => o.BeginAuthorization());
        }

        [Test]
        public void WillCallDeleteOnAllRepositoriesOnLogout() {
            var logic = new LoginLogic(null, null, this._tokenRepository, this._userRepository, this._preferencesRepository,
                this._updateRepository);
            logic.Logout();

            this._tokenRepository.AssertWasCalled(t => t.Delete());
            this._userRepository.AssertWasCalled(u => u.Delete());
            this._preferencesRepository.AssertWasCalled(u => u.Delete());
            this._updateRepository.AssertWasCalled(u => u.Delete());
        }

        [Test, ExpectedException(typeof (InvalidOperationException))]
        public async void WillCallTestLoginMethodOnIsUserLoggedInAsync() {
            var logic = new LoginLogic(this._oAuthManager, null, this._tokenRepository, this._userRepository, this._preferencesRepository,
                this._updateRepository);

            const string tokenString = "Some String";
            this._tokenRepository.Expect(t => t.Get()).Return(new Token {
                TokenString = tokenString,
                Secret = null
            });
            this._userRepository.Expect(t => t.Get()).Return(new User());

            this._oAuthManager.Expect(o => o.MakeAuthenticatedRequestAsync(null, null)).IgnoreArguments();

            var applyUser = new Action<User>(delegate { });

            await logic.IsUserLoggedInAsync(applyUser);

            this._tokenRepository.VerifyAllExpectations();
            this._userRepository.VerifyAllExpectations();

            Assert.Equals(this._oAuthManager.AccessToken, tokenString);

            this._oAuthManager.VerifyAllExpectations();
        }

        [Test]
        public async void WillReturnFalseOnIsUserLoggedInAsync() {
            const string nsId = "some nsid";
            const string userName = "some username";
            const string mockJsonResponse = "{\"user\":{\"id\":\"" + nsId + "\",\"username\":{\"_content\":\"" + userName +
                                            "\"}},\"stat\":\"ok\"}";
            dynamic deserializedJson = (new JavaScriptSerializer()).Deserialize<dynamic>(mockJsonResponse);

            var logic = new LoginLogic(this._oAuthManager, null, this._tokenRepository, this._userRepository, this._preferencesRepository,
                this._updateRepository);

            this._tokenRepository.Expect(t => t.Get()).Return(new Token {
                TokenString = "Some String",
                Secret = null
            });
            this._userRepository.Expect(t => t.Get()).Return(new User {
                Name = userName,
                UserNsId = "some other NsId"
            });

            this._oAuthManager.Stub(o => o.MakeAuthenticatedRequestAsync(null, null))
                .IgnoreArguments()
                .Return(Task.FromResult<dynamic>(deserializedJson));

            var applyUser = new Action<User>(delegate { });

            Assert.IsFalse(await logic.IsUserLoggedInAsync(applyUser));

            this._tokenRepository.VerifyAllExpectations();
            this._userRepository.VerifyAllExpectations();
            this._oAuthManager.VerifyAllExpectations();
        }

        [Test]
        public async void WillReturnFalseWhenTokenRepositoryReturnsEmptyTokenStringOnIsUserLoggedInAsync() {
            var logic = new LoginLogic(this._oAuthManager, null, this._tokenRepository, this._userRepository, this._preferencesRepository,
                this._updateRepository);
            this._tokenRepository.Expect(t => t.Get()).Return(new Token {
                TokenString = null,
                Secret = null
            });
            this._userRepository.Expect(t => t.Get()).Return(null);

            var applyUser = new Action<User>(delegate { });

            Assert.IsFalse(await logic.IsUserLoggedInAsync(applyUser));

            this._tokenRepository.VerifyAllExpectations();
            this._userRepository.VerifyAllExpectations();
        }

        [Test]
        public async void WillReturnTrueOnIsUserLoggedInAsync() {
            const string nsId = "some nsid";
            const string userName = "some username";
            const string mockJsonResponse = "{\"user\":{\"id\":\"" + nsId + "\",\"username\":{\"_content\":\"" + userName +
                                            "\"}},\"stat\":\"ok\"}";
            dynamic deserializedJson = (new JavaScriptSerializer()).Deserialize<dynamic>(mockJsonResponse);

            var logic = new LoginLogic(this._oAuthManager, this._userInfoLogic, this._tokenRepository, this._userRepository,
                this._preferencesRepository, this._updateRepository);

            this._tokenRepository.Expect(t => t.Get()).Return(new Token {
                TokenString = "Some String",
                Secret = null
            });
            this._userRepository.Expect(t => t.Get()).Return(new User {
                Name = userName,
                UserNsId = nsId
            });

            this._userInfoLogic.Expect(u => u.PopulateUserInfo(null)).IgnoreArguments().Return(Task.FromResult(new User()));

            this._oAuthManager.Stub(o => o.MakeAuthenticatedRequestAsync(null, null))
                .IgnoreArguments()
                .Return(Task.FromResult<dynamic>(deserializedJson));

            var applyUser = new Action<User>(delegate { });

            Assert.IsTrue(await logic.IsUserLoggedInAsync(applyUser));

            this._tokenRepository.VerifyAllExpectations();
            this._userRepository.VerifyAllExpectations();
            this._oAuthManager.VerifyAllExpectations();
        }
    }
}
