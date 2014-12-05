using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository.Extensions;

namespace FloydPink.Flickr.Downloadr.Repository {
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
