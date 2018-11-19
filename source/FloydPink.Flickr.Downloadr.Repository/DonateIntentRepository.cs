using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Repository.Extensions;

namespace FloydPink.Flickr.Downloadr.Repository
{
  public class DonateIntentRepository : RepositoryBase, IRepository<DonateIntent>
  {
    protected override string RepoFileName
    {
      get
      {
        return "donateIntent.repo";
      }
    }

    public DonateIntent Get()
    {
      return Read().FromJson<DonateIntent>();
    }

    public void Save(DonateIntent value)
    {
      Write(value.ToJson());
    }
  }
}
