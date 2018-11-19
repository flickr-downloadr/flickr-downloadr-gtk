using FloydPink.Flickr.Downloadr.Logic;
using FloydPink.Flickr.Downloadr.Logic.Diagnostics;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using StructureMap;

namespace FloydPink.Flickr.Downloadr.Bootstrap
{
  public class LogicRegistry : Registry
  {
    public LogicRegistry()
    {
      For<ILoginLogic>()
        .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<ILoginLogic>())
        .Use<LoginLogic>();
      For<IUserInfoLogic>()
        .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IUserInfoLogic>())
        .Use<UserInfoLogic>();
      For<IBrowserLogic>()
        .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IBrowserLogic>())
        .Use<BrowserLogic>();
      For<ILandingLogic>()
        .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<ILandingLogic>())
        .Use<LandingLogic>();
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
      For<IDonateIntentCheckLogic>()
        .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IDonateIntentCheckLogic>())
        .Use<DonateIntentCheckLogic>();
      For<ISystemProcess>()
        .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<ISystemProcess>())
        .Use<SystemProcess>();
    }
  }
}
