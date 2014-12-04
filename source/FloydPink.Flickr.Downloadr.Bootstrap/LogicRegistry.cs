using FloydPink.Flickr.Downloadr.Logic;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using StructureMap.Configuration.DSL;

namespace FloydPink.Flickr.Downloadr.Bootstrap {
    public class LogicRegistry : Registry {
        public LogicRegistry() {
            For<ILoginLogic>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<ILoginLogic>())
                .Use<LoginLogic>();
            For<IBrowserLogic>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IBrowserLogic>())
                .Use<BrowserLogic>();
            For<IDownloadLogic>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IDownloadLogic>())
                .Use<DownloadLogic>();
            For<IPreferencesLogic>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IPreferencesLogic>())
                .Use<PreferencesLogic>();
            For<IOriginalTagsLogic>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IOriginalTagsLogic>())
                .Use<OriginalTagsLogic>();
            For<IUpdateCheckLogic>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IUpdateCheckLogic>())
                .Use<UpdateCheckLogic>();
        }
    }
}
