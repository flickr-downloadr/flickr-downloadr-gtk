using System;
using System.Globalization;
using System.Threading;

namespace FloydPink.Flickr.Downloadr.Model.Helpers
{
  public static class InvariantCultureHelper
  {
    public static void PerformInInvariantCulture(Action action)
    {
      var before = Thread.CurrentThread.CurrentCulture;
      try
      {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        action();
      } finally
      {
        Thread.CurrentThread.CurrentCulture = before;
      }
    }
  }
}
