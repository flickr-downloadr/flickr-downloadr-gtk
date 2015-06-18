namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;

    public interface IBrowserLogic {
        Task<PhotosResponse> GetPhotosAsync(Photoset photoset, User user, Preferences preferences, int page,
                                            IProgress<ProgressUpdate> progress);

        Task Download(IEnumerable<Photo> photos, CancellationToken cancellationToken, IProgress<ProgressUpdate> progress,
                      Preferences preferences, Photoset photoset);
    }
}
