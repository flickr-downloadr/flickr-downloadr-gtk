namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using Gtk;
    using log4net;

    public class BaseWindow : Window {
        protected readonly ILog Log;

        protected BaseWindow() : base(WindowType.Toplevel) {
            Log = LogManager.GetLogger(GetType());
        }
    }
}
