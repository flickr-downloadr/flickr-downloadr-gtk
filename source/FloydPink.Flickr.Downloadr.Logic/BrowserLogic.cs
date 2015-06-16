namespace FloydPink.Flickr.Downloadr.Logic {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Interfaces;
    using Model;
    using Model.Constants;
    using OAuth;

    public class BrowserLogic : IBrowserLogic {
        private readonly IDownloadLogic _downloadLogic;
        private readonly IOAuthManager _oAuthManager;

        public BrowserLogic(IOAuthManager oAuthManager, IDownloadLogic downloadLogic) {
            this._oAuthManager = oAuthManager;
            this._downloadLogic = downloadLogic;
        }

        #region IBrowserLogic Members

        public async Task<PhotosResponse> GetPhotosAsync(string methodName, User user, Preferences preferences, int page,
                                                         IProgress<ProgressUpdate> progress) {
            var progressUpdate = new ProgressUpdate {
                OperationText = "Getting list of photos...",
                ShowPercent = false
            };
            progress.Report(progressUpdate);

            var extraParams = new Dictionary<string, string> {
                {
                    ParameterNames.UserId, user.UserNsId
                }, {
                    ParameterNames.SafeSearch, preferences.SafetyLevel
                }, {
                    ParameterNames.PerPage,
                    preferences.PhotosPerPage.ToString(CultureInfo.InvariantCulture)
                }, {
                    ParameterNames.Page, page.ToString(CultureInfo.InvariantCulture)
                }
            };

            var photosResponse = (Dictionary<string, object>)
                await this._oAuthManager.MakeAuthenticatedRequestAsync(methodName, extraParams);

            return photosResponse.GetPhotosResponseFromDictionary();
        }

        public async Task Download(IEnumerable<Photo> photos, CancellationToken cancellationToken,
                                   IProgress<ProgressUpdate> progress, Preferences preferences) {
            var photosList = photos as IList<Photo> ?? photos.ToList();
            if (!photosList.Any()) {
                return;
            }

            await this._downloadLogic.Download(photosList, cancellationToken, progress, preferences);
        }

        #endregion
    }
}
