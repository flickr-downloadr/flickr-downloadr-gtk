using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Extensions;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.OAuth;

namespace FloydPink.Flickr.Downloadr.Logic
{
  public class UserInfoLogic : IUserInfoLogic
  {
    private readonly IOAuthManager _oAuthManager;

    public UserInfoLogic(IOAuthManager oAuthManager)
    {
      _oAuthManager = oAuthManager;
    }

    public async Task<User> PopulateUserInfo(User user)
    {
      var exraParams = new Dictionary<string, string>
      {
        {
          ParameterNames.UserId, user.UserNsId
        }
      };

      dynamic userWithUserInfo = await _oAuthManager.MakeAuthenticatedRequestAsync(Methods.PeopleGetInfo, exraParams);

      var userInfo = (Dictionary<string, object>) userWithUserInfo["person"];

      user.Info = new UserInfo
      {
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
