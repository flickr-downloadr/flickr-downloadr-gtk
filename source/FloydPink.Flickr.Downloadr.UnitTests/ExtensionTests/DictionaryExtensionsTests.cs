using System.Collections.Generic;
using FloydPink.Flickr.Downloadr.Logic.Extensions;
using NUnit.Framework;

namespace FloydPink.Flickr.Downloadr.UnitTests.ExtensionTests {
    [TestFixture]
    public class DictionaryExtensionsTests {
        [Test]
        public void WillGetValueFromJsonDictionary() {
            var dictionary = new Dictionary<string, object> {
                {
                    "key1", new Dictionary<string, object> {
                        {
                            "_content", "value1"
                        }
                    }
                }, {
                    "key2", new Dictionary<string, object> {
                        {
                            "_content", "value2"
                        }
                    }
                }
            };
            Assert.AreEqual("value1", dictionary.GetSubValue("key1"));
            Assert.AreEqual("value2", dictionary.GetSubValue("key2"));
        }
    }
}
