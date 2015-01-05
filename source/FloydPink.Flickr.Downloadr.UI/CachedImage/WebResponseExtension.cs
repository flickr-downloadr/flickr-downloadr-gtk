namespace FloydPink.Flickr.Downloadr.UI.CachedImage {
    using System.IO;
    using System.Net;

    public static class WebResponseExtension {
        public static byte [] ReadToEnd(this WebResponse webresponse) {
            var responseStream = webresponse.GetResponseStream();

            using (var memoryStream = new MemoryStream((int) webresponse.ContentLength)) {
                if (responseStream != null) {
                    responseStream.CopyTo(memoryStream);
                }
                return memoryStream.ToArray();
            }
        }
    }
}
