using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Logic.Interfaces
{
  public interface IUserInfoLogic
  {
    Task<User> PopulateUserInfo(User user);
  }
}
