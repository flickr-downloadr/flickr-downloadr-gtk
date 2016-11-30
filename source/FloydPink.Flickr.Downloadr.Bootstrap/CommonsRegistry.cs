namespace FloydPink.Flickr.Downloadr.Bootstrap {
    using log4net;
    using log4net.Config;
    using StructureMap;

    public class CommonsRegistry : Registry {
        public CommonsRegistry() {

            For<ILog>().Use(c => LogManager.GetLogger(c.ParentType)).AlwaysUnique();

            XmlConfigurator.ConfigureAndWatch(Bootstrapper.GetLogConfigFile());
        }
    }
}
