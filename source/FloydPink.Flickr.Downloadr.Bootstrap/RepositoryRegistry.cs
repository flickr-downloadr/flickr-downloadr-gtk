using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;
using StructureMap.Configuration.DSL;

namespace FloydPink.Flickr.Downloadr.Bootstrap {
    public class RepositoryRegistry : Registry {
        public RepositoryRegistry() {
            For<IRepository<Token>>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IRepository<Token>>())
                .Use<TokenRepository>();
            For<IRepository<User>>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IRepository<User>>())
                .Use<UserRepository>();
            For<IRepository<Preferences>>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IRepository<Preferences>>())
                .Use<PreferencesRepository>();
            For<IRepository<Update>>()
                //.EnrichAllWith(DynamicProxy.LoggingInterceptorFor<IRepository<Preferences>>())
                .Use<UpdateRepository>();
        }
    }
}
