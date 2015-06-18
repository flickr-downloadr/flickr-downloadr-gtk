namespace FloydPink.Flickr.Downloadr.UI.Widgets {
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using Gdk;
    using Gtk;

    [ToolboxItem(true)]
    public partial class SpinnerWidget : Bin {
        private bool _cancellable;
        private string _operation;
        private string _percentDone;

        public SpinnerWidget() {
            Build();

            this.imageLoading.PixbufAnimation = new PixbufAnimation(Assembly.GetAssembly(typeof (SpinnerWidget)),
                "FloydPink.Flickr.Downloadr.UI.Assets.loading.gif");

            this.labelOperation.Visible = false;
            this.labelPercent.Visible = false;
            this.buttonCancel.Visible = false;

            this.buttonCancel.TooltipText = "Cancel the operation";
        }

        public bool Cancellable
        {
            get { return this._cancellable; }
            set
            {
                this._cancellable = value;
                this.buttonCancel.Visible = this._cancellable;
            }
        }

        public string PercentDone
        {
            get { return this._percentDone; }
            set
            {
                this._percentDone = value;
                this.labelPercent.LabelProp = this._percentDone;
                this.labelPercent.Visible = !string.IsNullOrEmpty(this._percentDone);
            }
        }

        public string Operation
        {
            get { return this._operation; }
            set
            {
                this._operation = value;
                this.labelOperation.LabelProp = this._operation;
                this.labelOperation.Visible = !string.IsNullOrEmpty(this._operation);
            }
        }

        public event EventHandler SpinnerCanceled;

        protected void buttonCancelClick(object sender, EventArgs e) {
            Visible = false;
            if (SpinnerCanceled != null) {
                SpinnerCanceled(this, new EventArgs());
            }
        }
    }
}
