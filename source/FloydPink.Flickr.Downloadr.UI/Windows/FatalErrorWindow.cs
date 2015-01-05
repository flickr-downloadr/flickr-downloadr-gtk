namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using Helpers;

    public partial class FatalErrorWindow : BaseWindow {
        public FatalErrorWindow() {
            Log.Debug("ctor");
            Build();
            Title += VersionHelper.GetVersionString();
        }
    }
}
