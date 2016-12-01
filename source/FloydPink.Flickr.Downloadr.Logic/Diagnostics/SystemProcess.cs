using System.Diagnostics;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;

namespace FloydPink.Flickr.Downloadr.Logic.Diagnostics
{
  public class SystemProcess : ISystemProcess
  {
    public void Start(ProcessStartInfo processStartInfo)
    {
      Process.Start(processStartInfo);
    }
  }
}
