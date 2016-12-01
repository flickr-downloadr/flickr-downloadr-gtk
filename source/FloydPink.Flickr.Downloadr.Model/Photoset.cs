using System.Web;
using FloydPink.Flickr.Downloadr.Model.Enums;

namespace FloydPink.Flickr.Downloadr.Model
{
  public class Photoset : IGridWidgetItem
  {
    public Photoset(string id, string primary, string secret, string server, int farm, int photos, int videos, string title,
      string description, PhotosetType type, string coverPhotoUrl)
    {
      Id = id;
      Primary = primary;
      Secret = secret;
      Server = server;
      Farm = farm;
      Photos = photos;
      Videos = videos;
      Title = title;
      Description = description;
      Type = type;
      CoverPhotoUrl = coverPhotoUrl;
    }

    public string Primary { get; }
    public int Photos { get; private set; }
    public int Videos { get; private set; }
    public PhotosetType Type { get; }
    public string CoverPhotoUrl { get; }

    public string HtmlEncodedTitle
    {
      get
      {
        return HttpUtility.HtmlEncode(Title);
      }
    }

    public string Id { get; }
    public string Secret { get; }
    public string Server { get; }
    public int Farm { get; }
    public string Title { get; }
    public string Description { get; }

    public string WidgetThumbnailUrl
    {
      get
      {
        if (Type == PhotosetType.Album)
        {
          return string.Format("https://farm{0}.staticflickr.com/{1}/{2}_{3}_s.jpg",
            Farm, Server, Primary, Secret);
        }
        return CoverPhotoUrl;
      }
    }
  }
}
