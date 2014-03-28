using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Logic.Interfaces
{
    public interface IBrowserLogic
    {
        Task<PhotosResponse> GetPhotosAsync(string methodName, User user, Preferences preferences, int page,
            IProgress<ProgressUpdate> progress);

        Task Download(IEnumerable<Photo> photos, CancellationToken cancellationToken, IProgress<ProgressUpdate> progress,
            Preferences preferences);
    }
}