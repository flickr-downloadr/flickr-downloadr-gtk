using System;
using FloydPink.Flickr.Downloadr.Model;

namespace FloydPink.Flickr.Downloadr.OAuth
{
    public class AuthenticatedEventArgs : EventArgs
    {
        public AuthenticatedEventArgs(User user)
        {
            AuthenticatedUser = user;
        }

        public User AuthenticatedUser { get; private set; }
    }
}