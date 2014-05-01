using FloydPink.Flickr.Downloadr.UI.Helpers;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Windows {
    public partial class FatalErrorWindow : Window {
        public FatalErrorWindow() :
            base(WindowType.Toplevel) {
            Build();
            Title += VersionHelper.GetVersionString();
        }
    }
}
