namespace FloydPink.Flickr.Downloadr.OAuth.Listener {
    using System;
    using System.Collections.Specialized;

    public class HttpListenerCallbackEventArgs : EventArgs {
        public HttpListenerCallbackEventArgs(NameValueCollection queryStrings) {
            QueryStrings = queryStrings;
        }

        public NameValueCollection QueryStrings { get; private set; }
    }
}
