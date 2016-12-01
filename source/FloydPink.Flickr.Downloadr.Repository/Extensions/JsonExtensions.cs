using System.Web.Script.Serialization;
using FloydPink.Flickr.Downloadr.Model.Helpers;

namespace FloydPink.Flickr.Downloadr.Repository.Extensions
{
  public static class JsonExtensions
  {
    public static string ToJson(this object value)
    {
      var serialized = string.Empty;
      InvariantCultureHelper.PerformInInvariantCulture(delegate { serialized = new JavaScriptSerializer().Serialize(value); });
      return serialized;
    }

    public static T FromJson<T>(this string json) where T : new()
    {
      if (string.IsNullOrEmpty(json))
      {
        return new T();
      }
      var deserialized = new T();
      InvariantCultureHelper.PerformInInvariantCulture(delegate { deserialized = new JavaScriptSerializer().Deserialize<T>(json); });
      return deserialized;
    }
  }
}
