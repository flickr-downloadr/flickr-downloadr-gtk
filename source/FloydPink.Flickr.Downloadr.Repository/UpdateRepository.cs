namespace FloydPink.Flickr.Downloadr.Repository {
    using Extensions;
    using Model;

    public class UpdateRepository : RepositoryBase, IRepository<Update> {
        protected override string RepoFileName { get { return "update.repo"; } }

        public Update Get() {
            return Read().FromJson<Update>();
        }

        public void Save(Update value) {
            Write(value.ToJson());
        }
    }
}
