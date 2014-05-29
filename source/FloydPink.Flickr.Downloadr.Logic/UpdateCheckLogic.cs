using System;
using System.IO;
using System.Net;
using System.Reflection;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;

namespace FloydPink.Flickr.Downloadr.Logic {
    public class UpdateCheckLogic : IUpdateCheckLogic {
        private readonly IRepository<Update> _repository;

        public UpdateCheckLogic(IRepository<Update> repository) {
            this._repository = repository;
        }

        #region IUpdateCheckLogic implementation

        public Update UpdateAvailable(Preferences preferences) {
            Update update = this._repository.Get();
            if (DateTime.Now.Subtract(TimeSpan.FromDays(1.0)) > update.LastChecked) {
                Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                Version latestVersion;
                WebRequest request = WebRequest.Create("http://flickrdownloadr.com/build.number");
                var response = (HttpWebResponse) request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream())) {
                    latestVersion = new Version(reader.ReadToEnd());
                }
                update.Available = latestVersion.CompareTo(currentVersion) > 0;
                update.LastChecked = DateTime.Now;
                update.LatestVersion = latestVersion.ToString();
                this._repository.Save(update);
            }
            return update;
        }

        #endregion
    }
}
