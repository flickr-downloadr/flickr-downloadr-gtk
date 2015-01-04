using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Constants;
using FloydPink.Flickr.Downloadr.OAuth.Listener;

namespace FloydPink.Flickr.Downloadr.OAuth {
    public class OAuthManager : IOAuthManager {
        private string _requestToken = string.Empty;
        private readonly DesktopConsumer _consumer;

        private readonly Dictionary<string, string> _defaultParameters = new Dictionary<string, string> {
            {
                ParameterNames.NoJsonCallback, "1"
            }, {
                ParameterNames.Format, "json"
            }, {
                ParameterNames.Extras,
                AppConstants.ExtraInfo
            }
        };

        private readonly IHttpListenerManager _listenerManager;
        private readonly MessageReceivingEndpoint _serviceEndPoint;

        public OAuthManager(IHttpListenerManager listenerManager, DesktopConsumer consumer,
                            MessageReceivingEndpoint serviceEndPoint) {
            _listenerManager = listenerManager;
            _consumer = consumer;
            _serviceEndPoint = serviceEndPoint;
            // Trying to fix https://github.com/flickr-downloadr/flickr-downloadr-gtk/issues/15
            // From the comment in this SO answer:
            // http://stackoverflow.com/questions/1186682/expectation-failed-when-trying-to-update-twitter-status/2025073#2025073
            ServicePointManager.FindServicePoint(_serviceEndPoint.Location).Expect100Continue = false;
        }

        private string CompleteAuthorization(string verifier) {
            var response = _consumer.ProcessUserAuthorization(_requestToken, verifier);
            AccessToken = response.AccessToken;

            var extraData = response.ExtraData;
            var authenticatedUser = new User(extraData["fullname"], extraData["username"], extraData["user_nsid"]);
            Authenticated(this, new AuthenticatedEventArgs(authenticatedUser));

            return response.AccessToken;
        }

        private void callbackManager_OnRequestReceived(object sender, HttpListenerCallbackEventArgs e) {
            var token = e.QueryStrings["oauth_token"];
            var verifier = e.QueryStrings["oauth_verifier"];
            if (token == _requestToken) {
                CompleteAuthorization(verifier);
            }
        }

        #region IOAuthManager Members

        public string AccessToken { get; set; }

        public event EventHandler<AuthenticatedEventArgs> Authenticated;

        public string BeginAuthorization() {
            if (!_listenerManager.RequestReceivedHandlerExists) {
                _listenerManager.RequestReceived += callbackManager_OnRequestReceived;
            }
            _listenerManager.ResponseString = AppConstants.AuthenticatedMessage;
            _listenerManager.SetupCallback();
            var requestArgs = new Dictionary<string, string> {
                {
                    ParameterNames.OAuthCallback, _listenerManager.ListenerAddress
                }
            };
            var redirectArgs = new Dictionary<string, string> {
                {
                    ParameterNames.Permissions, "read"
                }
            };

            return
                _consumer.RequestUserAuthorization(requestArgs, redirectArgs, out _requestToken).AbsoluteUri;
        }

        public HttpWebRequest PrepareAuthorizedRequest(IDictionary<string, string> parameters) {
            return _consumer.PrepareAuthorizedRequest(_serviceEndPoint, AccessToken, parameters);
        }

        public async Task<dynamic> MakeAuthenticatedRequestAsync(string methodName,
                                                                 IDictionary<string, string> parameters = null) {
            var request = PrepareAuthorizedRequest(AddRequestParameters(methodName, parameters));
            var response = (HttpWebResponse) await request.GetResponseAsync();
            using (var reader = new StreamReader(response.GetResponseStream())) {
                return (new JavaScriptSerializer()).Deserialize<dynamic>(reader.ReadToEnd());
            }
        }

        private Dictionary<string, string> AddRequestParameters(string methodName,
                                                                IDictionary<string, string> parameters = null) {
            parameters = parameters ?? new Dictionary<string, string>();
            var allParameters = new Dictionary<string, string>(parameters);
            foreach (var kvp in _defaultParameters) {
                allParameters.Add(kvp.Key, kvp.Value);
            }
            allParameters.Add(ParameterNames.Method, methodName);

            return allParameters;
        }

        #endregion
    }
}
