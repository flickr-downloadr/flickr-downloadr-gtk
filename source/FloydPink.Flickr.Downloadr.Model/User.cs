using System.ComponentModel;
using System.Web.Script.Serialization;
using FloydPink.Flickr.Downloadr.Model.Extensions;

namespace FloydPink.Flickr.Downloadr.Model
{
    public class User : INotifyPropertyChanged
    {
        [ScriptIgnore] private UserInfo _info;
        private string _name;
        private string _userName;
        private string _userNsId;

        public User()
        {
            _name = string.Empty;
            _userName = string.Empty;
            _userNsId = string.Empty;
        }

        public User(string name, string userName, string userNsId)
        {
            _name = name;
            _userName = userName;
            _userNsId = userNsId;
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged.Notify(() => Name);
            }
        }

        public string Username
        {
            get { return _userName; }
            set
            {
                _userName = value;
                PropertyChanged.Notify(() => Username);
            }
        }

        public string UserNsId
        {
            get { return _userNsId; }
            set
            {
                _userNsId = value;
                PropertyChanged.Notify(() => UserNsId);
            }
        }

        [ScriptIgnore]
        public UserInfo Info
        {
            get { return _info; }
            set
            {
                _info = value;
                PropertyChanged.Notify(() => Info);
            }
        }

        public string WelcomeMessage
        {
            get
            {
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