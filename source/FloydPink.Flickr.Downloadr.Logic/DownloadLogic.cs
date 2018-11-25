using System;
using System.Collections.Generic;
using System.Globalization;
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

    public async Task Download(IEnumerable<Photo> photos, CancellationToken cancellationToken, IProgress<ProgressUpdate> progress,
      Preferences preferences, Photoset photoset, string folderPrefix = null, string albumProgress = null)
    {
      await DownloadPhotos(photos, cancellationToken, progress, preferences, photoset, folderPrefix, albumProgress);
    }

    private async Task DownloadPhotos(IEnumerable<Photo> photos, CancellationToken cancellationToken, IProgress<ProgressUpdate> progress,
                                      Preferences preferences, Photoset photoset, string folderPrefix = null, string albumProgress = null)
    {
      var progressUpdate = new ProgressUpdate
      {
        Cancellable = true,
        OperationText = string.IsNullOrEmpty(albumProgress) ? "Downloading photos..." : "Downloading albums...",
        PercentDone = 0,
        ShowPercent = true,
        AlbumProgress = albumProgress
      };
      progress.Report(progressUpdate);

      var doneCount = 0;
      var photosList = photos as IList<Photo> ?? photos.ToList();
      var totalCount = photosList.Count();

      var imageDirectory = CreateDownloadFolder(preferences.DownloadLocation, photoset, folderPrefix);

      var curCount = 0;
      foreach (var photo in photosList)
      {
        curCount++;
        var photoUrl = photo.OriginalUrl;
        var photoExtension = "jpg";
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

        var photoWithPreferredTags = photo;

        if (preferences.NeedOriginalTags)
        {
          photoWithPreferredTags = await _originalTagsLogic.GetOriginalTagsTask(photo, preferences);
        }

        var photoName = preferences.FileNameMode == FileNameMode.Title ? GetSafeFilename(photo.Title) : photo.Id;

        if (preferences.FileNameMode == FileNameMode.OriginalOrder)
        {
          photoName = GetPadded(curCount);
        }

        var targetFileName = Path.Combine(imageDirectory.FullName,
          string.Format("{0}.{1}", photoName, photoExtension));

        if (File.Exists(targetFileName)) {
          targetFileName = Path.Combine(imageDirectory.FullName,
          string.Format("{0}-{2}.{1}", photoName, photoExtension, GetPadded(curCount)));
        }

        WriteMetaDataFile(photoWithPreferredTags, targetFileName, preferences);

        var request = WebRequest.Create(photoUrl);

        var buffer = new byte[4096];

        await DownloadAndSavePhoto(targetFileName, request, buffer);

        doneCount++;
        progressUpdate.PercentDone = doneCount*100/totalCount;
        progressUpdate.DownloadedPath = string.IsNullOrWhiteSpace(folderPrefix) ? imageDirectory.FullName : _currentTimestampFolder;
        progress.Report(progressUpdate);
        if (doneCount != totalCount)
        {
          cancellationToken.ThrowIfCancellationRequested();
        }
      }
    }

    private static string GetPadded(int curCount)
    {
      return curCount.ToString().PadLeft(6, '0');
    }

    private static async Task DownloadAndSavePhoto(string targetFileName, WebRequest request, byte[] buffer)
    {
      using (var target = new FileStream(targetFileName, FileMode.Create, FileAccess.Write))
      {
        using (var response = await request.GetResponseAsync())
        {
          using (var stream = response.GetResponseStream())
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

    private DirectoryInfo CreateDownloadFolder(string downloadLocation, Photoset currentPhotoset, string folderPrefix)
    {
      if (string.IsNullOrWhiteSpace(folderPrefix)) {
        _currentTimestampFolder = string.Format("flickr-downloadr{0}-{1}", GetDownloadFolderNameForPhotoset(currentPhotoset),
          GetSafeFilename(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));

        var imageDirectory = Directory.CreateDirectory(Path.Combine(downloadLocation, _currentTimestampFolder));

        return imageDirectory;
      }
      else {
        var albumRootDirectory = Directory.CreateDirectory(Path.Combine(downloadLocation, Path.Combine(downloadLocation, folderPrefix)));

        _currentTimestampFolder = albumRootDirectory.FullName;

        if (currentPhotoset.Type == PhotosetType.Album)
        {
          return Directory.CreateDirectory(Path.Combine(_currentTimestampFolder, GetSafeFilename(currentPhotoset.Title)));
        }

        return albumRootDirectory;
      }
    }

    private string GetDownloadFolderNameForPhotoset(Photoset photoset)
    {
      return photoset.Type == PhotosetType.Album ? string.Format("-[{0}]", GetSafeFilename(photoset.Title)) : string.Empty;
    }

    private static string RandomString(int size)
    {
      // http://stackoverflow.com/a/1122519/218882
      var builder = new StringBuilder();
      for (var i = 0; i < size; i++)
      {
        var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*Random.NextDouble() + 65), CultureInfo.InvariantCulture));
        builder.Append(ch);
      }

      return builder.ToString();
    }

    private static string GetSafeFilename(string path)
    {
      // http://stackoverflow.com/a/333297/218882
      var safeFilename = Path.GetInvalidFileNameChars()
        .Aggregate(path, (current, c) => current.Replace(c, '-'));
      return string.IsNullOrWhiteSpace(safeFilename) ? RandomString(8) : safeFilename;
    }

    private static void WriteMetaDataFile(Photo photo, string targetFileName, Preferences preferences)
    {
      var metadata = new Dictionary<string, object>();
      var metadataPreferences = preferences.Metadata;
      if (preferences.NeedLocationMetadata && !metadataPreferences.Contains(PhotoMetadata.Location))
      {
        metadataPreferences.Add(PhotoMetadata.Location);
      }
      foreach (var metadatum in metadataPreferences)
      {
        var metadataValue = photo.GetType().GetProperty(metadatum)?.GetValue(photo, null);
        if (metadataValue != null)
        {
          metadata.Add(metadatum, metadataValue);
        }
      }

      if (metadata.Count > 0)
      {
        File.WriteAllText(string.Format("{0}.json", targetFileName), metadata.ToJson(), Encoding.UTF8);
      }
    }
  }
}
