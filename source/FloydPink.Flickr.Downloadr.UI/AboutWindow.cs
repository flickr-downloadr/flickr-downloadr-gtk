using System;
using System.Diagnostics;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using Gtk;

namespace FloydPink.Flickr.Downloadr {
    public partial class AboutWindow : Window {
        public AboutWindow() :
            base(WindowType.Toplevel) {
            Build();

            this.labelLink.TooltipText = "http://flickrdownloadr.com";

            Title += VersionHelper.GetVersionString();
            this.labelVersion.LabelProp = string.Format("<big><big>flickr downloadr {0}</big></big>",
                VersionHelper.GetVersionString());
        }

        protected void buttonCloseClick(object sender, EventArgs e) {
            Destroy();
        }

        protected void eventboxHyperlinkClicked(object o, ButtonPressEventArgs args) {
            Process.Start(VersionHelper.GetAboutUrl());
        }
    }
}
