namespace FloydPink.Flickr.Downloadr.Presentation {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logic.Interfaces;
    using Model;
    using Model.Constants;
    using Model.Enums;
    using Views;

    public class BrowserPresenter : PresenterBase, IBrowserPresenter {
        private CancellationTokenSource _cancellationTokenSource;
        private string _downloadedLocation;
        private readonly IBrowserLogic _logic;
        private readonly Progress<ProgressUpdate> _progress = new Progress<ProgressUpdate>();
        private readonly IBrowserView _view;

        public BrowserPresenter(IBrowserLogic logic, IBrowserView view) {
            _logic = logic;
            _view = view;
            _progress.ProgressChanged += (sender, progress) => {
                                             _view.UpdateProgress(
                                                 progress.ShowPercent
                                                     ? string.Format("{0}%",
                                                         progress.PercentDone.ToString(
                                                             CultureInfo.InvariantCulture))
                                                     : string.Empty,
                                                 progress.OperationText, progress.Cancellable);
                                             _downloadedLocation = progress.DownloadedPath;
                                         };
        }

        public async Task InitializePhotoset() {
            await GetAndSetPhotos(1);
        }

        public async Task NavigateTo(PhotoPage page) {
            var targetPage = 0;
            var currentPage = int.Parse(_view.Page);
            var totalPages = int.Parse(_view.Pages);
            switch (page) {
                case PhotoPage.First:
                    if (currentPage != 1) {
                        targetPage = 1;
                    }
                    break;
                case PhotoPage.Previous:
                    if (currentPage != 1) {
                        targetPage = currentPage - 1;
                    }
                    break;
                case PhotoPage.Next:
                    if (currentPage != totalPages) {
                        targetPage = currentPage + 1;
                    }
                    break;
                case PhotoPage.Last:
                    if (currentPage != totalPages) {
                        targetPage = totalPages;
                    }
                    break;
            }
            if (targetPage != 0) {
                await GetAndSetPhotos(targetPage);
            }
        }

        public async Task NavigateTo(int page) {
            await GetAndSetPhotos(page);
        }

        public void CancelDownload() {
            if (!_cancellationTokenSource.IsCancellationRequested) {
                _cancellationTokenSource.Cancel();
            }
        }

        public async Task DownloadSelection() {
            var selectedPhotosList = _view.AllSelectedPhotos.Values.SelectMany(d => d.Values).ToList();
            if (UserAcceptedAppropriateWarning(selectedPhotosList.Count)) {
                await DownloadPhotos(selectedPhotosList);
            }
        }

        public async Task DownloadThisPage() {
            if (UserAcceptedAppropriateWarning(_view.Photos.Count())) {
                await DownloadPhotos(_view.Photos);
            }
        }

        public async Task DownloadAllPages() {
            if (UserAcceptedAppropriateWarning(int.Parse(_view.Total))) {
                _view.ShowSpinner(true);

                var photos = await GetAllPhotos();

                await DownloadPhotos(photos, false);

                _view.ShowSpinner(false);
            }
        }

        private bool UserAcceptedAppropriateWarning(int photosCount) {
            var lotOfPhotosWarningFailed = false;
            var warningFormat = string.Empty;

            if (photosCount > 1000) {
                warningFormat = AppConstants.MoreThan1000PhotosWarningFormat;
            } else if (photosCount > 500) {
                warningFormat = AppConstants.MoreThan500PhotosWarningFormat;
            } else if (photosCount > 100) {
                warningFormat = AppConstants.MoreThan100PhotosWarningFormat;
            }

            if (!string.IsNullOrWhiteSpace(warningFormat)) {
                lotOfPhotosWarningFailed = _view.ShowWarning(string.Format(warningFormat,
                    photosCount.ToString(CultureInfo.InvariantCulture)));
            }

            return !lotOfPhotosWarningFailed;
        }

        private async Task<IEnumerable<Photo>> GetAllPhotos() {
            var pages = int.Parse(_view.Pages);
            var photos = new List<Photo>();
            for (var page = 1; page <= pages; page++) {
                photos.AddRange((await GetPhotosResponse(page)).Photos);
            }
            return photos;
        }

        private async Task DownloadPhotos(IEnumerable<Photo> photos, bool handleSpinner = true) {
            try {
                if (handleSpinner) {
                    _view.ShowSpinner(true);
                }

                var photosList = photos as IList<Photo> ?? photos.ToList();

                _cancellationTokenSource = new CancellationTokenSource();
                await
                    _logic.Download(photosList, _cancellationTokenSource.Token, _progress,
                        _view.Preferences);
                _view.DownloadComplete(_downloadedLocation, true);
            }
            catch (OperationCanceledException) {
                _view.DownloadComplete(_downloadedLocation, false);
            }
            if (handleSpinner) {
                _view.ShowSpinner(false);
            }
        }

        private async Task GetAndSetPhotos(int page) {
            _view.ShowSpinner(true);

            SetPhotoResponse(await GetPhotosResponse(page));

            _view.ShowSpinner(false);
        }

        private string GetPhotosetMethodName(PhotosetType photoset) {
            switch (photoset) {
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

        private async Task<PhotosResponse> GetPhotosResponse(int page) {
            var methodName = GetPhotosetMethodName(_view.PhotosetType);
            return
                await
                    _logic.GetPhotosAsync(methodName, _view.User, _view.Preferences, page, _progress);
        }

        private void SetPhotoResponse(PhotosResponse photosResponse) {
            _view.Page = photosResponse.Page.ToString(CultureInfo.InvariantCulture);
            _view.Pages = photosResponse.Pages.ToString(CultureInfo.InvariantCulture);
            _view.PerPage = photosResponse.PerPage.ToString(CultureInfo.InvariantCulture);
            _view.Total = photosResponse.Total.ToString(CultureInfo.InvariantCulture);
            _view.Photos = photosResponse.Photos;
        }
    }
}
