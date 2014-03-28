using System;
using System.IO;
using StructureMap;

namespace FloydPink.Flickr.Downloadr.Bootstrap
{
    public static class Bootstrapper
    {
        private static Container _container;

        public static void Initialize()
        {
            _container = new Container(expression =>
            {
                expression.AddRegistry<CommonsRegistry>();
                expression.AddRegistry<OAuthRegistry>();
                expression.AddRegistry<RepositoryRegistry>();
                expression.AddRegistry<LogicRegistry>();
                expression.AddRegistry<PresentationRegistry>();
            });
        }

        public static T GetInstance<T>()
        {
            return _container.GetInstance<T>();
        }

        public static TPresenter GetPresenter<TView, TPresenter>(TView view)
        {
            return _container.With(view).GetInstance<TPresenter>();
        }

        public static FileInfo GetLogFile()
        {
            return GetAppDirectoryFile("flickrdownloadr.log");
        }

        public static FileInfo GetLogConfigFile()
        {
            return GetAppDirectoryFile("flickrdownloadr.log4net");
        }

        private static FileInfo GetAppDirectoryFile(string filename)
        {
            return new FileInfo(Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), filename));
        }
    }
}