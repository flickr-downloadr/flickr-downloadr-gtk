namespace FloydPink.Flickr.Downloadr.Presentation {
    using System.Threading.Tasks;
    using Model.Enums;

    public interface ILandingPresenter {
        Task Initialize();
        Task NavigateTo(PhotoOrAlbumPage page);
        Task NavigateTo(int page);
    }
}
