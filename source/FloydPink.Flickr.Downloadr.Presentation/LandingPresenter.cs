namespace FloydPink.Flickr.Downloadr.Presentation {
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Logic.Interfaces;
    using Model;
    using Model.Constants;
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
            this._view.ShowSpinner(true);

            var response =
                await this._logic.GetPhotosetsAsync(Methods.PhotosetsGetList, this._view.User, this._view.Preferences, 1, this._progress);

            SetPhotosetsResponse(response);

            this._view.ShowSpinner(false);
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
