namespace FloydPink.Flickr.Downloadr.Presentation.Views {
    using System;

    public interface IBaseView {
        void ShowSpinner(bool show);
        void HandleException(Exception ex);
    }
}
