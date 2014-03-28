using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Presentation.Views
{
    public interface ILoginView : IBaseView
    {
        User User { get; set; }

        void ShowLoggedInControl(Preferences preferences);
        void ShowLoggedOutControl();
        void OpenBrowserWindow();
        void OpenPreferencesWindow(Preferences preferences);
    }
}