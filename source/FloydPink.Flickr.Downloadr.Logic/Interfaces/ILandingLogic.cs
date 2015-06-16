namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    using System;
    using System.Threading.Tasks;
    using Model;

    public interface ILandingLogic {
        Task<PhotosetsResponse> GetPhotosetsAsync(string methodName, User user, Preferences preferences, int page,
                                                  IProgress<ProgressUpdate> progress);
    }
}
