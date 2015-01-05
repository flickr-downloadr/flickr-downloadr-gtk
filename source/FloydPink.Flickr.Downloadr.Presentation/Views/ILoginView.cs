namespace FloydPink.Flickr.Downloadr.Presentation.Views {
    using Model;

    public interface ILoginView : IBaseView {
        User User { get; set; }
        void ShowLoggedInControl(Preferences preferences);
        void ShowLoggedOutControl();
        void OpenBrowserWindow();
        void OpenPreferencesWindow(Preferences preferences);
        void ShowUpdateAvailableNotification(string latestVersion);
    }
}
