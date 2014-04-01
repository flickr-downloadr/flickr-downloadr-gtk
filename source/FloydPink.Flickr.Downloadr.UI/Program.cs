using System;
using System.Threading;
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
			SplashWindow win = new SplashWindow ();
			win.Show ();

			var thread = new Thread (delegate() {
				LoginWindow loginWindow = new LoginWindow();
				Thread.Sleep(2000);
				loginWindow.Show();
				win.Destroy();
			});
			thread.Start ();

			Application.Run ();
		}
	}
}
