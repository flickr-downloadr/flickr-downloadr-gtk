namespace FloydPink.Flickr.Downloadr.UI.CachedImage {
    using System.IO;
    using System.Net;

    public class HttpHelper {
        private static byte [] Get(string url) {
            var request = WebRequest.Create(url);
            var response = request.GetResponse();

            return response.ReadToEnd();
        }

        public static void GetAndSaveToFile(string url, string fileName) {
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
                var data = Get(url);
                stream.Write(data, 0, data.Length);
            }
        }
    }
}
