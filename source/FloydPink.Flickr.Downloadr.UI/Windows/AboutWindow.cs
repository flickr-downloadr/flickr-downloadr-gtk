using System;
using System.Diagnostics;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Windows
{
  public partial class AboutWindow : BaseWindow
  {
    public AboutWindow()
    {
      Log.Debug("ctor");
      Build();

      labelLink.TooltipText = "https://flickrdownloadr.com";

      Title += VersionHelper.GetVersionString();
      labelVersion.LabelProp = string.Format("<big><big>flickr downloadr {0}</big></big>",
        VersionHelper.GetVersionString());
    }

    protected void buttonCloseClick(object sender, EventArgs e)
    {
      Log.Debug("buttonCloseClick");
      Destroy();
    }

    protected void eventboxHyperlinkClicked(object o, ButtonPressEventArgs args)
    {
      Log.Debug("eventboxHyperlinkClicked");
      Process.Start(VersionHelper.GetAboutUrl());
    }
  }
}
