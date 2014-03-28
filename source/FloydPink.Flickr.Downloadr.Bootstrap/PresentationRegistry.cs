using FloydPink.Flickr.Downloadr.Presentation;
using StructureMap.Configuration.DSL;

namespace FloydPink.Flickr.Downloadr.Bootstrap
{
    public class PresentationRegistry : Registry
    {
        public PresentationRegistry()
        {
            For<ILoginPresenter>()
			//.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<ILoginPresenter>())
                .Use<LoginPresenter>();
            For<IPreferencesPresenter>()
			//.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IPreferencesPresenter>())
                .Use<PreferencesPresenter>();
            For<IBrowserPresenter>()
			//.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IBrowserPresenter>())
                .Use<BrowserPresenter>();
        }
    }
}