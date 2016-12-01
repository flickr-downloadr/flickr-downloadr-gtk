using System.IO;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;

namespace FloydPink.Flickr.Downloadr.Logic
{
  public class PreferencesLogic : IPreferencesLogic
  {
    private readonly IRepository<Preferences> _repository;

    public PreferencesLogic(IRepository<Preferences> repository)
    {
      _repository = repository;
    }

    public Preferences GetPreferences()
    {
      var preferences = _repository.Get();
      preferences = Validate(preferences);
      return preferences.PhotosPerPage == 0 ? null : preferences;
    }

    public void SavePreferences(Preferences preferences)
    {
      _repository.Save(preferences);
    }

    public void EmptyCacheDirectory(string cacheLocation)
    {
      var directory = new DirectoryInfo(cacheLocation);
      foreach (var file in directory.GetFiles())
      {
        file.Delete();
      }
      foreach (var subDirectory in directory.GetDirectories())
      {
        subDirectory.Delete(true);
      }
    }

    private Preferences Validate(Preferences preferences)
    {
      var defaults = Preferences.GetDefault();
      if (preferences.LogLocation == null)
      {
        preferences.LogLevel = defaults.LogLevel;
        preferences.LogLocation = defaults.LogLocation;
      }
      return preferences;
    }
  }
}
