using System;

namespace FloydPink.Flickr.Downloadr.Model {
    public class Update {
        public Update() {
            LastChecked = DateTime.MinValue;
            Available = false;
            LatestVersion = string.Empty;
        }

        public DateTime LastChecked { get; set; }
        public bool Available { get; set; }
        public string LatestVersion { get; set; }
    }
}
