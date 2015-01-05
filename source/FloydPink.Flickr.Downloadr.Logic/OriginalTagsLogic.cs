namespace FloydPink.Flickr.Downloadr.Logic {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Extensions;
    using Interfaces;
    using Model;
    using Model.Constants;
    using OAuth;

    public class OriginalTagsLogic : IOriginalTagsLogic {
        private readonly IOAuthManager _oAuthManager;

        public OriginalTagsLogic(IOAuthManager oAuthManager) {
            _oAuthManager = oAuthManager;
        }

        public async Task<Photo> GetOriginalTagsTask(Photo photo) {
            var extraParams = new Dictionary<string, string> {
                {
                    ParameterNames.PhotoId, photo.Id
                }, {
                    ParameterNames.Secret, photo.Secret
                }
            };

            var photoResponse =
                (Dictionary<string, object>)
                    await _oAuthManager.MakeAuthenticatedRequestAsync(Methods.PhotosGetInfo, extraParams);

            // Override the internal tags with the original ones
            photo.Tags = string.Join(", ", photoResponse.ExtractOriginalTags());

            return photo;
        }
    }
}
