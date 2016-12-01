using System;
using System.ComponentModel;
using System.Reflection;
using Gdk;
using Gtk;

namespace FloydPink.Flickr.Downloadr.UI.Widgets
{
  [ToolboxItem(true)]
  public partial class SpinnerWidget : Bin
  {
    private bool _cancellable;
    private string _operation;
    private string _percentDone;

    public SpinnerWidget()
    {
      Build();

      imageLoading.PixbufAnimation = new PixbufAnimation(Assembly.GetAssembly(typeof(SpinnerWidget)),
        "FloydPink.Flickr.Downloadr.UI.Assets.loading.gif");

      labelOperation.Visible = false;
      labelPercent.Visible = false;
      buttonCancel.Visible = false;

      buttonCancel.TooltipText = "Cancel the operation";
    }

    public bool Cancellable
    {
      get
      {
        return _cancellable;
      }
      set
      {
        _cancellable = value;
        buttonCancel.Visible = _cancellable;
      }
    }

    public string PercentDone
    {
      get
      {
        return _percentDone;
      }
      set
      {
        _percentDone = value;
        labelPercent.LabelProp = _percentDone;
        labelPercent.Visible = !string.IsNullOrEmpty(_percentDone);
      }
    }

    public string Operation
    {
      get
      {
        return _operation;
      }
      set
      {
        _operation = value;
        labelOperation.LabelProp = _operation;
        labelOperation.Visible = !string.IsNullOrEmpty(_operation);
      }
    }

    public event EventHandler SpinnerCanceled;

    protected void buttonCancelClick(object sender, EventArgs e)
    {
      Visible = false;
      if (SpinnerCanceled != null)
      {
        SpinnerCanceled(this, new EventArgs());
      }
    }
  }
}
