namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Model;

    public interface IDownloadLogic {
        Task Download(IEnumerable<Photo> photos, CancellationToken cancellationToken, IProgress<ProgressUpdate> progress,
                      Preferences preferences, Photoset photoset);
    }
}
