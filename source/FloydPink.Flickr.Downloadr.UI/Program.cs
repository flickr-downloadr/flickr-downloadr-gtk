namespace FloydPink.Flickr.Downloadr.UI {
    using System;
    using Windows;
    using Bootstrap;
    using GLib;
    using Gtk;
    using Helpers;
    using log4net;

    internal class MainClass {
        private static readonly ILog Log = LogManager.GetLogger(typeof (MainClass));

        public static void Main(string [] args) {
            Bootstrapper.Initialize();
            Log.Info("Application Start.");

            Application.Init();
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            UnhandledExceptionHandler handler = OnUnhandledException;
            ExceptionManager.UnhandledException += handler;

            Application.Run();
        }

        public static void Quit() {
            Log.Info("Application Exit.");
            Application.Quit();
        }

        private static void OnUnhandledException(UnhandledExceptionArgs args) {
            Log.Fatal("Unhandled Exception.", (Exception) args.ExceptionObject);
            Console.WriteLine("Fatal Error: {0} ... {1}", args.ExceptionObject, args.IsTerminating);
            MessageBox.Show(new FatalErrorWindow(), "Unhandled exception.\n" +
                                                    "Please raise an issue on the Support website.",
                ButtonsType.Ok, MessageType.Error);
            args.ExitApplication = true;
        }
    }
}
