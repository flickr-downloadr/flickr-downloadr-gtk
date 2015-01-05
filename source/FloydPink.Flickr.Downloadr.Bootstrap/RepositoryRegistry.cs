namespace FloydPink.Flickr.Downloadr.Bootstrap {
    using Model;
    using Repository;
    using StructureMap.Configuration.DSL;

    public class RepositoryRegistry : Registry {
        public RepositoryRegistry() {
            For<IRepository<Token>>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IRepository<Token>>())
                .Use<TokenRepository>();
            For<IRepository<User>>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IRepository<User>>())
                .Use<UserRepository>();
            For<IRepository<Preferences>>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IRepository<Preferences>>())
                .Use<PreferencesRepository>();
            For<IRepository<Update>>()
                .DecorateAllWith(DynamicProxy.LoggingInterceptorFor<IRepository<Update>>())
                .Use<UpdateRepository>();
        }
    }
}
