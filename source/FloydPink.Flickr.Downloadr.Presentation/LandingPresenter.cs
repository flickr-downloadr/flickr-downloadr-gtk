namespace FloydPink.Flickr.Downloadr.Presentation {
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Logic.Interfaces;
    using Model;
    using Model.Constants;
    using Model.Enums;
    using Views;

    public class LandingPresenter : PresenterBase, ILandingPresenter {
        private readonly ILandingLogic _logic;
        private readonly ILandingView _view;

        public LandingPresenter(ILandingLogic logic, ILandingView view) {
            this._logic = logic;
            this._view = view;
        }

        public async Task Initialize() {
            PhotosetsResponse response = await _logic.GetPhotosetsAsync(Methods.PhotosetsGetList, _view.User, _view.Preferences, 1, new Progress<ProgressUpdate>());
        }

    }
}
