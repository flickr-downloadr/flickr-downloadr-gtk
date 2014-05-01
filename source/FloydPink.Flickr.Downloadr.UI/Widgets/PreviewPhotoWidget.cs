using System.ComponentModel;
using System.Web;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.UI.CachedImage;
using Gtk;

namespace FloydPink.Flickr.Downloadr {
    [ToolboxItem(true)]
    public partial class PreviewPhotoWidget : Bin {
        public PreviewPhotoWidget(Photo photo) {
            Build();
            this.labelCaption.LabelProp =
                string.Format("<span color=\"white\" bgcolor=\"black\"><big><b>      {0}      </b></big></span>",
                    HttpUtility.HtmlEncode(photo.Title));
            this.imagePreview.SetCachedImage(FileCache.FromUrl(photo.Medium500Url));
        }
    }
}
