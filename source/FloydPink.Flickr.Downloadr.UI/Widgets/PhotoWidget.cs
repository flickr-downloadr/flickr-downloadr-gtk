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

        public PhotoWidget() {
            Build();
            HasTooltip = true;
            QueryTooltip += (object o, QueryTooltipArgs args) => {
                                var photo = ((PhotoWidget) o).WidgetItem;
                                SetupOnHoverImage(args, photo);
                            };
        }

        public string ImageUrl {
            get { return _imageUrl; }
            set {
                _imageUrl = value;
                if (!string.IsNullOrEmpty(_imageUrl)) {
                    imageMain.SetCachedImage(value);
                }
            }
        }

        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (_isSelected != value) {
                    _isSelected = value;
                    var selectedStar = "<span color=\"green\" size=\"x-large\"><b><big> ★ </big></b></span>";                        
                    var unselectedStar = "<span color=\"silver\" size=\"x-large\"><b><big> ☆ </big></b></span>";
                    frameLabel.LabelProp = _isSelected ? selectedStar : unselectedStar;
                    if (_isSelected) {
                        frameMain.ModifyBg(StateType.Normal, new Color(0, 255, 0));
                    } else {
                        frameMain.ModifyBg(StateType.Normal);
                    }
                    if (SelectionChanged != null) {
                        SelectionChanged(this, new EventArgs());
                    }
                }
            }
        }

        public IGridWidgetItem WidgetItem { get; set; }
        public event EventHandler SelectionChanged;

        protected void imageClick(object o, ButtonPressEventArgs args) {
            IsSelected = !IsSelected;
        }

        private void SetupOnHoverImage(QueryTooltipArgs args, IGridWidgetItem photo) {
            if (photo.GetType() == typeof(Photo)) {
                var customTooltip = new PreviewPhotoWidget((Photo)photo);
                args.Tooltip.Custom = customTooltip;
                args.RetVal = true;
            }
        }
    }
}
