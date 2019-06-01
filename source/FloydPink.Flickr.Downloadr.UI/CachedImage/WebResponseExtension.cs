using System.IO;
using System.Net;

namespace FloydPink.Flickr.Downloadr.UI.CachedImage
{
  public static class WebResponseExtension
  {
    public static byte[] ReadToEnd(this WebResponse webresponse)
    {
      var responseStream = webresponse.GetResponseStream();

      using (var memoryStream = new MemoryStream())
      {
        if (responseStream != null)
        {
          responseStream.CopyTo(memoryStream);
        }
        return memoryStream.ToArray();
      }
    }
  }
}
