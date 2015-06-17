namespace FloydPink.Flickr.Downloadr.Model {
    public interface IPhotoWidget {
        string Id { get; }
        string Secret { get; }
        string Server { get; }
        int Farm { get; }
        string Title { get; }
        string Description { get; }
   }
}

