namespace FloydPink.Flickr.Downloadr.Repository {
    using Extensions;
    using Model;

    public class UserRepository : RepositoryBase, IRepository<User> {
        protected override string RepoFileName { get { return "user.repo"; } }

        #region IRepository<User> Members

        public User Get() {
            return Read().FromJson<User>();
        }

        public void Save(User value) {
            Write(value.ToJson());
        }

        #endregion
    }
}
