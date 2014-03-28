using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Presentation.Views;

namespace FloydPink.Flickr.Downloadr.Presentation
{
    public class BrowserPresenter : PresenterBase, IBrowserPresenter
    {
        private readonly IBrowserLogic _logic;
        private readonly Progress<ProgressUpdate> _progress = new Progress<ProgressUpdate>();
        private readonly IBrowserView _view;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _downloadComplete;
        private string _downloadedLocation;

        public BrowserPresenter(IBrowserLogic logic, IBrowserView view)
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
                _downloadedLocation = progress.DownloadedPath;
                _downloadComplete = progress.PercentDone == 100;
            };
        }

        public async Task InitializePhotoset()
        {
            await GetAndSetPhotos(1);
        }

        public async Task NavigateTo(PhotoPage page)
        {
            int targetPage = 0;
            int currentPage = Convert.ToInt32(_view.Page);
            int totalPages = Convert.ToInt32(_view.Pages);
            switch (page)
            {
                case PhotoPage.First:
                    if (currentPage != 1) targetPage = 1;
                    break;
                case PhotoPage.Previous:
                    if (currentPage != 1) targetPage = currentPage - 1;
                    break;
                case PhotoPage.Next:
                    if (currentPage != totalPages) targetPage = currentPage + 1;
                    break;
                case PhotoPage.Last:
                    if (currentPage != totalPages) targetPage = totalPages;
                    break;
            }
            if (targetPage != 0) await GetAndSetPhotos(targetPage);
        }

        public void CancelDownload()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
                _cancellationTokenSource.Cancel();
        }

        public async Task DownloadSelection()
        {
            await DownloadPhotos(_view.AllSelectedPhotos.Values.SelectMany(d => d.Values).ToList());
        }

        public async Task DownloadThisPage()
        {
            await DownloadPhotos(_view.Photos);
        }

        public async Task DownloadAllPages()
        {
            _view.ShowSpinner(true);

            IEnumerable<Photo> photos = await GetAllPhotos();

            await DownloadPhotos(photos, false);

            _view.ShowSpinner(false);
        }

        private async Task<IEnumerable<Photo>> GetAllPhotos()
        {
            int pages = Convert.ToInt32(_view.Pages);
            var photos = new List<Photo>();
            for (int page = 1; page <= pages; page++)
            {
                photos.AddRange((await GetPhotosResponse(page)).Photos);
            }
            return photos;
        }

        private async Task DownloadPhotos(IEnumerable<Photo> photos, bool handleSpinner = true)
        {
            if (handleSpinner) _view.ShowSpinner(true);

            IList<Photo> photosList = photos as IList<Photo> ?? photos.ToList();
            bool lotOfPhotosWarningFailed = false;
            string warningFormat = string.Empty;

            if (photosList.Count() > 1000)
            {
                warningFormat = AppConstants.MoreThan1000PhotosWarningFormat;
            }
            else if (photosList.Count() > 500)
            {
                warningFormat = AppConstants.MoreThan500PhotosWarningFormat;
            }
            else if (photosList.Count() > 100)
            {
                warningFormat = AppConstants.MoreThan100PhotosWarningFormat;
            }

            if (!string.IsNullOrWhiteSpace(warningFormat))
            {
                lotOfPhotosWarningFailed = _view.ShowWarning(string.Format(warningFormat,
                    photosList.Count()
                        .ToString(
                            CultureInfo.InvariantCulture)));
            }

            if (!lotOfPhotosWarningFailed)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await _logic.Download(photosList, _cancellationTokenSource.Token, _progress, _view.Preferences);
                _view.DownloadComplete(_downloadedLocation, _downloadComplete);
            }

            if (handleSpinner) _view.ShowSpinner(false);
        }

        private async Task GetAndSetPhotos(int page)
        {
            _view.ShowSpinner(true);

            SetPhotoResponse(await GetPhotosResponse(page));

            _view.ShowSpinner(false);
        }

        private async Task<PhotosResponse> GetPhotosResponse(int page)
        {
            string methodName = _view.ShowAllPhotos ? Methods.PeopleGetPhotos : Methods.PeopleGetPublicPhotos;
            return await _logic.GetPhotosAsync(methodName, _view.User, _view.Preferences, page, _progress);
        }

        private void SetPhotoResponse(PhotosResponse photosResponse)
        {
            _view.Page = photosResponse.Page.ToString(CultureInfo.InvariantCulture);
            _view.Pages = photosResponse.Pages.ToString(CultureInfo.InvariantCulture);
            _view.PerPage = photosResponse.PerPage.ToString(CultureInfo.InvariantCulture);
            _view.Total = photosResponse.Total.ToString(CultureInfo.InvariantCulture);
            _view.Photos = new ObservableCollection<Photo>(photosResponse.Photos);
        }
    }
}