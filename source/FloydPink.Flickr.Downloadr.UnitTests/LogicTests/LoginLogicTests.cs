using FloydPink.Flickr.Downloadr.Logic;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;
using NUnit.Framework;
using Rhino.Mocks;

namespace FloydPink.Flickr.Downloadr.UnitTests.LogicTests {
    [TestFixture]
    public class LoginLogicTests {
        private IRepository<Token> _tokenRepository;
        private IRepository<User> _userRepository;
        private IRepository<Preferences> _preferencesRepository;

        [TestFixtureSetUp]
        public void Setup() {
            this._tokenRepository = MockRepository.GenerateStub<IRepository<Token>>();
            this._userRepository = MockRepository.GenerateStub<IRepository<User>>();
            this._preferencesRepository = MockRepository.GenerateStub<IRepository<Preferences>>();
        }

        [Test]
        public void WillCallDeleteOnBothRepositoriesOnLogout() {
            var logic = new LoginLogic(null, this._tokenRepository, this._userRepository, this._preferencesRepository);
            logic.Logout();

            this._tokenRepository.AssertWasCalled(t => t.Delete());
            this._userRepository.AssertWasCalled(u => u.Delete());
        }
    }
}
