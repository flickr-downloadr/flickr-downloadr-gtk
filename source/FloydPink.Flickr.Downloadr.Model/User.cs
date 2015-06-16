namespace FloydPink.Flickr.Downloadr.Model {
    using System.Web.Script.Serialization;

    public class User {
        public User() {
            this.Name = string.Empty;
            this.Username = string.Empty;
            this.UserNsId = string.Empty;
        }

        public User(string name, string userName, string userNsId) {
            this.Name = name;
            this.Username = userName;
            this.UserNsId = userNsId;
        }

        public string Name { get; set; }
        public string Username { get; set; }
        public string UserNsId { get; set; }

        [ScriptIgnore]
        public UserInfo Info { get; set; }

        public string WelcomeMessage
        {
            get
            {
                var userNameString = string.IsNullOrEmpty(Name)
                    ? (string.IsNullOrEmpty(Username) ? string.Empty : Username)
                    : Name;
                return string.IsNullOrEmpty(userNameString)
                    ? string.Empty
                    : string.Format("Welcome, {0}!", userNameString);
            }
        }
    }
}
