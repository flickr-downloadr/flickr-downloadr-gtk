namespace FloydPink.Flickr.Downloadr.Presentation.Views {
    using Model;

    public interface IPreferencesView : IBaseView {
        Preferences Preferences { get; set; }
    }
}
