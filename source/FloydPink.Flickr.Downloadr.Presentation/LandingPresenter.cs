using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Presentation.Views;

namespace FloydPink.Flickr.Downloadr.Presentation
{
  public class LandingPresenter : PresenterBase, ILandingPresenter
  {
    private static readonly Random Random = new Random((int)DateTime.Now.Ticks);
    private readonly ILandingLogic _logic;
    private readonly IBrowserLogic _browserLogic;
    private readonly IDownloadLogic _downloadLogic;
    private readonly Progress<ProgressUpdate> _progress = new Progress<ProgressUpdate>();
    private readonly ILandingView _view;
    private readonly IDonateIntentCheckLogic _donateIntentCheckLogic;
    private CancellationTokenSource _cancellationTokenSource;
    private string _downloadedLocation;

    public LandingPresenter(
      ILandingLogic logic,
      IBrowserLogic browserLogic,
      IDownloadLogic downloadLogic,
      IDonateIntentCheckLogic donateIntentCheckLogic,
      ILandingView view)
    {
      _logic = logic;
      _browserLogic = browserLogic;
      _downloadLogic = downloadLogic;
      _donateIntentCheckLogic = donateIntentCheckLogic;
      _view = view;
      _progress.ProgressChanged += (sender, progress) =>
      {
        _view.UpdateProgress(
          progress.ShowPercent
          ? string.Format("{0}{1}",
              progress.AlbumProgress,
              progress.PercentDone > 0 ? string.Format(" - {0}%", progress.PercentDone.ToString(CultureInfo.InvariantCulture)): string.Empty)
            : string.Empty,
          progress.OperationText, progress.Cancellable);
        _downloadedLocation = progress.DownloadedPath;
      };
    }

    public async Task DownloadSelection()
    {
      var selectedAlbumsList = _view.AllSelectedAlbums.Values.SelectMany(d => d.Values).ToList();
      await DownloadAlbums(selectedAlbumsList);
    }

    public async Task Initialize(int curAlbumPageNumber)
    {
      try
      {
        _view.ShowSpinner(true);

        _view.PublicPhotoset = await _logic.GetCoverPhotoAsync(_view.User, _view.Preferences, false);
        _view.PrivatePhotoset = await _logic.GetCoverPhotoAsync(_view.User, _view.Preferences, true);

        _view.ShowSpinner(false);

        await GetAndSetPhotosets(curAlbumPageNumber);
      } catch (Exception ex)
      {
        _view.HandleException(ex);
      }
    }

    public async Task NavigateTo(PhotoOrAlbumPage page)
    {
      var targetPage = 0;
      var currentPage = int.Parse(_view.Page);
      var totalPages = int.Parse(_view.Pages);
      switch (page)
      {
        case PhotoOrAlbumPage.First:
          if (currentPage != 1)
          {
            targetPage = 1;
          }
          break;
        case PhotoOrAlbumPage.Previous:
          if (currentPage != 1)
          {
            targetPage = currentPage - 1;
          }
          break;
        case PhotoOrAlbumPage.Next:
          if (currentPage != totalPages)
          {
            targetPage = currentPage + 1;
          }
          break;
        case PhotoOrAlbumPage.Last:
          if (currentPage != totalPages)
          {
            targetPage = totalPages;
          }
          break;
      }
      if (targetPage != 0)
      {
        await GetAndSetPhotosets(targetPage);
      }
    }

    public async Task NavigateTo(int page)
    {
      await GetAndSetPhotosets(page);
    }

    private async Task GetAndSetPhotosets(int page)
    {
      _view.ShowSpinner(true);

       SetPhotosetsResponse(await GetPhotosetsResponse(page));

      _view.ShowSpinner(false);
    }

    private async Task<PhotosetsResponse> GetPhotosetsResponse(int page)
    {
      return
        await _logic.GetPhotosetsAsync(Methods.PhotosetsGetList, _view.User, _view.Preferences, page, _progress);
    }

    private void SetPhotosetsResponse(PhotosetsResponse photosetsResponse)
    {
      _view.Page = photosetsResponse.Page.ToString(CultureInfo.InvariantCulture);
      _view.Pages = photosetsResponse.Pages.ToString(CultureInfo.InvariantCulture);
      _view.PerPage = photosetsResponse.PerPage.ToString(CultureInfo.InvariantCulture);
      _view.Total = photosetsResponse.Total.ToString(CultureInfo.InvariantCulture);
      _view.Albums = photosetsResponse.Photosets;
    }

    private bool UserAcceptedAppropriateWarning(int photosCount)
    {
      var lotOfPhotosWarningFailed = false;
      var warningFormat = string.Empty;

      if (photosCount > 1000)
      {
        warningFormat = AppConstants.MoreThan1000PhotosWarningFormat;
      }
      else if (photosCount > 500)
      {
        warningFormat = AppConstants.MoreThan500PhotosWarningFormat;
      }
      else if (photosCount > 100)
      {
        warningFormat = AppConstants.MoreThan100PhotosWarningFormat;
      }

      if (!string.IsNullOrWhiteSpace(warningFormat))
      {
        lotOfPhotosWarningFailed = _view.ShowWarning(string.Format(warningFormat,
          photosCount.ToString(CultureInfo.InvariantCulture)));
      }

      return !lotOfPhotosWarningFailed;
    }

    private async Task<IEnumerable<Photoset>> GetAllAlbums()
    {
      var pages = int.Parse(_view.Pages);
      var albums = new List<Photoset>();
      for (var page = 1; page <= pages; page++)
      {
        albums.AddRange((await GetPhotosetsResponse(page)).Photosets);
      }
      return albums;
    }

    private async Task DownloadAlbums(IEnumerable<Photoset> albums, bool handleSpinner = true)
    {
      try
      {
        if (handleSpinner)
        {
          _view.ShowSpinner(true);
        }

        var albumsList = albums as IList<Photoset> ?? albums.ToList();
        var albumsDictionary = new Dictionary<Photoset, List<Photo>>();
        var allPhotosCount = albumsList.Select(a => a.Photos).Sum();

        _cancellationTokenSource = new CancellationTokenSource();

        if (UserAcceptedAppropriateWarning(allPhotosCount))
        {
          var count = 0;
          foreach (var album in albumsList)
          {
            var albumProgress = string.Format("{0} ({1} of {2})", album.Title, ++count, albumsList.Count);
            var photos = await GetAllPhotosInAlbum(album, albumProgress);
            albumsDictionary.Add(album, photos);
          }

          var folderPrefix = string.Format("flickr-downloadr-{0}", GetSafeFilename(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));

          count = 0;
          foreach (var album in albumsDictionary)
          {
            var albumProgress = string.Format("{0} ({1} of {2})", album.Key.Title, ++count, albumsDictionary.Count);
            await _downloadLogic.Download(album.Value, _cancellationTokenSource.Token, _progress, _view.Preferences, album.Key, folderPrefix, albumProgress);
          }

          var intent = _donateIntentCheckLogic.DonateIntentAvailable(allPhotosCount);
          _view.DownloadComplete(_downloadedLocation, true, intent);
        }
      }
      catch (OperationCanceledException)
      {
        _view.DownloadComplete(_downloadedLocation, false);
      }
      if (handleSpinner)
      {
        _view.ShowSpinner(false);
      }
    }

    private async Task<List<Photo>> GetAllPhotosInAlbum(Photoset album, string albumProgress)
    {
      var photos = new List<Photo>();

      // get first page on the album
      var photosResponse = await GetPhotosResponse(album, albumProgress);
      photos.AddRange(photosResponse.Photos);

      // get remaining pages
      for (var page = 2; page <= photosResponse.Pages; page++)
      {
        photos.AddRange((await GetPhotosResponse(album, albumProgress, page)).Photos);
      }

      return photos;
    }

    private async Task<PhotosResponse> GetPhotosResponse(Photoset album, string albumProgress, int page = 1)
    {
      return await _browserLogic.GetPhotosAsync(album, _view.User, _view.Preferences, page, _progress, albumProgress);
    }

    private static string RandomString(int size)
    {
      // http://stackoverflow.com/a/1122519/218882
      var builder = new StringBuilder();
      for (var i = 0; i < size; i++)
      {
        var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65), CultureInfo.InvariantCulture));
        builder.Append(ch);
      }

      return builder.ToString();
    }

    private static string GetSafeFilename(string path)
    {
      // http://stackoverflow.com/a/333297/218882
      var safeFilename = Path.GetInvalidFileNameChars()
        .Aggregate(path, (current, c) => current.Replace(c, '-'));
      return string.IsNullOrWhiteSpace(safeFilename) ? RandomString(8) : safeFilename;
    }


  }
}
