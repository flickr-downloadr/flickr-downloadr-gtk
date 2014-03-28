using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository.Extensions;

namespace FloydPink.Flickr.Downloadr.Repository
{
    public class UserRepository : RepositoryBase, IRepository<User>
    {
        internal override string RepoFileName
        {
            get { return "user.repo"; }
        }

        #region IRepository<User> Members

        public User Get()
        {
            return Read().FromJson<User>();
        }

        public void Save(User value)
        {
            Write(value.ToJson());
        }

        #endregion
    }
}