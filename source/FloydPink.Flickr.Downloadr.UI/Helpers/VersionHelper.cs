using System.Reflection;
using System.Web;

namespace FloydPink.Flickr.Downloadr.UI.Helpers
{
    public class VersionHelper
    {
        public static string GetVersionString()
        {
            return string.Format(" (beta v{0})", Assembly.GetExecutingAssembly().GetName().Version);
        }

        public static string GetAboutUrl()
        {
            return string.Format("http://flickrdownloadr.com/?utm_source=app&utm_medium=about&utm_campaign={0}",
                HttpUtility.UrlEncode(GetVersionString()));
        }
    }
}