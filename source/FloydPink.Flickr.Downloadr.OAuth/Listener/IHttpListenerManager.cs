using System;

namespace FloydPink.Flickr.Downloadr.OAuth.Listener
{
    public interface IHttpListenerManager
    {
        string ListenerAddress { get; }
        string ResponseString { get; set; }
        bool RequestReceivedHandlerExists { get; }
        IAsyncResult SetupCallback();
        event EventHandler<HttpListenerCallbackEventArgs> RequestReceived;
    }
}