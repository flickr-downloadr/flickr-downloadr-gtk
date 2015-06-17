namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using System;
    using Gtk;
    using log4net;

    public class BaseWindow : Window {
        protected readonly ILog Log;

        protected BaseWindow() : base(WindowType.Toplevel) {
            Log = LogManager.GetLogger(GetType());
        }

        public void HandleException(Exception ex) {
            Application.Invoke(delegate {
                Console.WriteLine("############################\n############################\n############################\n");
                throw ex;
            });
        }
    }
}
