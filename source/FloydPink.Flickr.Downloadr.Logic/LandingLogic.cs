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

            var extraParams = new Dictionary<string, string> {
                {
                    ParameterNames.UserId, user.UserNsId
                }, {
                    ParameterNames.SafeSearch, preferences.SafetyLevel
                }, {
                    ParameterNames.PerPage,
                    // TODO: Clean this up
                    // preferences.PhotosPerPage.ToString(CultureInfo.InvariantCulture)
                    "500"
                }, {
                    ParameterNames.Page, page.ToString(CultureInfo.InvariantCulture)
                }
            };

            var photosetsResponse = (Dictionary<string, object>)
                await this._oAuthManager.MakeAuthenticatedRequestAsync(methodName, extraParams);

            return photosetsResponse.GetPhotosetsResponseFromDictionary();
        }

        #endregion
    }
}
