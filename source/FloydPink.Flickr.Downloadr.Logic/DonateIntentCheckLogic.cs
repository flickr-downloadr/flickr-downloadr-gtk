using System;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository;
using log4net;

namespace FloydPink.Flickr.Downloadr.Logic
{
  public class DonateIntentCheckLogic : IDonateIntentCheckLogic
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(DonateIntentCheckLogic));
    private readonly IRepository<DonateIntent> _repository;
    private readonly IRepository<Preferences> _preferencesRepository;

    public DonateIntentCheckLogic(IRepository<DonateIntent> repository,
                                  IRepository<Preferences> preferencesRepository)
    {
      _repository = repository;
      _preferencesRepository = preferencesRepository;
    }

    #region IDonateIntentCheckLogic implementation

    public DonateIntent DonateIntentAvailable(int photosCount = 0)
    {
      try
      {
        var intent = _repository.Get();
        var preferences = _preferencesRepository.Get();

        var suppressed = DateTime.Now.Subtract(TimeSpan.FromHours(1.0)) <= intent.LastChecked;
        intent.Suppressed = suppressed || preferences.SuppressDonationPrompt;
        intent.LastChecked = DateTime.Now;
        intent.DownloadedPhotosCount = photosCount;

        _repository.Save(intent);

        return intent;
      } catch (Exception ex)
      {
        Log.Error(string.Format("Unexpected exception in DonateIntentCheckLogic::DonateIntentAvailble method\n\n{0}", ex));
        return null;
      }
    }

    #endregion
  }
}
