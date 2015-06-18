namespace FloydPink.Flickr.Downloadr.Model {
    using System.Web;

    public class Photo : IGridWidgetItem {
        private readonly string _large1024Url;
        private readonly string _medium500Url;
        private readonly string _medium640Url;
        private readonly string _medium800Url;
        private readonly string _originalUrl;
        private readonly string _small320Url;

        public Photo(string id, string owner, string secret, string server, int farm, string title, bool isPublic,
                     bool isFamily, bool isFriend,
                     string description, string tags, string originalSecret, string originalFormat,
                     string smallSquare75X75Url, string largeSquare150X150Url,
                     string thumbnailUrl, string small240Url, string small320Url, string medium500Url,
                     string medium640Url, string medium800Url, string large1024Url,
                     string originalUrl) {
            Id = id;
            Owner = owner;
            Secret = secret;
            Server = server;
            Farm = farm;
            Title = title;
            IsPublic = isPublic;
            IsFamily = isFamily;
            IsFriend = isFriend;
            Description = description;
            Tags = tags;
            OriginalSecret = originalSecret;
            OriginalFormat = originalFormat;
            SmallSquare75X75Url = smallSquare75X75Url;
            LargeSquare150X150Url = largeSquare150X150Url;
            ThumbnailUrl = thumbnailUrl;
            Small240Url = small240Url;
            this._small320Url = small320Url;
            this._medium500Url = medium500Url;
            this._medium640Url = medium640Url;
            this._medium800Url = medium800Url;
            this._large1024Url = large1024Url;
            this._originalUrl = originalUrl;
        }

        public string Owner { get; private set; }
        public bool IsPublic { get; private set; }
        public bool IsFamily { get; private set; }
        public bool IsFriend { get; private set; }
        public string Tags { get; set; }
        public string OriginalSecret { get; private set; }
        public string OriginalFormat { get; private set; }
        public string SmallSquare75X75Url { get; private set; }
        public string LargeSquare150X150Url { get; private set; }
        public string ThumbnailUrl { get; private set; }
        public string Small240Url { get; private set; }
        public string Small320Url { get { return string.IsNullOrWhiteSpace(this._small320Url) ? Small240Url : this._small320Url; } }
        public string Medium500Url { get { return string.IsNullOrWhiteSpace(this._medium500Url) ? Small320Url : this._medium500Url; } }
        public string Medium640Url { get { return string.IsNullOrWhiteSpace(this._medium640Url) ? Medium500Url : this._medium640Url; } }
        public string Medium800Url { get { return string.IsNullOrWhiteSpace(this._medium800Url) ? Medium640Url : this._medium800Url; } }
        public string Large1024Url { get { return string.IsNullOrWhiteSpace(this._large1024Url) ? Medium800Url : this._large1024Url; } }
        public string OriginalUrl { get { return string.IsNullOrWhiteSpace(this._originalUrl) ? Large1024Url : this._originalUrl; } }
        public string DownloadFormat { get { return string.IsNullOrWhiteSpace(OriginalFormat) ? "jpg" : OriginalFormat; } }

        public string HtmlEncodedTitle { get { return HttpUtility.HtmlEncode(Title); } }

        public string Id { get; private set; }
        public string Secret { get; private set; }
        public string Server { get; private set; }
        public int Farm { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string WidgetThumbnailUrl { get { return LargeSquare150X150Url; } }
    }
}
