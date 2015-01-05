namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    using System;
    using System.Threading.Tasks;
    using Model;

    public interface ILoginLogic {
        void Login(Action<User> applyUser);
        void Logout();
        Task<bool> IsUserLoggedInAsync(Action<User> applyUser);
    }
}
