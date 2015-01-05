namespace FloydPink.Flickr.Downloadr.OAuth {
    using System;
    using Model;

    public class AuthenticatedEventArgs : EventArgs {
        public AuthenticatedEventArgs(User user) {
            AuthenticatedUser = user;
        }

        public User AuthenticatedUser { get; private set; }
    }
}
