using System;
using Castle.DynamicProxy;
using log4net;
using StructureMap.Interceptors;

namespace FloydPink.Flickr.Downloadr.Bootstrap
{
    public class DynamicProxy
    {
        public static EnrichmentHandler<T> LoggingInterceptorFor<T>()
        {
            return s => CreateInterfaceProxyWithTargetInterface(typeof (T), s);
        }

        private static object CreateInterfaceProxyWithTargetInterface(Type interfaceType, object concreteObject)
        {
            var proxyGenerator = new ProxyGenerator();
            object result = proxyGenerator.
                CreateInterfaceProxyWithTargetInterface(interfaceType,
                    concreteObject,
                    new[]
                    {
                        new LogInterceptor(
                            LogManager.GetLogger(concreteObject.GetType()))
                    });

            return result;
        }
    }
}