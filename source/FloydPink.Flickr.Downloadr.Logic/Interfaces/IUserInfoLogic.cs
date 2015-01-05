namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    using System.Threading.Tasks;
    using Model;

    public interface IUserInfoLogic {
        Task<User> PopulateUserInfo(User user);
    }
}
