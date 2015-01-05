namespace FloydPink.Flickr.Downloadr.Repository {
    using Extensions;
    using Model;

    public class PreferencesRepository : RepositoryBase, IRepository<Preferences> {
        protected override string RepoFileName { get { return "prefs.repo"; } }

        public Preferences Get() {
            return Read().FromJson<Preferences>();
        }

        public void Save(Preferences value) {
            Write(value.ToJson());
        }
    }
}
