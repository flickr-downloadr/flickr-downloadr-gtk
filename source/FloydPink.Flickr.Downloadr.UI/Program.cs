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
		public static bool RunningOnMono = Type.GetType ("Mono.Runtime") != null;

		public static void Main (string[] args)
		{
			Bootstrapper.Initialize ();

			Application.Init ();
			LoginWindow loginWindow = new LoginWindow ();
			loginWindow.Show ();

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
