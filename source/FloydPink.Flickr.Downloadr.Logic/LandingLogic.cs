using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Extensions;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.OAuth;

namespace FloydPink.Flickr.Downloadr.Logic
{
  public class LandingLogic : ILandingLogic
  {
    private readonly IOAuthManager _oAuthManager;
    private static List<Dictionary<string, object>> cachedFilteredPhotosets;

    public LandingLogic(IOAuthManager oAuthManager)
    {
      _oAuthManager = oAuthManager;
    }

    #region ILandingLogic Members

    public async Task<Photoset> GetCoverPhotoAsync(User user, Preferences preferences, bool onlyPrivate)
    {
      var extraParams = new Dictionary<string, string>
      {
        {
          ParameterNames.UserId, user.UserNsId
        },
        {
          ParameterNames.SafeSearch, preferences.SafetyLevel
        },
        {
          ParameterNames.PerPage, "1"
        },
        {
          ParameterNames.Page, "1"
        },
        {
          ParameterNames.PrivacyFilter, onlyPrivate ? "5" : "1"
          // magic numbers: https://www.flickr.com/services/api/flickr.people.getPhotos.html
        }
      };

      var photosetsResponseDictionary = (Dictionary<string, object>)
        await _oAuthManager.MakeAuthenticatedRequestAsync(Methods.PeopleGetPhotos, extraParams);

      var photo = photosetsResponseDictionary.GetPhotosResponseFromDictionary(false).Photos.FirstOrDefault();

      return photo != null
        ? new Photoset(null, null, null, null, 0, 0, 0,
          onlyPrivate ? "All Photos" : "All Public Photos", "",
          onlyPrivate ? PhotosetType.All : PhotosetType.Public,
          photo.SmallSquare75X75Url)
        : null;
    }

    public async Task<PhotosetsResponse> GetPhotosetsAsync(string methodName, User user, Preferences preferences, int page,
      IProgress<ProgressUpdate> progress)
    {
      var progressUpdate = new ProgressUpdate
      {
        OperationText = "Getting list of albums...",
        ShowPercent = false
      };
      progress.Report(progressUpdate);

      var extraParams = new Dictionary<string, string>
      {
        {
          ParameterNames.UserId, user.UserNsId
        },
        {
          ParameterNames.SafeSearch, preferences.SafetyLevel
        },
        {
          ParameterNames.PerPage, preferences.PhotosPerPage.ToString() //<-- fixed (previously there was the constant '21')
        },
        {
          ParameterNames.Page, page.ToString(CultureInfo.InvariantCulture)
        }
      };

      var photosetsResponseDictionary = (Dictionary<string, object>)
        await _oAuthManager.MakeAuthenticatedRequestAsync(methodName, extraParams);
    
      if (!String.IsNullOrWhiteSpace(preferences.AlbumSearchName))
      {
        bool refreshCache = (page == 1 && (cachedFilteredPhotosets == null || preferences.Visited));

        if (refreshCache)
        {
          await LoadCacheForMatchingAlbums(methodName, user, preferences, photosetsResponseDictionary);
        }
        return PaginatedCache(page, preferences.PhotosPerPage);
      }
      else
      {
        return photosetsResponseDictionary.GetPhotosetsResponseFromDictionary();
      }
    }

    /*
      Here we are going to get all the albums and then we'll filter only the matching names. 
    */
    private async Task LoadCacheForMatchingAlbums(string methodName, User user, Preferences preferences, Dictionary<string, object> photosetsResponseDictionary)
    {
      preferences.Visited = false;
      String albumName = preferences.AlbumSearchName.ToLower().Trim();
      bool all = albumName.Equals("*");

      cachedFilteredPhotosets = new List<Dictionary<string, object>>();
      int maxNum = int.Parse(photosetsResponseDictionary.GetSubValue("photosets", "total").ToString());

      /*
        See the behaviour for the optional 'page' parameter, from "https://www.flickr.com/services/api/explore/flickr.photosets.getList":
          - The page of results to get. Currently, if this is not provided, all sets are returned, but this behaviour may change in future.
      */
      var innerParams = new Dictionary<string, string>
      {
        {
          ParameterNames.UserId, user.UserNsId

          //let's skip the 'page' parameter
        }
      };

      //we are going to get the list of all the albums, along with their title...
      var tmpPhotosetsToBeFiltered = (Dictionary<string, object>)
         await _oAuthManager.MakeAuthenticatedRequestAsync(methodName, innerParams);

      IEnumerator<Dictionary<string, object>> browser = DictionaryExtensions.ExtractPhotosets(tmpPhotosetsToBeFiltered).GetEnumerator();

      //let's cache only the matching albums
      while (browser.MoveNext())
      {
        Dictionary<string, object> cur = browser.Current;
        if (all || cur.GetSubValue("title").ToString().ToLower().Contains(albumName))
        {
          cachedFilteredPhotosets.Add(cur);
        }
      }
    }

    private static PhotosetsResponse PaginatedCache(int pageNumber, int photosPerPage)
    {
      return DictionaryExtensions.GetPhotosetsResponseFromFilteredDictionary(pageNumber, photosPerPage, cachedFilteredPhotosets);
    }

    #endregion
  }
}
