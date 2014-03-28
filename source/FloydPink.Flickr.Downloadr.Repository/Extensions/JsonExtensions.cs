using System.Web.Script.Serialization;

namespace FloydPink.Flickr.Downloadr.Repository.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object value)
        {
            return (new JavaScriptSerializer()).Serialize(value);
        }

        public static T FromJson<T>(this string json) where T : new()
        {
            if (string.IsNullOrEmpty(json))
                return new T();
            return (new JavaScriptSerializer()).Deserialize<T>(json);
        }
    }
}