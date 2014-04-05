using System;
using System.IO;
using System.Net;

namespace FloydPink.Flickr.Downloadr.UI.CachedImage
{
	public class HttpHelper
	{
		public static byte[] Get(string url)
		{
			WebRequest request = WebRequest.Create(url);
			WebResponse response = request.GetResponse();

			return response.ReadToEnd();
		}

		public static void GetAndSaveToFile(string url, string fileName)
		{
			using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				byte[] data = Get(url);
				stream.Write(data, 0, data.Length);
			}
		}
	}
}

