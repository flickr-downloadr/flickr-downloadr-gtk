namespace FloydPink.Flickr.Downloadr.Model {
    using System.Collections.Generic;

    public class Session {
        private List<Photoset> _photosets;

        public Session(User user, Preferences preferences, Photoset photoset = null) {
            User = user;
            Preferences = preferences;
            SelectedPhotoset = photoset;
        }

        public User User { get; set; }
        public Preferences Preferences { get; set; }
        public Photoset SelectedPhotoset { get; private set; }
    }
}
