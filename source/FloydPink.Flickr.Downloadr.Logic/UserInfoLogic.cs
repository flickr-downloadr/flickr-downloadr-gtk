namespace FloydPink.Flickr.Downloadr.Logic {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Extensions;
    using Interfaces;
    using Model;
    using Model.Constants;
    using OAuth;

    public class UserInfoLogic : IUserInfoLogic {
        private readonly IOAuthManager _oAuthManager;

        public UserInfoLogic(IOAuthManager oAuthManager) {
            this._oAuthManager = oAuthManager;
        }

        public async Task<User> PopulateUserInfo(User user) {
            var exraParams = new Dictionary<string, string> {
                {
                    ParameterNames.UserId, user.UserNsId
                }
            };

            dynamic userWithUserInfo = await this._oAuthManager.MakeAuthenticatedRequestAsync(Methods.PeopleGetInfo, exraParams);

            var userInfo = (Dictionary<string, object>) userWithUserInfo["person"];

            user.Info = new UserInfo {
                Id = user.UserNsId,
                IsPro = Convert.ToBoolean(userInfo["ispro"]),
                IconServer = userInfo["iconserver"].ToString(),
                IconFarm = int.Parse(userInfo["iconfarm"].ToString()),
                PathAlias =
                    userInfo["path_alias"] == null
                        ? string.Empty
                        : userInfo["path_alias"].ToString(),
                Description = userInfo.GetSubValue("description").ToString(),
                PhotosUrl = userInfo.GetSubValue("photosurl").ToString(),
                ProfileUrl = userInfo.GetSubValue("profileurl").ToString(),
                MobileUrl = userInfo.GetSubValue("mobileurl").ToString(),
                PhotosCount = int.Parse(((Dictionary<string, object>) userInfo["photos"]).GetSubValue("count").ToString())
            };

            return user;
        }
    }
}
