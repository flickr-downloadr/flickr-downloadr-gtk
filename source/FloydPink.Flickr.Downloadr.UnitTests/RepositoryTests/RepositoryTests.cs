namespace FloydPink.Flickr.Downloadr.UnitTests.RepositoryTests {
    using Model;
    using NUnit.Framework;
    using Repository;

    [TestFixture]
    public class RepositoryTests {
        private TokenRepository _repository;

        [TestFixtureSetUp]
        public void SetUp() {
            _repository = new TokenRepository();
        }

        private Token getNewAccessToken() {
            return new Token("token", "secret");
        }

        [Test]
        public void WillDeleteAndNotGetToken() {
            var token = getNewAccessToken();
            _repository.Save(token);
            _repository.Delete();
            token = _repository.Get();
            Assert.IsEmpty(token.TokenString);
            Assert.IsEmpty(token.Secret);
        }

        [Test]
        public void WillNotGetTokenWhenNoFileExists() {
            var token = _repository.Get();
            Assert.IsEmpty(token.TokenString);
            Assert.IsEmpty(token.Secret);
        }

        [Test]
        public void WillSaveAndGetToken() {
            var token = getNewAccessToken();
            _repository.Save(token);
            token = _repository.Get();
            Assert.AreEqual("token", token.TokenString);
            Assert.AreEqual("secret", token.Secret);
            _repository.Delete();
        }
    }
}
