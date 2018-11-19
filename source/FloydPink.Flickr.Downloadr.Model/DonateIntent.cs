using System;

namespace FloydPink.Flickr.Downloadr.Model
{
  public class DonateIntent
  {
    public DonateIntent()
    {
      LastChecked = DateTime.MinValue;
      Suppressed = false;
    }

    public DateTime LastChecked { get; set; }
    public bool Suppressed { get; set; }
  }
}
