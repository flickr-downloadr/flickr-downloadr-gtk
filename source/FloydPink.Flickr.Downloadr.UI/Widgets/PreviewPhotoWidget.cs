namespace FloydPink.Flickr.Downloadr.UI.Widgets {
    using System.ComponentModel;
    using CachedImage;
    using Gtk;
    using Model;

    [ToolboxItem(true)]
    public partial class PreviewPhotoWidget : Bin {
        public PreviewPhotoWidget(Photo photo) {
            Build();
            this.labelCaption.LabelProp =
                string.Format("<span color=\"white\" bgcolor=\"black\"><big><b>      {0}      </b></big></span>",
                    photo.HtmlEncodedTitle);
            this.imagePreview.SetCachedImage(FileCache.FromUrl(photo.Medium500Url));
        }
    }
}
