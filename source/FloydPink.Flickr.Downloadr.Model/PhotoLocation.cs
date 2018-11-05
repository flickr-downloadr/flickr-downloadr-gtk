namespace FloydPink.Flickr.Downloadr.Model
{
  public class PhotoLocation
  {
    public PhotoLocation(int accuracy, double latitude, double longitude, string country = null, string region = null,
      string county = null, string locality = null)
    {
      Accuracy = accuracy;
      Latitude = latitude;
      Longitude = longitude;
      Country = country;
      Region = region;
      County = county;
      Locality = locality;
    }

    // Refer here: https://www.flickr.com/services/api/flickr.photos.geo.photosForLocation.html
    public int Accuracy { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public string Country { get; }
    public string Region { get; }
    public string County { get; }
    public string Locality { get; }
  }
}
