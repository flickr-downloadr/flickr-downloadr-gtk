namespace FloydPink.Flickr.Downloadr.Repository.Extensions {
    using System.Globalization;
    using System.Threading;
    using System.Web.Script.Serialization;

    public static class JsonExtensions {
        public static string ToJson(this object value) {
            string serialized;
            var before = Thread.CurrentThread.CurrentCulture;
            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                serialized = (new JavaScriptSerializer()).Serialize(value);
            }

            finally {
                Thread.CurrentThread.CurrentUICulture = before;
            }
            return serialized;
        }

        public static T FromJson<T>(this string json) where T : new() {
            if (string.IsNullOrEmpty(json)) {
                return new T();
            }
            T deserialized;
            var before = Thread.CurrentThread.CurrentCulture;
            try {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                deserialized = (new JavaScriptSerializer()).Deserialize<T>(json);
            }

            finally {
                Thread.CurrentThread.CurrentUICulture = before;
            }
            return deserialized;
        }
    }
}
