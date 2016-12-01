using System;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using Gtk;
using log4net;

namespace FloydPink.Flickr.Downloadr.UI.Windows
{
  public class BaseWindow : Window, IBaseView
  {
    protected readonly ILog Log;

    protected BaseWindow() : base(WindowType.Toplevel)
    {
      Log = LogManager.GetLogger(GetType());
    }

    public virtual void ShowSpinner(bool show) {}

    public void HandleException(Exception ex)
    {
      Application.Invoke(delegate { throw ex; });
    }
  }
}
