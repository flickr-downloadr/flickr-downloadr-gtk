namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    using Model;

    public interface IUpdateCheckLogic {
        Update UpdateAvailable(Preferences preferences);
    }
}
