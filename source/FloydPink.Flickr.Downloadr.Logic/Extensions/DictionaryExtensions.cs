using System;
using System.Collections.Generic;
using System.Linq;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;

namespace FloydPink.Flickr.Downloadr.Logic.Extensions
{
    public static class DictionaryExtensions
    {
        public static object GetValue(this Dictionary<string, object> dictionary, string key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : string.Empty;
        }

        public static object GetSubValue(this Dictionary<string, object> dictionary, string key,
            string subKey = AppConstants.FlickrDictionaryContentKey)
        {
            if (dictionary.ContainsKey(key))
            {
                var subDictionary = (Dictionary<string, object>) dictionary[key];
                return subDictionary.ContainsKey(subKey) ? subDictionary[subKey] : null;
            }
            return null;
        }

        public static PhotosResponse GetPhotosResponseFromDictionary(this Dictionary<string, object> dictionary)
        {
            var photos = new List<Photo>();
            IEnumerable<Dictionary<string, object>> photoDictionary =
                ((IEnumerable<object>) dictionary.GetSubValue("photos", "photo")).
                    Cast<Dictionary<string, object>>();

            photos.AddRange(photoDictionary.Select(BuildPhoto));

            return new PhotosResponse(
                Convert.ToInt32(dictionary.GetSubValue("photos", "page")),
                Convert.ToInt32(dictionary.GetSubValue("photos", "pages")),
                Convert.ToInt32(dictionary.GetSubValue("photos", "perpage")),
                Convert.ToInt32(dictionary.GetSubValue("photos", "total")),
                photos);
        }

        public static IEnumerable<string> ExtractOriginalTags(this Dictionary<string, object> dictionary)
        {
            var photoJson = (Dictionary<string, object>) dictionary.GetValue("photo");
            var tagsJson = (Dictionary<string, object>) photoJson.GetValue("tags");
            var tagJsonArray = (object[]) tagsJson.GetValue("tag");
            return (from Dictionary<string, object> tag in tagJsonArray select tag.GetValue("raw").ToString()).ToList();
        }

        private static Photo BuildPhoto(Dictionary<string, object> dictionary)
        {
            return new Photo(dictionary.GetValue("id").ToString(),
                dictionary.GetValue("owner").ToString(),
                dictionary.GetValue("secret").ToString(),
                dictionary.GetValue("server").ToString(),
                Convert.ToInt32(dictionary.GetValue("farm")),
                dictionary.GetValue("title").ToString(),
                Convert.ToBoolean(dictionary.GetValue("ispublic")),
                Convert.ToBoolean(dictionary.GetValue("isfriend")),
                Convert.ToBoolean(dictionary.GetValue("isfamily")),
                dictionary.GetSubValue("description").ToString().Trim(),
                dictionary.GetValue("tags").ToString(),
                dictionary.GetValue("originalsecret").ToString(),
                dictionary.GetValue("originalformat").ToString(),
                dictionary.GetValue("url_sq").ToString(),
                dictionary.GetValue("url_q").ToString(),
                dictionary.GetValue("url_t").ToString(),
                dictionary.GetValue("url_s").ToString(),
                dictionary.GetValue("url_n").ToString(),
                dictionary.GetValue("url_m").ToString(),
                dictionary.GetValue("url_z").ToString(),
                dictionary.GetValue("url_c").ToString(),
                dictionary.GetValue("url_l").ToString(),
                dictionary.GetValue("url_o").ToString());
        }
    }
}