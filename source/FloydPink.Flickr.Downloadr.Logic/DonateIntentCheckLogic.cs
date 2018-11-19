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

    public DonateIntent DonateIntentAvailable()
    {
      try
      {
        var update = _repository.Get();
        var preferences = _preferencesRepository.Get();

        var suppressed = DateTime.Now.Subtract(TimeSpan.FromDays(1.0)) <= update.LastChecked;
        update.Suppressed = suppressed || preferences.SuppressDonationPrompt;
        update.LastChecked = DateTime.Now;

        _repository.Save(update);

        return update;
      } catch (Exception ex)
      {
        Log.Error(string.Format("Unexpected exception in DonateIntentCheckLogic::DonateIntentAvailble method\n\n{0}", ex));
        return null;
      }
    }

    #endregion
  }
}
