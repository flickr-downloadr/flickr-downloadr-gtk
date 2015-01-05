namespace FloydPink.Flickr.Downloadr.Presentation {
    using Model;

    public interface IPreferencesPresenter {
        void Save(Preferences preferences);
        string GetCacheFolderSize(string cacheLocation);
        void EmptyCacheDirectory(string cacheLocation);
    }
}
