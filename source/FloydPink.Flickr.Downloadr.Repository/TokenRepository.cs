namespace FloydPink.Flickr.Downloadr.Repository {
    using Extensions;
    using Model;

    public class TokenRepository : RepositoryBase, IRepository<Token> {
        protected override string RepoFileName { get { return "token.repo"; } }

        #region IRepository<Token> Members

        public Token Get() {
            return Read().FromJson<Token>();
        }

        public void Save(Token value) {
            Write(value.ToJson());
        }

        #endregion
    }
}
