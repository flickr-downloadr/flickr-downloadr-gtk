namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using System;
    using System.Diagnostics;
    using Gtk;
    using Helpers;

    public partial class AboutWindow : BaseWindow {
        public AboutWindow() {
            Log.Debug("ctor");
            Build();

            this.labelLink.TooltipText = "http://flickrdownloadr.com";

            Title += VersionHelper.GetVersionString();
            this.labelVersion.LabelProp = string.Format("<big><big>flickr downloadr {0}</big></big>",
                VersionHelper.GetVersionString());
        }

        protected void buttonCloseClick(object sender, EventArgs e) {
            Log.Debug("buttonCloseClick");
            Destroy();
        }

        protected void eventboxHyperlinkClicked(object o, ButtonPressEventArgs args) {
            Log.Debug("eventboxHyperlinkClicked");
            Process.Start(VersionHelper.GetAboutUrl());
        }
    }
}
