namespace FloydPink.Flickr.Downloadr.Logic {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Interfaces;
    using Model;
    using Model.Constants;
    using Model.Enums;
    using OAuth;

    public class LandingLogic : ILandingLogic {
        private readonly IOAuthManager _oAuthManager;

        public LandingLogic(IOAuthManager oAuthManager) {
            this._oAuthManager = oAuthManager;
        }

        #region ILandingLogic Members

        public async Task<Photoset> GetCoverPhotoAsync(User user, Preferences preferences, bool onlyPrivate) {
            var extraParams = new Dictionary<string, string> {
                {
                    ParameterNames.UserId, user.UserNsId
                }, {
                    ParameterNames.SafeSearch, preferences.SafetyLevel
                }, {
                    ParameterNames.PerPage, "1"
                }, {
                    ParameterNames.Page, "1"
                }, {
                    ParameterNames.PrivacyFilter, onlyPrivate ? "5" : "1"
                    // magic numbers: https://www.flickr.com/services/api/flickr.people.getPhotos.html
                }
            };

            var photosetsResponseDictionary = (Dictionary<string, object>)
                await this._oAuthManager.MakeAuthenticatedRequestAsync(Methods.PeopleGetPhotos, extraParams);

            var photo = photosetsResponseDictionary.GetPhotosResponseFromDictionary(false).Photos.FirstOrDefault();

            return photo != null
                ? new Photoset(null, null, null, null, 0, 0, 0,
                    onlyPrivate ? "All Photos" : "All Public Photos", "",
                    onlyPrivate ? PhotosetType.All : PhotosetType.Public,
                    photo.SmallSquare75X75Url)
                : null;
        }

        public async Task<PhotosetsResponse> GetPhotosetsAsync(string methodName, User user, Preferences preferences, int page,
                                                               IProgress<ProgressUpdate> progress) {
            var progressUpdate = new ProgressUpdate {
                OperationText = "Getting list of albums...",
                ShowPercent = false
            };
            progress.Report(progressUpdate);

            var extraParams = new Dictionary<string, string> {
                {
                    ParameterNames.UserId, user.UserNsId
                }, {
                    ParameterNames.SafeSearch, preferences.SafetyLevel
                }, {
                    ParameterNames.PerPage, "21"
                }, {
                    ParameterNames.Page, page.ToString(CultureInfo.InvariantCulture)
                }
            };

            var photosetsResponseDictionary = (Dictionary<string, object>)
                await this._oAuthManager.MakeAuthenticatedRequestAsync(methodName, extraParams);

            return photosetsResponseDictionary.GetPhotosetsResponseFromDictionary();
        }

        #endregion
    }
}
