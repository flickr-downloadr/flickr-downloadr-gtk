﻿using System.Collections.Generic;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Extensions;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.OAuth;

namespace FloydPink.Flickr.Downloadr.Logic
{
  public class OriginalTagsLogic : IOriginalTagsLogic
  {
    private readonly IOAuthManager _oAuthManager;

    public OriginalTagsLogic(IOAuthManager oAuthManager)
    {
      _oAuthManager = oAuthManager;
    }

    public async Task<Photo> GetOriginalTagsTask(Photo photo, Preferences preferences)
    {
      var extraParams = new Dictionary<string, string>
      {
        {
          ParameterNames.PhotoId, photo.Id
        },
        {
          ParameterNames.Secret, photo.Secret
        }
      };

      var photoResponse =
        (Dictionary<string, object>)
        await _oAuthManager.MakeAuthenticatedRequestAsync(Methods.PhotosGetInfo, extraParams);

      // Override the internal tags with the original ones
      photo.Tags = string.Join(", ", photoResponse.ExtractOriginalTags());

      if (preferences.NeedLocationMetadata)
      {
        photo.Location = photoResponse.ExtractLocationDetails();
      }

      return photo;
    }
  }
}
