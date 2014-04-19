using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Presentation {
    public interface IPreferencesPresenter {
        void Save(Preferences preferences);
        string GetCacheFolderSize(string cacheLocation);
        void EmptyCacheDirectory(string cacheLocation);
    }
}
