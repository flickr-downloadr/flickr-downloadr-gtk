namespace FloydPink.Flickr.Downloadr.Model {
    public class Photoset {

        public Photoset(string id, string owner, string username, string primary, string secret, string server, int farm, int photos, int countViews, int countComments, int countPhotos, int countVideos,
        string title, string description, int coverphotoServer, int coverphotoFarm) {
            Id = id;
            Owner = owner;
            Username = username;
            Primary = primary;
            Secret = secret;
            Server = server;
            Farm = farm;
            Photos = photos;
            CountViews = countViews;
            CountComments = countComments;
            CountPhotos = countPhotos;
            CountVideos = countVideos;
            Title = title;
            Description = description;
            CoverPhotoServer = coverphotoServer;
            CoverPhotoFarm = coverphotoFarm;
        }

        public string Id { get; private set; }
        public string Owner { get; private set; }
        public string Username { get; private set; }
        public string Primary { get; private set; }
        public string Secret { get; private set; }
        public string Server { get; private set; }
        public int Farm { get; private set; }
        public int Photos { get; private set; }
        public int CountViews { get; private set; }
        public int CountComments { get; private set; }
        public int CountPhotos { get; private set; }
        public int CountVideos { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int CoverPhotoServer { get; private set; }
        public int CoverPhotoFarm { get; private set; }
    }
}
