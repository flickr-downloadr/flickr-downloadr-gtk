namespace FloydPink.Flickr.Downloadr.Bootstrap {
    using Presentation;
    using StructureMap.Configuration.DSL;

    public class PresentationRegistry : Registry {
        public PresentationRegistry() {
            For<ILoginPresenter>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<ILoginPresenter>())
                .Use<LoginPresenter>();
            For<IPreferencesPresenter>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IPreferencesPresenter>())
                .Use<PreferencesPresenter>();
            For<ILandingPresenter>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<ILandingPresenter>())
                .Use<LandingPresenter>();
            For<IBrowserPresenter>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IBrowserPresenter>())
                .Use<BrowserPresenter>();
        }
    }
}
