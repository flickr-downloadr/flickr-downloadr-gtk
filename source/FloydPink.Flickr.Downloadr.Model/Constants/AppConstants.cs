namespace FloydPink.Flickr.Downloadr.Model.Constants
{
    public static class AppConstants
    {
        public const string FlickrDictionaryContentKey = "_content";

        public const string AuthenticatedMessage =
            "<html><head><title>flickr downloadr Authenticated</title></head>" +
            "<body><div style=\"text-align:center;\">" +
            "<img alt=\"flickr downloadr\" src=\"http://flickrdownloadr.com/img/logo.png\" />" +
            "<h1>You have been authenticated.</h1><div><span>You could close this window and return to the flickr downloadr application " +
            "now.</span></div></div></body></html>";

        public const string BuddyIconUrlFormat = "http://farm{0}.staticflickr.com/{1}/buddyicons/{2}.jpg";
        public const string DefaultBuddyIconUrl = "http://www.flickr.com/images/buddyicon.gif";

        public const string ExtraInfo =
            "description, license, date_upload, date_taken, owner_name, icon_server, original_format, last_update, geo, tags, machine_tags, o_dims, views, media, path_alias, url_sq, url_t, url_s, url_q, url_m, url_n, url_z, url_c, url_l, url_o";

        public const string MoreThan1000PhotosWarningFormat =
            "There are a total of {0} photos to be downloaded! This WILL take quite a while. Are you sure?";

        public const string MoreThan500PhotosWarningFormat =
            "You are going to download {0} photos. It could take a while. Continue?";

        public const string MoreThan100PhotosWarningFormat = "Are you sure you want to download {0} photos?";
    }
}