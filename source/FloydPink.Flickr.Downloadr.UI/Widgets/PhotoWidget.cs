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
                                var photo = ((PhotoWidget) o).Photo;
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
                    frameLabel.LabelProp = _isSelected
                        ? "<span color=\"green\" size=\"x-large\"><b><big> ★ </big></b></span>"
                        : "<span color=\"silver\" size=\"x-large\"><b><big> ☆ </big></b></span>";
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

        public IPhotoWidget Photo { get; set; }
        public event EventHandler SelectionChanged;

        protected void imageClick(object o, ButtonPressEventArgs args) {
            IsSelected = !IsSelected;
        }

        private void SetupOnHoverImage(QueryTooltipArgs args, IPhotoWidget photo) {
            if (photo.GetType() == typeof(Photo)) {
                var customTooltip = new PreviewPhotoWidget((Photo)photo);
                args.Tooltip.Custom = customTooltip;
                args.RetVal = true;
            }
        }
    }
}
