using System.Collections.Generic;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Presentation.Views
{
  public interface ILandingView : IBaseView
  {
    User User { get; set; }
    Preferences Preferences { get; set; }
    string Page { get; set; }
    string Pages { get; set; }
    string PerPage { get; set; }
    string Total { get; set; }
    Photoset PublicPhotoset { get; set; }
    Photoset PrivatePhotoset { get; set; }
    IEnumerable<Photoset> Albums { get; set; }
    IDictionary<string, Dictionary<string, Photoset>> AllSelectedAlbums { get; set; }
    void UpdateProgress(string percentDone, string operationText, bool cancellable);
    bool ShowWarning(string warningMessage);
    void DownloadComplete(string downloadedLocation, bool downloadComplete, DonateIntent intent = null);
  }
}
