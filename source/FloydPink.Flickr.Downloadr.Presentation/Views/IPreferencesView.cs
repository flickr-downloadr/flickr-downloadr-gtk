using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.Presentation.Views
{
    public interface IPreferencesView : IBaseView
    {
        Preferences Preferences { get; set; }
    }
}