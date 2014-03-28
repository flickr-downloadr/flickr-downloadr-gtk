using System;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Logic.Interfaces
{
    public interface ILoginLogic
    {
        void Login(Action<User> applyUser);
        void Logout();
        Task<bool> IsUserLoggedInAsync(Action<User> applyUser);
    }
}