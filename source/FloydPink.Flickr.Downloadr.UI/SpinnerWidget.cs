using System;
using System.Reflection;

namespace FloydPink.Flickr.Downloadr
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class SpinnerWidget : Gtk.Bin
	{
		bool _cancellable;
		public bool Cancellable {
			get {
				return _cancellable;
			}
			set {
				_cancellable = value;
				buttonCancel.Visible = _cancellable;
			}
		}

		string _percentDone;
		public string PercentDone {
			get {
				return _percentDone;
			}
			set {
				_percentDone = value;
				labelPercent.LabelProp = _percentDone;
				labelPercent.Visible = !string.IsNullOrEmpty (_percentDone);
			}
		}

		string _operation;
		public string Operation {
			get {
				return _operation;
			}
			set {
				_operation = value;
				labelOperation.LabelProp = _operation;
				labelOperation.Visible = !string.IsNullOrEmpty (_operation);
			}
		}

		public event EventHandler SpinnerCanceled;

		public SpinnerWidget ()
		{
			this.Build ();

			imageLoading.PixbufAnimation = new Gdk.PixbufAnimation (Assembly.GetAssembly (typeof(SpinnerWidget)),
				"FloydPink.Flickr.Downloadr.Assets.loading.gif");

			labelOperation.Visible = false;
			labelPercent.Visible = false;
			buttonCancel.Visible = false;

			buttonCancel.TooltipText = "Cancel the operation";
		}

		protected void buttonCancelClick (object sender, EventArgs e)
		{
			this.Visible = false;
			if (SpinnerCanceled != null) {
				SpinnerCanceled (this, new EventArgs ());
			}
		}
	}
}

