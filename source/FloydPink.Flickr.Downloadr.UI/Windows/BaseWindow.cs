using Gtk;
using log4net;

namespace FloydPink.Flickr.Downloadr.UI.Windows {
    public class BaseWindow : Window {
        protected readonly ILog Log;

        protected BaseWindow() : base(WindowType.Toplevel) {
            Log = LogManager.GetLogger(GetType());
        }
    }
}
