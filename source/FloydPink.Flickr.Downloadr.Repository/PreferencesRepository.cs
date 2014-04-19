using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository.Extensions;

namespace FloydPink.Flickr.Downloadr.Repository {
    public class PreferencesRepository : RepositoryBase, IRepository<Preferences> {
        internal override string RepoFileName { get { return "prefs.repo"; } }

        public Preferences Get() {
            return Read().FromJson<Preferences>();
        }

        public void Save(Preferences value) {
            Write(value.ToJson());
        }
    }
}
