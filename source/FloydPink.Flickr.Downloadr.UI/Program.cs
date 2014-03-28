using System;
using Gtk;
using FloydPink.Flickr.Downloadr.Bootstrap;

namespace FloydPink.Flickr.Downloadr
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Bootstrapper.Initialize();

			Application.Init ();
			LoginWindow win = new LoginWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
