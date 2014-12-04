using FloydPink.Flickr.Downloadr.Presentation;
using StructureMap.Configuration.DSL;

namespace FloydPink.Flickr.Downloadr.Bootstrap {
    public class PresentationRegistry : Registry {
        public PresentationRegistry() {
            For<ILoginPresenter>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<ILoginPresenter>())
                .Use<LoginPresenter>();
            For<IPreferencesPresenter>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IPreferencesPresenter>())
                .Use<PreferencesPresenter>();
            For<IBrowserPresenter>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IBrowserPresenter>())
                .Use<BrowserPresenter>();
        }
    }
}
