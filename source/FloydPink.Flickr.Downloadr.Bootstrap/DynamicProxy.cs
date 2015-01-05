namespace FloydPink.Flickr.Downloadr.Bootstrap {
    using System;
    using System.Linq.Expressions;
    using Castle.DynamicProxy;
    using log4net;

    public class DynamicProxy {
        public static Expression<Func<T, T>> LoggingInterceptorFor<T>() {
            // TODO: Uncomment the below line when the issue with Dynamic Proxy crashing Mono is fixed: https://github.com/castleproject/Core/issues/72
            // return s => CreateInterfaceProxyWithTargetInterface(typeof (T), s);
            return s => s;
        }

        private static T CreateInterfaceProxyWithTargetInterface<T>(Type interfaceType, T concreteObject) {
            var proxyGenerator = new ProxyGenerator();
            var result = proxyGenerator.
                CreateInterfaceProxyWithTargetInterface(interfaceType,
                    concreteObject, new LogInterceptor(
                        LogManager.GetLogger(concreteObject.GetType())));
            return (T) result;
        }
    }
}
