using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Logic.Interfaces {
    public interface IUpdateCheckLogic {
        Update UpdateAvailable(Preferences preferences);
    }
}
