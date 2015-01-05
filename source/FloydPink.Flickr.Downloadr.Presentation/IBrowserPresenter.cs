namespace FloydPink.Flickr.Downloadr.Presentation {
    using System.Threading.Tasks;
    using Model.Enums;

    public interface IBrowserPresenter {
        Task InitializePhotoset();
        Task NavigateTo(PhotoPage page);
        Task NavigateTo(int page);
        void CancelDownload();
        Task DownloadSelection();
        Task DownloadThisPage();
        Task DownloadAllPages();
    }
}
