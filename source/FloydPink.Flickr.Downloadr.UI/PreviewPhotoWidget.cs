using System;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.UI.CachedImage;

namespace FloydPink.Flickr.Downloadr
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class PreviewPhotoWidget : Gtk.Bin
	{
		public PreviewPhotoWidget (Photo photo)
		{
			this.Build ();
			labelCaption.LabelProp = string.Format ("<span color=\"white\" bgcolor=\"black\"><big><b>      {0}      </b></big></span>", photo.Title);
			imagePreview.SetCachedImage (FileCache.FromUrl (photo.Medium500Url));
		}
	}
}

