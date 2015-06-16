namespace FloydPink.Flickr.Downloadr.OAuth {
    using System;
    using System.Collections.Generic;
    using DotNetOpenAuth.OAuth.ChannelElements;
    using DotNetOpenAuth.OAuth.Messages;
    using Model;
    using Repository;

    public class TokenManager : IConsumerTokenManager {
        private readonly IRepository<Token> _tokenRepository;

        private readonly Dictionary<string, Tuple<string, TokenType>> _tokens =
            new Dictionary<string, Tuple<string, TokenType>>();

        public TokenManager(string consumerKey, string consumerSecret, IRepository<Token> tokenRepository) {
            if (string.IsNullOrEmpty(consumerKey)) {
                throw new ArgumentNullException("consumerKey");
            }

            this._tokenRepository = tokenRepository;

            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;

            GetStoredAccessToken();
        }

        private void GetStoredAccessToken() {
            var token = this._tokenRepository.Get();
            if (!string.IsNullOrEmpty(token.TokenString)) {
                this._tokens[token.TokenString] = new Tuple<string, TokenType>(token.Secret, TokenType.AccessToken);
            }
        }

        #region IConsumerTokenManager Members

        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken,
                                                             string accessTokenSecret) {
            this._tokens.Remove(requestToken);
            this._tokens[accessToken] = new Tuple<string, TokenType>(accessTokenSecret, TokenType.AccessToken);
            this._tokenRepository.Save(new Token(accessToken, accessTokenSecret));
        }

        public string GetTokenSecret(string token) {
            return this._tokens[token].Item1;
        }

        public TokenType GetTokenType(string token) {
            return this._tokens[token].Item2;
        }

        public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response) {
            this._tokens[response.Token] = new Tuple<string, TokenType>(response.TokenSecret, TokenType.RequestToken);
        }

        #endregion
    }
}
