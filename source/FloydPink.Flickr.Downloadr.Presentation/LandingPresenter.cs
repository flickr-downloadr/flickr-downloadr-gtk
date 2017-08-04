using System;
using System.Globalization;
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
    private readonly ILandingLogic _logic;
    private readonly Progress<ProgressUpdate> _progress = new Progress<ProgressUpdate>();
    private readonly ILandingView _view;

    public LandingPresenter(ILandingLogic logic, ILandingView view)
    {
      _logic = logic;
      _view = view;
      _progress.ProgressChanged += (sender, progress) =>
      {
        _view.UpdateProgress(
          progress.ShowPercent
            ? string.Format("{0}%",
              progress.PercentDone.ToString(
                CultureInfo.InvariantCulture))
            : string.Empty,
          progress.OperationText, progress.Cancellable);
      };
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
  }
}
