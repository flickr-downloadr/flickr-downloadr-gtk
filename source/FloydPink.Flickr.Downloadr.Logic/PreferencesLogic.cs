namespace FloydPink.Flickr.Downloadr.Logic {
    using System.IO;
    using Interfaces;
    using Model;
    using Repository;

    public class PreferencesLogic : IPreferencesLogic {
        private readonly IRepository<Preferences> _repository;

        public PreferencesLogic(IRepository<Preferences> repository) {
            this._repository = repository;
        }

        public Preferences GetPreferences() {
            var preferences = this._repository.Get();
            preferences = Validate(preferences);
            return preferences.PhotosPerPage == 0 ? null : preferences;
        }

        public void SavePreferences(Preferences preferences) {
            this._repository.Save(preferences);
        }

        public void EmptyCacheDirectory(string cacheLocation) {
            var directory = new DirectoryInfo(cacheLocation);
            foreach (var file in directory.GetFiles()) {
                file.Delete();
            }
            foreach (var subDirectory in directory.GetDirectories()) {
                subDirectory.Delete(true);
            }
        }

        private Preferences Validate(Preferences preferences) {
            var defaults = Preferences.GetDefault();
            if (preferences.LogLocation == null) {
                preferences.LogLevel = defaults.LogLevel;
                preferences.LogLocation = defaults.LogLocation;
            }
            return preferences;
        }
    }
}
