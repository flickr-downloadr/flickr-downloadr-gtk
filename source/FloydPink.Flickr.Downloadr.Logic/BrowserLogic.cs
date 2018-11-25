using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Extensions;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.OAuth;

namespace FloydPink.Flickr.Downloadr.Logic
{
  public class BrowserLogic : IBrowserLogic
  {
    private readonly IDownloadLogic _downloadLogic;
    private readonly IOAuthManager _oAuthManager;

    public BrowserLogic(IOAuthManager oAuthManager, IDownloadLogic downloadLogic)
    {
      _oAuthManager = oAuthManager;
      _downloadLogic = downloadLogic;
    }

    private string GetPhotosetMethodName(PhotosetType photoset)
    {
      switch (photoset)
      {
        case PhotosetType.All:
          return Methods.PeopleGetPhotos;
        case PhotosetType.Public:
          return Methods.PeopleGetPublicPhotos;
        case PhotosetType.Album:
          return Methods.PhotosetsGetPhotos;
        default:
          return null;
      }
    }

    #region IBrowserLogic Members

    public async Task<PhotosResponse> GetPhotosAsync(Photoset photoset, User user, Preferences preferences, int page,
                     IProgress<ProgressUpdate> progress, string albumProgress = null)
    {
      var isGettingAlbumPhotos = !string.IsNullOrEmpty(albumProgress);
      var progressUpdate = new ProgressUpdate
      {
        OperationText = isGettingAlbumPhotos ? "Getting photos in album..." : "Getting list of photos...",
        ShowPercent = isGettingAlbumPhotos,
        PercentDone = 0,
        AlbumProgress = albumProgress
      };
      progress.Report(progressUpdate);

      var methodName = GetPhotosetMethodName(photoset.Type);

      var extraParams = new Dictionary<string, string>
      {
        {
          ParameterNames.UserId, user.UserNsId
        },
        {
          ParameterNames.SafeSearch, preferences.SafetyLevel
        },
        {
          ParameterNames.PerPage,
          preferences.PhotosPerPage.ToString(CultureInfo.InvariantCulture)
        },
        {
          ParameterNames.Page, page.ToString(CultureInfo.InvariantCulture)
        }
      };

      var isAlbum = photoset.Type == PhotosetType.Album;
      if (isAlbum)
      {
        extraParams.Add(ParameterNames.PhotosetId, photoset.Id);
      }

      var photosResponse = (Dictionary<string, object>)
        await _oAuthManager.MakeAuthenticatedRequestAsync(methodName, extraParams);

      return photosResponse.GetPhotosResponseFromDictionary(isAlbum);
    }

    public async Task Download(IEnumerable<Photo> photos, CancellationToken cancellationToken, IProgress<ProgressUpdate> progress,
      Preferences preferences, Photoset photoset)
    {
      var photosList = photos as IList<Photo> ?? photos.ToList();
      if (!photosList.Any())
      {
        return;
      }

      await _downloadLogic.Download(photosList, cancellationToken, progress, preferences, photoset);
    }

    #endregion
  }
}
