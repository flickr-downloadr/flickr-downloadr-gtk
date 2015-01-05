namespace FloydPink.Flickr.Downloadr.UI.Helpers {
    using System.Reflection;
    using System.Web;

    public class VersionHelper {
        private const string CampaignUrlFormat = "http://flickrdownloadr.com/{0}?utm_source=gtkapp&utm_medium={1}&utm_campaign={2}";

        public static string GetVersionString() {
            return string.Format(" (beta v{0})", Assembly.GetExecutingAssembly().GetName().Version);
        }

        public static string GetAboutUrl() {
            return string.Format(CampaignUrlFormat, string.Empty, "about", HttpUtility.UrlEncode(GetVersionString()));
        }

        public static string GetUpdateUrl() {
            return string.Format(CampaignUrlFormat, "downloads", "update", HttpUtility.UrlEncode(GetVersionString()));
        }
    }
}
