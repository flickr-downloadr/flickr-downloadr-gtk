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
        private readonly IBrowserLogic _logic;
        private readonly Progress<ProgressUpdate> _progress = new Progress<ProgressUpdate>();
        private readonly IBrowserView _view;
        private CancellationTokenSource _cancellationTokenSource;
        private string _downloadedLocation;

        public BrowserPresenter(IBrowserLogic logic, IBrowserView view) {
            this._logic = logic;
            this._view = view;
            this._progress.ProgressChanged += (sender, progress) => {
                                                  this._view.UpdateProgress(
                                                      progress.ShowPercent
                                                          ? string.Format("{0}%",
                                                              progress.PercentDone.ToString(
                                                                  CultureInfo.InvariantCulture))
                                                          : string.Empty,
                                                      progress.OperationText, progress.Cancellable);
                                                  this._downloadedLocation = progress.DownloadedPath;
                                              };
        }

        public async Task InitializePhotoset() {
            try {
                await GetAndSetPhotos(1);
            }
            catch (Exception ex) {
                _view.HandleException(ex);
            }
        }

        public async Task NavigateTo(PhotoOrAlbumPage page) {
            var targetPage = 0;
            var currentPage = int.Parse(this._view.Page);
            var totalPages = int.Parse(this._view.Pages);
            switch (page) {
                case PhotoOrAlbumPage.First:
                    if (currentPage != 1) {
                        targetPage = 1;
                    }
                    break;
                case PhotoOrAlbumPage.Previous:
                    if (currentPage != 1) {
                        targetPage = currentPage - 1;
                    }
                    break;
                case PhotoOrAlbumPage.Next:
                    if (currentPage != totalPages) {
                        targetPage = currentPage + 1;
                    }
                    break;
                case PhotoOrAlbumPage.Last:
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
            if (!this._cancellationTokenSource.IsCancellationRequested) {
                this._cancellationTokenSource.Cancel();
            }
        }

        public async Task DownloadSelection() {
            var selectedPhotosList = this._view.AllSelectedPhotos.Values.SelectMany(d => d.Values).ToList();
            if (UserAcceptedAppropriateWarning(selectedPhotosList.Count)) {
                await DownloadPhotos(selectedPhotosList);
            }
        }

        public async Task DownloadThisPage() {
            if (UserAcceptedAppropriateWarning(this._view.Photos.Count())) {
                await DownloadPhotos(this._view.Photos);
            }
        }

        public async Task DownloadAllPages() {
            if (UserAcceptedAppropriateWarning(int.Parse(this._view.Total))) {
                this._view.ShowSpinner(true);

                var photos = await GetAllPhotos();

                await DownloadPhotos(photos, false);

                this._view.ShowSpinner(false);
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
                lotOfPhotosWarningFailed = this._view.ShowWarning(string.Format(warningFormat,
                    photosCount.ToString(CultureInfo.InvariantCulture)));
            }

            return !lotOfPhotosWarningFailed;
        }

        private async Task<IEnumerable<Photo>> GetAllPhotos() {
            var pages = int.Parse(this._view.Pages);
            var photos = new List<Photo>();
            for (var page = 1; page <= pages; page++) {
                photos.AddRange((await GetPhotosResponse(page)).Photos);
            }
            return photos;
        }

        private async Task DownloadPhotos(IEnumerable<Photo> photos, bool handleSpinner = true) {
            try {
                if (handleSpinner) {
                    this._view.ShowSpinner(true);
                }

                var photosList = photos as IList<Photo> ?? photos.ToList();

                this._cancellationTokenSource = new CancellationTokenSource();
                await
                    this._logic.Download(photosList, this._cancellationTokenSource.Token, this._progress, this._view.Preferences,
                        this._view.CurrentPhotoset);
                this._view.DownloadComplete(this._downloadedLocation, true);
            }
            catch (OperationCanceledException) {
                this._view.DownloadComplete(this._downloadedLocation, false);
            }
            if (handleSpinner) {
                this._view.ShowSpinner(false);
            }
        }

        private async Task GetAndSetPhotos(int page) {
            this._view.ShowSpinner(true);

            SetPhotosResponse(await GetPhotosResponse(page));

            this._view.ShowSpinner(false);
        }

        private async Task<PhotosResponse> GetPhotosResponse(int page) {
            return
                await this._logic.GetPhotosAsync(this._view.CurrentPhotoset, this._view.User, this._view.Preferences, page, this._progress);
        }

        private void SetPhotosResponse(PhotosResponse photosResponse) {
            this._view.Page = photosResponse.Page.ToString(CultureInfo.InvariantCulture);
            this._view.Pages = photosResponse.Pages.ToString(CultureInfo.InvariantCulture);
            this._view.PerPage = photosResponse.PerPage.ToString(CultureInfo.InvariantCulture);
            this._view.Total = photosResponse.Total.ToString(CultureInfo.InvariantCulture);
            this._view.Photos = photosResponse.Photos;
        }
    }
}
