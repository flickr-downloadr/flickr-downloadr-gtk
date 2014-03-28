using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FloydPink.Flickr.Downloadr.Logic.Interfaces;
using FloydPink.Flickr.Downloadr.Model;
using FloydPink.Flickr.Downloadr.Model.Enums;
using FloydPink.Flickr.Downloadr.Repository.Extensions;

namespace FloydPink.Flickr.Downloadr.Logic
{
    public class DownloadLogic : IDownloadLogic
    {
        private static readonly Random Random = new Random((int) DateTime.Now.Ticks);
        private readonly IOriginalTagsLogic _originalTagsLogic;
        private string _currentTimestampFolder;

        public DownloadLogic(IOriginalTagsLogic originalTagsLogic)
        {
            _originalTagsLogic = originalTagsLogic;
        }

        public async Task Download(IEnumerable<Photo> photos, CancellationToken cancellationToken,
            IProgress<ProgressUpdate> progress, Preferences preferences)
        {
            await DownloadPhotos(photos, cancellationToken, progress, preferences);
        }

        private async Task DownloadPhotos(IEnumerable<Photo> photos, CancellationToken cancellationToken,
            IProgress<ProgressUpdate> progress, Preferences preferences)
        {
            try
            {
                var progressUpdate = new ProgressUpdate
                {
                    Cancellable = true,
                    OperationText = "Downloading photos...",
                    PercentDone = 0,
                    ShowPercent = true
                };
                progress.Report(progressUpdate);

                int doneCount = 0;
                IList<Photo> photosList = photos as IList<Photo> ?? photos.ToList();
                int totalCount = photosList.Count();

                DirectoryInfo imageDirectory = CreateDownloadFolder(preferences.DownloadLocation);

                foreach (Photo photo in photosList)
                {
                    string photoUrl = photo.OriginalUrl;
                    string photoExtension = "jpg";
                    switch (preferences.DownloadSize)
                    {
                        case PhotoDownloadSize.Medium:
                            photoUrl = photo.Medium800Url;
                            break;
                        case PhotoDownloadSize.Large:
                            photoUrl = photo.Large1024Url;
                            break;
                        case PhotoDownloadSize.Original:
                            photoUrl = photo.OriginalUrl;
                            photoExtension = photo.DownloadFormat;
                            break;
                    }

                    Photo photoWithPreferredTags = photo;

                    if (preferences.NeedOriginalTags)
                    {
                        photoWithPreferredTags = await _originalTagsLogic.GetOriginalTagsTask(photo);
                    }

                    string photoName = preferences.TitleAsFilename ? GetSafeFilename(photo.Title) : photo.Id;
                    string targetFileName = Path.Combine(imageDirectory.FullName,
                        string.Format("{0}.{1}", photoName, photoExtension));
                    WriteMetaDataFile(photoWithPreferredTags, targetFileName, preferences);

                    WebRequest request = WebRequest.Create(photoUrl);

                    var buffer = new byte[4096];

                    await DownloadAndSavePhoto(targetFileName, request, buffer);

                    doneCount++;
                    progressUpdate.PercentDone = doneCount*100/totalCount;
                    progressUpdate.DownloadedPath = imageDirectory.FullName;
                    progress.Report(progressUpdate);
                    if (progressUpdate.PercentDone != 100) cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private static async Task DownloadAndSavePhoto(string targetFileName, WebRequest request, byte[] buffer)
        {
            using (var target = new FileStream(targetFileName, FileMode.Create, FileAccess.Write))
            {
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        int read;
                        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await target.WriteAsync(buffer, 0, read);
                        }
                    }
                }
            }
        }

        private DirectoryInfo CreateDownloadFolder(string downloadLocation)
        {
            _currentTimestampFolder = string.Format("flickr-downloadr-{0}",
                GetSafeFilename(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
            DirectoryInfo imageDirectory =
                Directory.CreateDirectory(Path.Combine(downloadLocation, _currentTimestampFolder));
            return imageDirectory;
        }

        private static string RandomString(int size)
        {
            // http://stackoverflow.com/a/1122519/218882
            var builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*Random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        private static string GetSafeFilename(string path)
        {
            // http://stackoverflow.com/a/333297/218882
            string safeFilename = Path.GetInvalidFileNameChars()
                .Aggregate(path, (current, c) => current.Replace(c, '-'));
            return string.IsNullOrWhiteSpace(safeFilename) ? RandomString(8) : safeFilename;
        }

        private static void WriteMetaDataFile(Photo photo, string targetFileName, Preferences preferences)
        {
            Dictionary<string, string> metadata = preferences.Metadata.ToDictionary(metadatum => metadatum,
                metadatum =>
                    photo.GetType()
                        .GetProperty(metadatum)
                        .GetValue(photo, null)
                        .ToString());
            if (metadata.Count > 0)
                File.WriteAllText(string.Format("{0}.json", targetFileName), metadata.ToJson(), Encoding.Unicode);
        }
    }
}