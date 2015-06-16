namespace FloydPink.Flickr.Downloadr.Presentation.Views {
    using System.Collections.Generic;
    using Model;
    using Model.Enums;

    public interface IBrowserView : IBaseView {
        User User { get; set; }
        Preferences Preferences { get; set; }
        IEnumerable<Photo> Photos { get; set; }
        IDictionary<string, Dictionary<string, Photo>> AllSelectedPhotos { get; set; }
        PhotosetType PhotosetType { get; set; }
        string Page { get; set; }
        string Pages { get; set; }
        string PerPage { get; set; }
        string Total { get; set; }
        void UpdateProgress(string percentDone, string operationText, bool cancellable);
        bool ShowWarning(string warningMessage);
        void DownloadComplete(string downloadedLocation, bool downloadComplete);
    }
}
