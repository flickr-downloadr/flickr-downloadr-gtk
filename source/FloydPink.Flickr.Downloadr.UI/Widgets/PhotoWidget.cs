namespace FloydPink.Flickr.Downloadr.UI.Widgets {
    using System;
    using System.ComponentModel;
    using Gdk;
    using Gtk;
    using Model;

    [ToolboxItem(true)]
    public partial class PhotoWidget : Bin {
        private string _imageUrl;
        private bool _isSelected;
        private IGridWidgetItem _widgetItem;

        public PhotoWidget() {
            Build();
            HasTooltip = true;
            QueryTooltip += (object o, QueryTooltipArgs args) => {
                                var photo = ((PhotoWidget) o).WidgetItem;
                                SetupOnHoverImage(args, photo);
                            };
        }

        public string ImageUrl
        {
            get { return this._imageUrl; }
            set
            {
                this._imageUrl = value;
                if (!string.IsNullOrEmpty(this._imageUrl)) {
                    this.imageMain.SetCachedImage(value);
                }
            }
        }

        public bool IsSelected
        {
            get { return this._isSelected; }
            set
            {
                if (this._isSelected != value) {
                    this._isSelected = value;
                    var selectedStar = "<span color=\"green\" size=\"x-large\"><b><big> ★ </big></b></span>";
                    var unselectedStar = "<span color=\"silver\" size=\"x-large\"><b><big> ☆ </big></b></span>";
                    this.frameLabel.LabelProp = this._isSelected ? selectedStar : unselectedStar;
                    if (this._isSelected) {
                        this.frameMain.ModifyBg(StateType.Normal, new Color(0, 255, 0));
                    } else {
                        this.frameMain.ModifyBg(StateType.Normal);
                    }
                    if (SelectionChanged != null) {
                        SelectionChanged(this, new EventArgs());
                    }
                }
            }
        }

        public IGridWidgetItem WidgetItem
        {
            get { return this._widgetItem; }
            set
            {
                this._widgetItem = value;
                if (value != null) {
                    ImageUrl = value.WidgetThumbnailUrl;
                }
            }
        }

        public event EventHandler SelectionChanged;

        protected void imageClick(object o, ButtonPressEventArgs args) {
            IsSelected = !IsSelected;
        }

        private void SetupOnHoverImage(QueryTooltipArgs args, IGridWidgetItem photo) {
            if (photo == null) {
                return;
            }
            if (photo.GetType() == typeof (Photo)) {
                var previewPhotoTooltip = new PreviewPhotoWidget((Photo) photo);
                args.Tooltip.Custom = previewPhotoTooltip;
            } else {
                var albumName = ((Photoset) photo).HtmlEncodedTitle;
                var albumNameToolTip = new Label(albumName);
                albumNameToolTip.UseMarkup = true;
                args.Tooltip.Custom = albumNameToolTip;
            }
            args.RetVal = true;
        }
    }
}
