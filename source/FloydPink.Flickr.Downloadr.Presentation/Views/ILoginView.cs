namespace FloydPink.Flickr.Downloadr.Presentation.Views {
    using Model;

    public interface ILoginView : IBaseView {
        User User { get; set; }
        void ShowLoggedInControl(Preferences preferences);
        void ShowLoggedOutControl();
        void OpenLandingWindow();
        void OpenPreferencesWindow(Preferences preferences);
        void ShowUpdateAvailableNotification(string latestVersion);
    }
}
