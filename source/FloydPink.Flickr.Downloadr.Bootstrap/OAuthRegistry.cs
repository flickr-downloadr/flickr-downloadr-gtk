using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using FloydPink.Flickr.Downloadr.OAuth;
using FloydPink.Flickr.Downloadr.OAuth.Listener;
using FloydPink.Flickr.Downloadr.Repository.Helpers;
using StructureMap.Configuration.DSL;

namespace FloydPink.Flickr.Downloadr.Bootstrap {
    public class OAuthRegistry : Registry {
        private const string SharedSecret = "kn98nkgg90sknka2038234(&9883!@%^";

        private const string ConsumerKey =
            "EAAAABumLz7N4IfZ9hH2YCoRjttqgG3QQEpPUhHC4EUnXl/JOE9Zl90MwGZh2KtuUzIpJz/9s0BX9q3DVBrPUP00g9E=";

        private const string ConsumerSecret = "EAAAAEsjQ3vGqYjtqsHqE+unh1gtlK6usoX2+65UUOW83RHCAC+/n0EPnCaPbaXUAvPs9w==";

        private static readonly ServiceProviderDescription FlickrServiceDescription =
            new ServiceProviderDescription {
                ProtocolVersion = ProtocolVersion.V10a,
                RequestTokenEndpoint =
                    new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/request_token",
                        HttpDeliveryMethods.PostRequest),
                UserAuthorizationEndpoint =
                    new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/authorize",
                        HttpDeliveryMethods.GetRequest),
                AccessTokenEndpoint =
                    new MessageReceivingEndpoint("http://www.flickr.com/services/oauth/access_token",
                        HttpDeliveryMethods.GetRequest),
                TamperProtectionElements =
                    new ITamperProtectionChannelBindingElement [] {
                        new HmacSha1SigningBindingElement()
                    }
            };

        private static readonly MessageReceivingEndpoint FlickrServiceEndPoint =
            new MessageReceivingEndpoint("http://api.flickr.com/services/rest", HttpDeliveryMethods.PostRequest);

        public OAuthRegistry() {
            For<IOAuthManager>()
                .Singleton()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IOAuthManager>())
                .Use<OAuthManager>().
                 Ctor<MessageReceivingEndpoint>("serviceEndPoint").Is(FlickrServiceEndPoint);

            For<DesktopConsumer>()
                .Use<DesktopConsumer>()
                .Ctor<ServiceProviderDescription>("serviceDescription")
                .Is(FlickrServiceDescription);

            For<IConsumerTokenManager>()
                .Use<TokenManager>()
                .Ctor<string>("consumerKey")
                .Is(Crypt.Decrypt(ConsumerKey, SharedSecret))
                .Ctor<string>("consumerSecret")
                .Is(Crypt.Decrypt(ConsumerSecret, SharedSecret));

            For<IHttpListenerManager>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IHttpListenerManager>())
                .Use<HttpListenerManager>();
        }
    }
}
