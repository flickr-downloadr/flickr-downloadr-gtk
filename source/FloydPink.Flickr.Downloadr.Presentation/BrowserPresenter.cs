using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Presentation.Views;

namespace FloydPink.Flickr.Downloadr.Presentation {
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
            await GetAndSetPhotos(1);
        }

        public async Task NavigateTo(PhotoPage page) {
            int targetPage = 0;
            int currentPage = Convert.ToInt32(this._view.Page);
            int totalPages = Convert.ToInt32(this._view.Pages);
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

        public void CancelDownload() {
            if (!this._cancellationTokenSource.IsCancellationRequested) {
                this._cancellationTokenSource.Cancel();
            }
        }

        public async Task DownloadSelection() {
            List<Photo> selectedPhotosList = this._view.AllSelectedPhotos.Values.SelectMany(d => d.Values).ToList();
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

                IEnumerable<Photo> photos = await GetAllPhotos();

                await DownloadPhotos(photos, false);

                this._view.ShowSpinner(false);
            }
        }

        private bool UserAcceptedAppropriateWarning(int photosCount) {
            bool lotOfPhotosWarningFailed = false;
            string warningFormat = string.Empty;

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
            int pages = Convert.ToInt32(this._view.Pages);
            var photos = new List<Photo>();
            for (int page = 1; page <= pages; page++) {
                photos.AddRange((await GetPhotosResponse(page)).Photos);
            }
            return photos;
        }

        private async Task DownloadPhotos(IEnumerable<Photo> photos, bool handleSpinner = true) {
            try {
                if (handleSpinner) {
                    this._view.ShowSpinner(true);
                }

                IList<Photo> photosList = photos as IList<Photo> ?? photos.ToList();

                this._cancellationTokenSource = new CancellationTokenSource();
                await
                    this._logic.Download(photosList, this._cancellationTokenSource.Token, this._progress,
                        this._view.Preferences);
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

            SetPhotoResponse(await GetPhotosResponse(page));

            this._view.ShowSpinner(false);
        }

        private async Task<PhotosResponse> GetPhotosResponse(int page) {
            string methodName = this._view.ShowAllPhotos ? Methods.PeopleGetPhotos : Methods.PeopleGetPublicPhotos;
            return
                await
                    this._logic.GetPhotosAsync(methodName, this._view.User, this._view.Preferences, page, this._progress);
        }

        private void SetPhotoResponse(PhotosResponse photosResponse) {
            this._view.Page = photosResponse.Page.ToString(CultureInfo.InvariantCulture);
            this._view.Pages = photosResponse.Pages.ToString(CultureInfo.InvariantCulture);
            this._view.PerPage = photosResponse.PerPage.ToString(CultureInfo.InvariantCulture);
            this._view.Total = photosResponse.Total.ToString(CultureInfo.InvariantCulture);
            this._view.Photos = photosResponse.Photos;
        }
    }
}
