using System.ComponentModel;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.UI.CachedImage;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Widgets
{
  [ToolboxItem(true)]
  public partial class PreviewPhotoWidget : Bin
  {
    public PreviewPhotoWidget(Photo photo)
    {
      Build();
      labelCaption.LabelProp =
        string.Format("<span color=\"white\" bgcolor=\"black\"><big><b>      {0}      </b></big></span>",
          photo.HtmlEncodedTitle);
      imagePreview.SetCachedImage(FileCache.FromUrl(photo.Medium500Url));
    }
  }
}
