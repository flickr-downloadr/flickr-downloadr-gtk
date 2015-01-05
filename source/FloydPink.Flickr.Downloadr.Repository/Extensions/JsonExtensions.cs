namespace FloydPink.Flickr.Downloadr.Repository.Extensions {
    using System.Web.Script.Serialization;
    using Model.Helpers;

    public static class JsonExtensions {
        public static string ToJson(this object value) {
            var serialized = string.Empty;
            InvariantCultureHelper.PerformInInvariantCulture(delegate { serialized = (new JavaScriptSerializer()).Serialize(value); });
            return serialized;
        }

        public static T FromJson<T>(this string json) where T : new() {
            if (string.IsNullOrEmpty(json)) {
                return new T();
            }
            var deserialized = new T();
            InvariantCultureHelper.PerformInInvariantCulture(delegate { deserialized = (new JavaScriptSerializer()).Deserialize<T>(json); });
            return deserialized;
        }
    }
}
