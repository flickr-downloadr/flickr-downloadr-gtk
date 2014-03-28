using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;
using NUnit.Framework;

namespace FloydPink.Flickr.Downloadr.UnitTests.RepositoryTests
{
    [TestFixture]
    public class RepositoryTests
    {
        private TokenRepository _repository;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _repository = new TokenRepository();
        }

        private Token getNewAccessToken()
        {
            return new Token("token", "secret");
        }

        [Test]
        public void WillDeleteAndNotGetToken()
        {
            Token token = getNewAccessToken();
            _repository.Save(token);
            _repository.Delete();
            token = _repository.Get();
            Assert.IsEmpty(token.TokenString);
            Assert.IsEmpty(token.Secret);
        }

        [Test]
        public void WillNotGetTokenWhenNoFileExists()
        {
            Token token = _repository.Get();
            Assert.IsEmpty(token.TokenString);
            Assert.IsEmpty(token.Secret);
        }

        [Test]
        public void WillSaveAndGetToken()
        {
            Token token = getNewAccessToken();
            _repository.Save(token);
            token = _repository.Get();
            Assert.AreEqual("token", token.TokenString);
            Assert.AreEqual("secret", token.Secret);
            _repository.Delete();
        }
    }
}