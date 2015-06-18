namespace FloydPink.Flickr.Downloadr.Presentation {
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Logic.Interfaces;
    using Model;
    using Model.Constants;
    using Model.Enums;
    using Views;

    public class LandingPresenter : PresenterBase, ILandingPresenter {
        private readonly ILandingLogic _logic;
        private readonly Progress<ProgressUpdate> _progress = new Progress<ProgressUpdate>();
        private readonly ILandingView _view;

        public LandingPresenter(ILandingLogic logic, ILandingView view) {
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
                                              };
        }

        public async Task Initialize() {
            try {
                this._view.ShowSpinner(true);

                this._view.PublicPhotoset = await this._logic.GetCoverPhotoAsync(this._view.User, this._view.Preferences, false);
                this._view.PrivatePhotoset = await this._logic.GetCoverPhotoAsync(this._view.User, this._view.Preferences, true);

                await GetAndSetPhotosets(1);
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
                await GetAndSetPhotosets(targetPage);
            }
        }

        public async Task NavigateTo(int page) {
            await GetAndSetPhotosets(page);
        }

        private async Task GetAndSetPhotosets(int page) {
            this._view.ShowSpinner(true);

            SetPhotosetsResponse(await GetPhotosetsResponse(page));

            this._view.ShowSpinner(false);
        }

        private async Task<PhotosetsResponse> GetPhotosetsResponse(int page) {
            return
                await this._logic.GetPhotosetsAsync(Methods.PhotosetsGetList, this._view.User, this._view.Preferences, page, this._progress);
        }

        private void SetPhotosetsResponse(PhotosetsResponse photosetsResponse) {
            this._view.Page = photosetsResponse.Page.ToString(CultureInfo.InvariantCulture);
            this._view.Pages = photosetsResponse.Pages.ToString(CultureInfo.InvariantCulture);
            this._view.PerPage = photosetsResponse.PerPage.ToString(CultureInfo.InvariantCulture);
            this._view.Total = photosetsResponse.Total.ToString(CultureInfo.InvariantCulture);
            this._view.Albums = photosetsResponse.Photosets;
        }
    }
}
