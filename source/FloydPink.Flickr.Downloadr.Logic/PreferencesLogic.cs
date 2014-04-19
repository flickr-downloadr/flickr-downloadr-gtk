using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;

namespace FloydPink.Flickr.Downloadr.Logic {
    public class PreferencesLogic : IPreferencesLogic {
        private readonly IRepository<Preferences> _repository;

        public PreferencesLogic(IRepository<Preferences> repository) {
            this._repository = repository;
        }

        public Preferences GetPreferences() {
            Preferences preferences = this._repository.Get();
            return preferences.PhotosPerPage == 0 ? null : preferences;
        }

        public void SavePreferences(Preferences preferences) {
            this._repository.Save(preferences);
        }
    }
}
