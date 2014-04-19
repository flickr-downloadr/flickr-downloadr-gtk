using FloydPink.Flickr.Downloadr.Logic;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using StructureMap.Configuration.DSL;

namespace FloydPink.Flickr.Downloadr.Bootstrap {
    public class LogicRegistry : Registry {
        public LogicRegistry() {
            For<ILoginLogic>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<ILoginLogic>())
                .Use<LoginLogic>();
            For<IBrowserLogic>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IBrowserLogic>())
                .Use<BrowserLogic>();
            For<IDownloadLogic>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IDownloadLogic>())
                .Use<DownloadLogic>();
            For<IPreferencesLogic>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IPreferencesLogic>())
                .Use<PreferencesLogic>();
            For<IOriginalTagsLogic>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IOriginalTagsLogic>())
                .Use<OriginalTagsLogic>();
        }
    }
}
