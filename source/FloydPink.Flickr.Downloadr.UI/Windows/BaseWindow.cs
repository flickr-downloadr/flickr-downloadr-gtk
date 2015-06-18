namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using System;
    using Gtk;
    using log4net;
    using Presentation.Views;

    public class BaseWindow : Window, IBaseView {
        protected readonly ILog Log;

        protected BaseWindow() : base(WindowType.Toplevel) {
            this.Log = LogManager.GetLogger(GetType());
        }

        public virtual void ShowSpinner(bool show) {
            
        }

        public void HandleException(Exception ex) {
            Application.Invoke(delegate {
                                   throw ex;
                               });
        }
    }
}
