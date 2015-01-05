namespace FloydPink.Flickr.Downloadr.OAuth {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    public interface IOAuthManager {
        string AccessToken { get; set; }
        event EventHandler<AuthenticatedEventArgs> Authenticated;
        string BeginAuthorization();
        HttpWebRequest PrepareAuthorizedRequest(IDictionary<string, string> parameters);
        Task<dynamic> MakeAuthenticatedRequestAsync(string methodName, IDictionary<string, string> parameters = null);
    }
}
