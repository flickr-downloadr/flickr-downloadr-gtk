using System.ComponentModel;
using System.Web.Script.Serialization;
using FloydPink.Flickr.Downloadr.Model.Extensions;

namespace FloydPink.Flickr.Downloadr.Model {
    public class User : INotifyPropertyChanged {
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

        public string Name {
            get { return this._name; }
            set {
                this._name = value;
                PropertyChanged.Notify(() => Name);
            }
        }

        public string Username {
            get { return this._userName; }
            set {
                this._userName = value;
                PropertyChanged.Notify(() => Username);
            }
        }

        public string UserNsId {
            get { return this._userNsId; }
            set {
                this._userNsId = value;
                PropertyChanged.Notify(() => UserNsId);
            }
        }

        [ScriptIgnore]
        public UserInfo Info {
            get { return this._info; }
            set {
                this._info = value;
                PropertyChanged.Notify(() => Info);
            }
        }

        public string WelcomeMessage {
            get {
                string userNameString = string.IsNullOrEmpty(Name)
                    ? (string.IsNullOrEmpty(Username) ? string.Empty : Username)
                    : Name;
                return string.IsNullOrEmpty(userNameString)
                    ? string.Empty
                    : string.Format("Welcome, {0}!", userNameString);
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
