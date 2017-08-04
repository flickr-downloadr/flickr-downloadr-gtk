namespace FloydPink.Flickr.Downloadr.Model
{
  public class Session
  {
    public Session(User user, Preferences preferences, int currentAlbumPageNumber) : 
      this(user, preferences, null, currentAlbumPageNumber) { }

    public Session(User user, Preferences preferences, Photoset photoset = null, int currentAlbumPageNumber = 1)
    {
      User = user;
      Preferences = preferences;
      SelectedPhotoset = photoset;
      CurrentAlbumPageNumber = currentAlbumPageNumber;
    }

    public User User { get; set; }
    public Preferences Preferences { get; set; }
    public Photoset SelectedPhotoset { get; private set; }
    public int CurrentAlbumPageNumber { get; private set; }
  }
}
