using System;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Logic.Interfaces
{
  public interface ILandingLogic
  {
    Task<Photoset> GetCoverPhotoAsync(User user, Preferences preferences, bool onlyPrivate);

    Task<PhotosetsResponse> GetPhotosetsAsync(string methodName, User user, Preferences preferences, int page,
      IProgress<ProgressUpdate> progress);
  }
}
