namespace FloydPink.Flickr.Downloadr.Presentation {
    using System;
    using Views;

    public abstract class PresenterBase {
        protected IBaseView _view { get; private set; }

        protected void HandleException(Exception ex) {
            Console.WriteLine("ERROR: {0}", ex);
            _view.HandleException(ex);
        }
    }
}
