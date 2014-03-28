using System.Collections.Generic;
using System.Collections.ObjectModel;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Presentation.Views
{
    public interface IBrowserView : IBaseView
    {
        User User { get; set; }
        Preferences Preferences { get; set; }
        ObservableCollection<Photo> Photos { get; set; }
        IDictionary<string, Dictionary<string, Photo>> AllSelectedPhotos { get; set; }
        bool ShowAllPhotos { get; }

        string Page { get; set; }
        string Pages { get; set; }
        string PerPage { get; set; }
        string Total { get; set; }
        void UpdateProgress(string percentDone, string operationText, bool cancellable);
        bool ShowWarning(string warningMessage);
        void DownloadComplete(string downloadedLocation, bool downloadComplete);
    }
}