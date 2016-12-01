using System;

namespace FloydPink.Flickr.Downloadr.Presentation.Views
{
  public interface IBaseView
  {
    void ShowSpinner(bool show);
    void HandleException(Exception ex);
  }
}
