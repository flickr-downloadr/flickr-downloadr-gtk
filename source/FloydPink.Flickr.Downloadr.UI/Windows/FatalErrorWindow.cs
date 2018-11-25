using System;
using FloydPink.Flickr.Downloadr.UI.Helpers;

namespace FloydPink.Flickr.Downloadr.UI.Windows
{
  public partial class FatalErrorWindow : BaseWindow
  {
    public FatalErrorWindow()
    {
      Log.Debug("ctor");
      Build();
      Title += VersionHelper.GetVersionString();
    }

    public override void ClearSelectedPhotos()
    {
      throw new NotImplementedException();
    }

  }
}
