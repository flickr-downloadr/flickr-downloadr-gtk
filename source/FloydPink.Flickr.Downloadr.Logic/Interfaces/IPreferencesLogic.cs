namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    using Model;

    public interface IPreferencesLogic {
        Preferences GetPreferences();
        void SavePreferences(Preferences preferences);
        void EmptyCacheDirectory(string cacheLocation);
    }
}
