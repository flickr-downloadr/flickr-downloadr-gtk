namespace FloydPink.Flickr.Downloadr.Bootstrap {
    using DotNetOpenAuth.Messaging;
    using DotNetOpenAuth.OAuth;
    using DotNetOpenAuth.OAuth.ChannelElements;
    using OAuth;
    using OAuth.Listener;
    using Repository.Helpers;
    using StructureMap.Configuration.DSL;

    public class OAuthRegistry : Registry {
        private static readonly ServiceProviderDescription FlickrServiceDescription =
            new ServiceProviderDescription {
                ProtocolVersion = ProtocolVersion.V10a,
                RequestTokenEndpoint =
                    new MessageReceivingEndpoint("https://www.flickr.com/services/oauth/request_token",
                        HttpDeliveryMethods.PostRequest),
                UserAuthorizationEndpoint =
                    new MessageReceivingEndpoint("https://www.flickr.com/services/oauth/authorize",
                        HttpDeliveryMethods.GetRequest),
                AccessTokenEndpoint =
                    new MessageReceivingEndpoint("https://www.flickr.com/services/oauth/access_token",
                        HttpDeliveryMethods.GetRequest),
                TamperProtectionElements =
                    new ITamperProtectionChannelBindingElement [] {
                        new HmacSha1SigningBindingElement()
                    }
            };

        private static readonly MessageReceivingEndpoint FlickrServiceEndPoint =
            new MessageReceivingEndpoint("https://api.flickr.com/services/rest", HttpDeliveryMethods.PostRequest);

        public OAuthRegistry() {
            For<IOAuthManager>()
                .Singleton()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IOAuthManager>())
                .Use<OAuthManager>().
                 Ctor<MessageReceivingEndpoint>("serviceEndPoint").Is(FlickrServiceEndPoint);
            For<DesktopConsumer>()
                .Use<DesktopConsumer>()
                .Ctor<ServiceProviderDescription>("serviceDescription")
                .Is(FlickrServiceDescription);
            For<IConsumerTokenManager>()
                .Use<TokenManager>()
                .Ctor<string>("consumerKey")
                .Is(Crypt.Decrypt(Secrets.ConsumerKey, Secrets.SharedSecret))
                .Ctor<string>("consumerSecret")
                .Is(Crypt.Decrypt(Secrets.ConsumerSecret, Secrets.SharedSecret));
            For<IHttpListenerManager>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IHttpListenerManager>())
                .Use<HttpListenerManager>();
        }
    }
}
