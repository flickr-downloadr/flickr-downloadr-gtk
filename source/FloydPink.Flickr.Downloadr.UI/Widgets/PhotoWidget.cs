using System;
using System.ComponentModel;
using FloydPink.Flickr.Downloadr.Model;
using Gdk;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Widgets {
    [ToolboxItem(true)]
    public partial class PhotoWidget : Bin {
        private string _imageUrl;

        private bool _isSelected;

        public PhotoWidget() {
            Build();
            HasTooltip = true;
            QueryTooltip += (object o, QueryTooltipArgs args) => {
                                Photo photo = ((PhotoWidget) o).Photo;
                                SetupOnHoverImage(args, photo);
                            };
        }

        public string ImageUrl {
            get { return this._imageUrl; }
            set {
                this._imageUrl = value;
                if (!string.IsNullOrEmpty(this._imageUrl)) {
                    this.imageMain.SetCachedImage(value);
                }
            }
        }

        public bool IsSelected {
            get { return this._isSelected; }
            set {
                if (this._isSelected != value) {
                    this._isSelected = value;
                    this.frameLabel.LabelProp = this._isSelected
                        ? "<span color=\"green\" size=\"x-large\"><b><big> ★ </big></b></span>"
                        : "<span color=\"silver\" size=\"x-large\"><b><big> ☆ </big></b></span>";
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

        public Photo Photo { get; set; }

        public event EventHandler SelectionChanged;

        protected void imageClick(object o, ButtonPressEventArgs args) {
            IsSelected = !IsSelected;
        }

        private void SetupOnHoverImage(QueryTooltipArgs args, Photo photo) {
            var customTooltip = new PreviewPhotoWidget(photo);
            args.Tooltip.Custom = customTooltip;
            args.RetVal = true;
        }
    }
}
