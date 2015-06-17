namespace FloydPink.Flickr.Downloadr.Logic {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Extensions;
    using Interfaces;
    using Model;
    using Model.Constants;
    using OAuth;

    public class LandingLogic : ILandingLogic {
        private readonly IOAuthManager _oAuthManager;

        public LandingLogic(IOAuthManager oAuthManager) {
            this._oAuthManager = oAuthManager;
        }

        #region IBrowserLogic Members

        public async Task<PhotosetsResponse> GetPhotosetsAsync(string methodName, User user, Preferences preferences, int page,
                                                               IProgress<ProgressUpdate> progress) {
            var progressUpdate = new ProgressUpdate {
                OperationText = "Getting list of albums...",
                ShowPercent = false
            };
            progress.Report(progressUpdate);

            var extraParams = new Dictionary<string, string> { {
                    ParameterNames.UserId, user.UserNsId
                }, {
                    ParameterNames.SafeSearch, preferences.SafetyLevel
                }, {
                    ParameterNames.PerPage,
                    // TODO: Clean this up
                    // preferences.PhotosPerPage.ToString(CultureInfo.InvariantCulture)
                    "10"
                }, {
                    ParameterNames.Page, page.ToString(CultureInfo.InvariantCulture)
                }
            };

            var photosetsResponseDictionary = (Dictionary<string, object>)
                await this._oAuthManager.MakeAuthenticatedRequestAsync(methodName, extraParams);

            var photosetsResponse = photosetsResponseDictionary.GetPhotosetsResponseFromDictionary();

            photosetsResponse.Photosets = await RetrieveCoverPhoto(photosetsResponse.Photosets, progress);

            return photosetsResponse;
        }

        #endregion

        private async Task<IEnumerable<Photoset>> RetrieveCoverPhoto(IEnumerable<Photoset> photosets, IProgress<ProgressUpdate> progress) {
            var photosetsList = new List<Photoset>();

            var progressUpdate = new ProgressUpdate {
                OperationText = "Getting album photos...",
                ShowPercent = false
            };
            progress.Report(progressUpdate);

            foreach (var photoset in photosets) {
                var extraParams = new Dictionary<string, string> { {
                    ParameterNames.PhotoId, photoset.Primary
                }, {
                    ParameterNames.Secret, photoset.Secret
                }
            };

                var photoResponse =                (Dictionary<string, object>)
                    await this._oAuthManager.MakeAuthenticatedRequestAsync(Methods.PhotosGetInfo, extraParams);

                    photoset.CoverPhoto = photoResponse.ExtractPhoto();

                    photosetsList.Add(photoset);
                
            }

            return photosetsList;
        }
    }
}
