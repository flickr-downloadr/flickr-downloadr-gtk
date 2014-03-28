using log4net;
using log4net.Config;
using StructureMap.Configuration.DSL;

namespace FloydPink.Flickr.Downloadr.Bootstrap
{
    public class CommonsRegistry : Registry
    {
        public CommonsRegistry()
        {
            For<ILog>().AlwaysUnique().TheDefault.Is.
                ConstructedBy(s =>
                {
                    if (s.ParentType == null)
                        return LogManager.GetLogger(s.BuildStack.Current.ConcreteType);
                    return LogManager.GetLogger(s.ParentType);
                });

            XmlConfigurator.ConfigureAndWatch(Bootstrapper.GetLogConfigFile());
        }
    }
}