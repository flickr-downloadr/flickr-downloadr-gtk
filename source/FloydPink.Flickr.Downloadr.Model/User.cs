namespace FloydPink.Flickr.Downloadr.Model {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Script.Serialization;

    public class User {
        [ScriptIgnore] private UserInfo _info;
        private string _name;
        private string _userName;
        private string _userNsId;

        public User() {
            this._name = string.Empty;
            this._userName = string.Empty;
            this._userNsId = string.Empty;
        }

        public User(string name, string userName, string userNsId) {
            this._name = name;
            this._userName = userName;
            this._userNsId = userNsId;
        }

        public string Name
        {
            get { return this._name; }
            set
            {
                this._name = value;
            }
        }

        public string Username
        {
            get { return this._userName; }
            set
            {
                this._userName = value;
            }
        }

        public string UserNsId
        {
            get { return this._userNsId; }
            set
            {
                this._userNsId = value;
            }
        }

        [ScriptIgnore]
        public UserInfo Info
        {
            get { return this._info; }
            set
            {
                this._info = value;
            }
        }

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
