using System;
using System.IO;
using System.Net;
using System.Reflection;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;
using log4net;

namespace FloydPink.Flickr.Downloadr.Logic
{
  public class UpdateCheckLogic : IUpdateCheckLogic
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(UpdateCheckLogic));
    private readonly IRepository<Update> _repository;


    public UpdateCheckLogic(IRepository<Update> repository)
    {
      _repository = repository;
    }

    #region IUpdateCheckLogic implementation

    public Update UpdateAvailable(Preferences preferences)
    {
      try
      {
        var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        Version latestVersion;
        var update = _repository.Get();
        if (DateTime.Now.Subtract(TimeSpan.FromDays(1.0)) > update.LastChecked)
        {
          var request = WebRequest.Create("http://flickrdownloadr.com/build.number");
          var response = (HttpWebResponse) request.GetResponse();
          using (var reader = new StreamReader(response.GetResponseStream()))
          {
            latestVersion = new Version(reader.ReadToEnd());
          }
          update.LastChecked = DateTime.Now;
          update.LatestVersion = latestVersion.ToString();
        }
        else
        {
          latestVersion = new Version(update.LatestVersion);
        }
        update.Available = latestVersion.CompareTo(currentVersion) > 0;
        _repository.Save(update);
        return update;
      } catch (Exception ex)
      {
        Log.Error(string.Format("Unexpected exception in UpdateCheckLogic::UpdateAvailble method\n\n{0}", ex));
        return null;
      }
    }

    #endregion
  }
}
