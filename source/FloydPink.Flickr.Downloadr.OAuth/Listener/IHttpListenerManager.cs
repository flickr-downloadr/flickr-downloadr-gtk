namespace FloydPink.Flickr.Downloadr.OAuth.Listener {
    using System;

    public interface IHttpListenerManager {
        string ListenerAddress { get; }
        string ResponseString { get; set; }
        bool RequestReceivedHandlerExists { get; }
        IAsyncResult SetupCallback();
        event EventHandler<HttpListenerCallbackEventArgs> RequestReceived;
    }
}
