using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository.Extensions;
using NUnit.Framework;

namespace FloydPink.Flickr.Downloadr.UnitTests.ExtensionTests
{
    [TestFixture]
    public class JsonExtensionsTests
    {
        [Test]
        public void WillConvertEmptyJsonStringToNewInstance()
        {
            string userAsJson = string.Empty;
            var user = userAsJson.FromJson<User>();
            Assert.IsNotNull(user);
            Assert.AreEqual(string.Empty, user.Name);
            Assert.AreEqual(string.Empty, user.Username);
            Assert.AreEqual(string.Empty, user.UserNsId);
        }

        [Test]
        public void WillConvertInstanceToJson()
        {
            var token = new Token("token", "secret");
            string tokenJson = "{\"TokenString\":\"token\",\"Secret\":\"secret\"}";
            Assert.AreEqual(tokenJson, token.ToJson());
        }

        [Test]
        public void WillConvertJsonToInstance()
        {
            string tokenJson = "{\"TokenString\":\"token\",\"Secret\":\"secret\"}";
            var token = tokenJson.FromJson<Token>();
            Assert.AreEqual("token", token.TokenString);
            Assert.AreEqual("secret", token.Secret);
        }

        [Test]
        public void WillConvertJsonToUserInstance()
        {
            string userAsJson = "{\"Name\":\"name\",\"Username\":\"username\",\"UserNSId\":\"usernsid\"}";
            var user = userAsJson.FromJson<User>();
            Assert.AreEqual("name", user.Name);
            Assert.AreEqual("username", user.Username);
            Assert.AreEqual("usernsid", user.UserNsId);
        }

        [Test]
        public void WillConvertUserInstanceToJson()
        {
            var user = new User("name", "username", "usernsid");
            string userAsJson =
                "{\"Name\":\"name\",\"Username\":\"username\",\"UserNsId\":\"usernsid\",\"WelcomeMessage\":\"Welcome, name!\"}";
            Assert.AreEqual(userAsJson, user.ToJson());
        }
    }
}