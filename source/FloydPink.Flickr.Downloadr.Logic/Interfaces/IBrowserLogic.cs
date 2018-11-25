using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Logic.Interfaces
{
  public interface IBrowserLogic
  {
    Task<PhotosResponse> GetPhotosAsync(Photoset photoset, User user, Preferences preferences, int page,
      IProgress<ProgressUpdate> progress, string albumProgress = null);

    Task Download(IEnumerable<Photo> photos, CancellationToken cancellationToken, IProgress<ProgressUpdate> progress,
      Preferences preferences, Photoset photoset);
  }
}
