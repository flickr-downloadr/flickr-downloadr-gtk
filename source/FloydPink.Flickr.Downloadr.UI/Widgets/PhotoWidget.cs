using System;
using System.ComponentModel;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.UI.CachedImage;
using Gdk;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Widgets
{
  [ToolboxItem(true)]
  public partial class PhotoWidget : Bin
  {
    private string _imageUrl;
    private bool _isSelected;
    private IGridWidgetItem _widgetItem;

    public PhotoWidget()
    {
      Build();
      HasTooltip = true;
      QueryTooltip += (o, args) =>
      {
        var photo = ((PhotoWidget) o).WidgetItem;
        SetupOnHoverImage(args, photo);
      };
    }

    public string ImageUrl
    {
      get
      {
        return _imageUrl;
      }
      set
      {
        _imageUrl = value;
        if (!string.IsNullOrEmpty(_imageUrl))
        {
          imageMain.SetCachedImage(value);
        }
      }
    }

    public bool IsSelected
    {
      get
      {
        return _isSelected;
      }
      set
      {
        if (_isSelected != value)
        {
          _isSelected = value;
          var selectedStar = "<span color=\"green\" size=\"x-large\"><b><big> ★ </big></b></span>";
          var unselectedStar = "<span color=\"silver\" size=\"x-large\"><b><big> ☆ </big></b></span>";
          frameLabel.LabelProp = _isSelected ? selectedStar : unselectedStar;
          if (_isSelected)
          {
            frameMain.ModifyBg(StateType.Normal, new Color(0, 255, 0));
          }
          else
          {
            frameMain.ModifyBg(StateType.Normal);
          }
          if (SelectionChanged != null)
          {
            SelectionChanged(this, new EventArgs());
          }
        }
      }
    }

    public IGridWidgetItem WidgetItem
    {
      get
      {
        return _widgetItem;
      }
      set
      {
        _widgetItem = value;
        if (value != null)
        {
          ImageUrl = value.WidgetThumbnailUrl;
        }
      }
    }

    public event EventHandler SelectionChanged;

    protected void imageClick(object o, ButtonPressEventArgs args)
    {
      IsSelected = !IsSelected;
    }

    private void SetupOnHoverImage(QueryTooltipArgs args, IGridWidgetItem photo)
    {
      if (photo == null)
      {
        return;
      }
      if (photo.GetType() == typeof(Photo))
      {
        var previewPhotoTooltip = new PreviewPhotoWidget((Photo) photo);
        args.Tooltip.Custom = previewPhotoTooltip;
      }
      else
      {
        var albumName = ((Photoset) photo).HtmlEncodedTitle;
        var albumNameToolTip = new Label(albumName);
        albumNameToolTip.UseMarkup = true;
        args.Tooltip.Custom = albumNameToolTip;
      }
      args.RetVal = true;
    }
  }
}
