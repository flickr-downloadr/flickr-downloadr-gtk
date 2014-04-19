using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;
using NUnit.Framework;

namespace FloydPink.Flickr.Downloadr.UnitTests.RepositoryTests {
    [TestFixture]
    public class RepositoryTests {
        private TokenRepository _repository;

        [TestFixtureSetUp]
        public void SetUp() {
            this._repository = new TokenRepository();
        }

        private Token getNewAccessToken() {
            return new Token("token", "secret");
        }

        [Test]
        public void WillDeleteAndNotGetToken() {
            Token token = getNewAccessToken();
            this._repository.Save(token);
            this._repository.Delete();
            token = this._repository.Get();
            Assert.IsEmpty(token.TokenString);
            Assert.IsEmpty(token.Secret);
        }

        [Test]
        public void WillNotGetTokenWhenNoFileExists() {
            Token token = this._repository.Get();
            Assert.IsEmpty(token.TokenString);
            Assert.IsEmpty(token.Secret);
        }

        [Test]
        public void WillSaveAndGetToken() {
            Token token = getNewAccessToken();
            this._repository.Save(token);
            token = this._repository.Get();
            Assert.AreEqual("token", token.TokenString);
            Assert.AreEqual("secret", token.Secret);
            this._repository.Delete();
        }
    }
}
