using System.Diagnostics;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.UI.Windows;
using GLib;
using Gtk;
using Systhread = System.Threading;

namespace FloydPink.Flickr.Downloadr {
    internal class MainClass {
        public static void Main(string [] args) {
            Bootstrapper.Initialize();

            Application.Init();
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            UnhandledExceptionHandler handler = OnUnhandledException;
            ExceptionManager.UnhandledException += handler;

            Application.Run();
        }

        private static void OnUnhandledException(UnhandledExceptionArgs args) {
            Debug.WriteLine("Fatal Error: {0} ... {1}", args.ExceptionObject, args.IsTerminating);
            MessageBox.Show(new FatalErrorWindow(), "Unhandled exception.\n" +
                                                    "Please raise an issue on the Support website.",
                ButtonsType.Ok, MessageType.Error);
            args.ExitApplication = true;
        }
    }
}
