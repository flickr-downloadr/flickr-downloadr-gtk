using System;
using System.Diagnostics;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Presentation.Views;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using Gtk;
using log4net;

namespace FloydPink.Flickr.Downloadr.UI.Windows
{
  public abstract class BaseWindow : Window, IBaseView
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

    public bool ShowWarning(string warningMessage)
    {
      Log.Debug("ShowWarning");
      var result = MessageBox.Show(this, warningMessage, ButtonsType.YesNo, MessageType.Question);
      return result != ResponseType.Yes;
    }

    public abstract void ClearSelectedPhotos();

    public void DownloadComplete(string downloadedLocation, bool downloadComplete, DonateIntent intent = null)
    {
      Log.Debug("DownloadComplete");
      Application.Invoke(delegate
      {
        var message = downloadComplete
          ? "Download completed to the directory"
          : "Incomplete download could be found at";
        var readyToDonate = ResponseType.No;
        if (downloadComplete)
        {
          ClearSelectedPhotos();
          if (!intent.Suppressed || intent.DownloadedPhotosCount > 100)
          {
            var donateRequestMessageOpening = intent.DownloadedPhotosCount > 0
                                                    ? string.Format("Download of {0} photos complete", intent.DownloadedPhotosCount)
                                                    : "Download completed";
            readyToDonate = MessageBox.Show(this, string.Format("{0}: {1}{1}{2}",
                                                donateRequestMessageOpening,
                                                Environment.NewLine,
                                                "Would you consider making a small donation?"),
                            ButtonsType.YesNo, MessageType.Question);
          }
        }
        MessageBox.Show(this,
          string.Format("{0}: {1}{1}{2}", message, Environment.NewLine, downloadedLocation),
          ButtonsType.Ok, MessageType.Info);

        if (readyToDonate == ResponseType.Yes)
        {
          Process.Start(VersionHelper.GetDonateUrl("downloadComplete"));
        }
      });
    }

  }
}
