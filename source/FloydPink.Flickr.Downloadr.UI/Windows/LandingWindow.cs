namespace FloydPink.Flickr.Downloadr.UI.Windows {
    using Bootstrap;
    using Gtk;
    using System;
    using Helpers;
    using Model;
    using Presentation;
    using Presentation.Views;

    public partial class LandingWindow : BaseWindow, ILandingView {
        private readonly ILandingPresenter _presenter;

        public LandingWindow(User user, Preferences preferences) {
            Log.Debug("ctor");
            Build();

            Title += VersionHelper.GetVersionString();
            Preferences = preferences;
            User = user;

            _presenter = Bootstrapper.GetPresenter<ILandingView, ILandingPresenter>(this);
            _presenter.Initialize();
        }

        public User User { get; set; }
        public Preferences Preferences { get; set; }

        public void ShowSpinner(bool show) {
            Log.Debug("ShowSpinner");
            Application.Invoke(delegate {
//                                   this.hboxLogin.Sensitive = !show;
//                                   this.spinner.Visible = show;
                               });
        }

    }
}

