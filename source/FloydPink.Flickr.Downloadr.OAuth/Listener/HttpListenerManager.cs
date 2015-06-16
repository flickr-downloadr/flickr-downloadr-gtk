namespace FloydPink.Flickr.Downloadr.OAuth.Listener {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    public class HttpListenerManager : IHttpListenerManager {
        private static readonly Random Random = new Random();
        private static readonly List<string> ActiveListeners = new List<string>();

        private void KillAnyExistingListeners() {
            if (ActiveListeners.Count != 0) {
                var staleListener = new HttpListener();
                staleListener.Prefixes.Add(ActiveListeners[0]);
                staleListener.Stop();
                staleListener.Close();
                ActiveListeners.Clear();
            }

            ActiveListeners.Add(ListenerAddress);
        }

        private string GetNewHttpListenerAddress() {
            string listenerAddress;
            while (true) {
                var listener = new HttpListener();
                var randomPortNumber = Random.Next(1025, 65535);
                listenerAddress = string.Format("http://localhost:{0}/", randomPortNumber);
                listener.Prefixes.Add(listenerAddress);
                try {
                    listener.Start();
                    listener.Stop();
                    listener.Close();
                }
                catch {
                    continue;
                }
                break;
            }

            return listenerAddress;
        }

        private void HttpListenerCallback(IAsyncResult result) {
            var listener = (HttpListener) result.AsyncState;

            var context = listener.EndGetContext(result);
            var request = context.Request;
            var queryStrings = request.QueryString;

            var response = context.Response;
            var buffer = Encoding.UTF8.GetBytes(ResponseString);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            response.Close();
            listener.Close();

            RequestReceived(this, new HttpListenerCallbackEventArgs(queryStrings));
        }

        #region IHttpListenerManager Members

        public string ListenerAddress { get; private set; }

        public string ResponseString { get; set; }

        public event EventHandler<HttpListenerCallbackEventArgs> RequestReceived;

        public bool RequestReceivedHandlerExists
        {
            get
            {
                var count = 0;
                var eventHandler = RequestReceived;
                if (eventHandler != null) {
                    count = eventHandler.GetInvocationList().Length;
                }
                return count != 0;
            }
        }

        public IAsyncResult SetupCallback() {
            ListenerAddress = GetNewHttpListenerAddress();

            KillAnyExistingListeners();

            var listener = new HttpListener();
            listener.Prefixes.Add(ListenerAddress);
            listener.Start();

            return listener.BeginGetContext(HttpListenerCallback, listener);
        }

        #endregion
    }
}
