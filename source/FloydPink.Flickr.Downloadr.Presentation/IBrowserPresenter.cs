using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Model.Enums;

namespace FloydPink.Flickr.Downloadr.Presentation {
    public interface IBrowserPresenter {
        Task InitializePhotoset();
        Task NavigateTo(PhotoPage page);
        void CancelDownload();
        Task DownloadSelection();
        Task DownloadThisPage();
        Task DownloadAllPages();
    }
}
