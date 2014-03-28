using System;
using System.Globalization;
using FloydPink.Flickr.Downloadr.Model.Constants;

namespace FloydPink.Flickr.Downloadr.Model
{
    public class UserInfo
    {
        public string Id { get; set; }
        public bool IsPro { get; set; }
        public string IconServer { get; set; }
        public int IconFarm { get; set; }
        public string PathAlias { get; set; }
        public string Description { get; set; }
        public string PhotosUrl { get; set; }
        public string ProfileUrl { get; set; }
        public string MobileUrl { get; set; }
        public int PhotosCount { get; set; }

        public string BuddyIconUrl
        {
            get
            {
                return Convert.ToInt32(IconServer) > 0
                    ? string.Format(AppConstants.BuddyIconUrlFormat,
                        IconFarm.ToString(CultureInfo.InvariantCulture), IconServer, Id)
                    : AppConstants.DefaultBuddyIconUrl;
            }
        }
    }
}