namespace FloydPink.Flickr.Downloadr.Presentation.Views {
    using System.Collections.Generic;
    using Model;
    using Model.Enums;

    public interface ILandingView : IBaseView {
        User User { get; set; }
        Preferences Preferences { get; set; }
    }
}
