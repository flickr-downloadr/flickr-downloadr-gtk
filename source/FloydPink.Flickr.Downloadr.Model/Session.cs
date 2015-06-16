namespace FloydPink.Flickr.Downloadr.Model {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class Session {

        private List<Photoset> _photosets;

        public Session(User user, Preferences preferences, List<Photoset> photosets = null) {
            this.User = user;
            this.Preferences = preferences;
            this.Photosets = photosets;
        }

        public User User { get; set; }

        public Preferences Preferences { get; set; }

        public List<Photoset> Photosets { 
            get { 
                return this._photosets ?? new List<Photoset>();
            } 
            set { 
                this._photosets = value;
            } 
        }

        public Photoset SelectedPhotoset { get; set; }

    }
}
