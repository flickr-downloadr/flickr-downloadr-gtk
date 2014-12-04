using log4net;
using log4net.Config;
using StructureMap.Configuration.DSL;
namespace FloydPink.Flickr.Downloadr.Bootstrap {
	public class CommonsRegistry : Registry {
		public CommonsRegistry() {
			For<ILog>().AlwaysUnique().UseSpecial(s => s.ConstructedBy(c => LogManager.GetLogger(c.ParentType)));
			XmlConfigurator.ConfigureAndWatch(Bootstrapper.GetLogConfigFile());
		}
	}
}