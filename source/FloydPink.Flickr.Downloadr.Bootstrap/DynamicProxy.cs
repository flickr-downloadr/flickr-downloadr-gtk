using System;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using log4net;

namespace FloydPink.Flickr.Downloadr.Bootstrap {
    public class DynamicProxy {
        public static Expression<Func<T, T>> LoggingInterceptorFor<T>() {
            return s => CreateInterfaceProxyWithTargetInterface(typeof (T), s);
        }

        private static T CreateInterfaceProxyWithTargetInterface<T>(Type interfaceType, T concreteObject) {
            var proxyGenerator = new ProxyGenerator();
            object result = proxyGenerator.
                CreateInterfaceProxyWithTargetInterface(interfaceType,
                    concreteObject,
                    new [] {
                        new LogInterceptor(
                            LogManager.GetLogger(concreteObject.GetType()))
                    });
            return (T) result;
        }
    }
}
