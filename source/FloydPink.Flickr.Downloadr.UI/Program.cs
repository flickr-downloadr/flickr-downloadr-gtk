using System;
using System.Diagnostics;
using Systhread = System.Threading;
using Gtk;
using GLib;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.UI.Helpers;

namespace FloydPink.Flickr.Downloadr
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Bootstrapper.Initialize ();

			Application.Init ();
			SplashWindow win = new SplashWindow ();
			win.Show ();

			var thread = new Systhread.Thread (delegate() {
				LoginWindow loginWindow = new LoginWindow ();
				Systhread.Thread.Sleep (2000);
				loginWindow.Show ();
				win.Destroy ();
			});
			thread.Start ();

			UnhandledExceptionHandler handler = new UnhandledExceptionHandler (OnUnhandledException);
			ExceptionManager.UnhandledException += handler;

			Application.Run ();
		}

		static void OnUnhandledException (UnhandledExceptionArgs args)
		{
			Debug.WriteLine ("Fatal Error: {0} ... {1}", args.ExceptionObject, args.IsTerminating);
			MessageBox.Show (new FatalErrorWindow (), "Unhandled exception.\n" +
			"Please raise an issue on the Support website.", 
				ButtonsType.Ok, MessageType.Error);
			args.ExitApplication = true;
		}
	}
}
