using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository.Extensions;

namespace FloydPink.Flickr.Downloadr.Repository
{
    public class TokenRepository : RepositoryBase, IRepository<Token>
    {
        internal override string RepoFileName
        {
            get { return "token.repo"; }
        }

        #region IRepository<Token> Members

        public Token Get()
        {
            return Read().FromJson<Token>();
        }

        public void Save(Token value)
        {
            Write(value.ToJson());
        }

        #endregion
    }
}