namespace FloydPink.Flickr.Downloadr.Model {
    public class Token {
        public Token() {
            TokenString = string.Empty;
            Secret = string.Empty;
        }

        public Token(string token, string secret) {
            TokenString = token;
            Secret = secret;
        }

        public string TokenString { get; set; }
        public string Secret { get; set; }
    }
}
