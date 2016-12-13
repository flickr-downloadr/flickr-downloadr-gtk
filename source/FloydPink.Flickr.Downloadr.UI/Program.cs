using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using FloydPink.Flickr.Downloadr.Bootstrap;
using FloydPink.Flickr.Downloadr.UI.Helpers;
using FloydPink.Flickr.Downloadr.UI.Windows;
using GLib;
using Gtk;
using log4net;
using Microsoft.Win32;
using Process = System.Diagnostics.Process;

namespace FloydPink.Flickr.Downloadr.UI
{
  internal class MainClass
  {
    private static readonly ILog Log = LogManager.GetLogger(typeof(MainClass));

    public static void Main(string[] args)
    {
      Bootstrapper.Initialize();
      Log.Info("Application Start.");

      if (PlatformDetection.IsWindows && !CheckWindowsGtk())
      {
        throw new SystemException(
          "Cannot find GTKSharp - please raise an issue at https://github.com/flickr-downloadr/flickr-downloadr-gtk/issues/new");
      }

      Application.Init();
      var loginWindow = new LoginWindow();
      loginWindow.Show();

      UnhandledExceptionHandler handler = OnUnhandledException;
      ExceptionManager.UnhandledException += handler;

      Application.Run();
    }

    public static void Quit()
    {
      Log.Info("Application Exit.");
      Application.Quit();
    }

    private static void OnUnhandledException(UnhandledExceptionArgs args)
    {
      Log.Fatal("Unhandled Exception.", (Exception) args.ExceptionObject);
      Console.WriteLine("Fatal Error: {0} ... {1}", args.ExceptionObject, args.IsTerminating);
      MessageBox.Show(new FatalErrorWindow(), "Unhandled exception.\n" +
                                              "Please raise an issue on the Support website.",
        ButtonsType.Ok, MessageType.Error);
      args.ExitApplication = true;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);

    private static bool CheckWindowsGtk()
    {
      string location = null;
      Version version = null;
      var minVersion = new Version(2, 12, 22);

      using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Xamarin\GtkSharp\InstallFolder"))
      {
        if (key != null)
        {
          location = key.GetValue(null) as string;
        }
      }
      using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Xamarin\GtkSharp\Version"))
      {
        if (key != null)
        {
          Version.TryParse(key.GetValue(null) as string, out version);
        }
      }

      //TODO: check build version of GTK# dlls in GAC
      if ((version == null) || (version < minVersion) || (location == null) || !File.Exists(Path.Combine(location, "bin", "libgtk-win32-2.0-0.dll")))
      {
        Log.Error("Did not find required GTK# installation");

        if (DisplayOkCancelMessageBox(string.Format("{0} did not find the required version of GTK#. " +
                                                    "Please click OK to open the download page, where you can download and install the latest version.",
          "flickr downloadr"), "Fatal Error"))
        {
          Process.Start("http://www.mono-project.com/download/#download-win");
        }
        return false;
      }

      Log.Info("Found GTK# version " + version);

      var path = Path.Combine(location, @"bin");
      try
      {
        if (SetDllDirectory(path))
        {
          return true;
        }
      } catch (EntryPointNotFoundException) {}

      // this shouldn't happen unless something is weird in Windows
      Log.Error("Unable to set GTK+ dll directory");

      return true;
    }

    private static bool DisplayOkCancelMessageBox(string message, string caption)
    {
      var name = typeof(int).Assembly.FullName.Replace("mscorlib", "System.Windows.Forms");
      var asm = Assembly.Load(name);
      var md = asm.GetType("System.Windows.Forms.MessageBox");
      var mbb = asm.GetType("System.Windows.Forms.MessageBoxButtons");
      var okCancel = Enum.ToObject(mbb, 1);
      var dr = asm.GetType("System.Windows.Forms.DialogResult");
      var ok = Enum.ToObject(dr, 1);

      const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
      return md.InvokeMember("Show", flags, null, null, new[] {message, caption, okCancel}).Equals(ok);
    }
  }
}
