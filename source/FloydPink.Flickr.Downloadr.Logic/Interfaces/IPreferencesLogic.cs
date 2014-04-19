using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    public interface IPreferencesLogic {
        Preferences GetPreferences();
        void SavePreferences(Preferences preferences);
    }
}
