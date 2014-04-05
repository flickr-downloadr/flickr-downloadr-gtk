using System;
using System.Net;
using System.IO;

namespace FloydPink.Flickr.Downloadr.UI.CachedImage
{
	public static class WebResponseExtension
	{
		public static byte[] ReadToEnd(this WebResponse webresponse)
		{
			Stream responseStream = webresponse.GetResponseStream();

			using (var memoryStream = new MemoryStream((int) webresponse.ContentLength))
			{
				if (responseStream != null) responseStream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}
	}
}

