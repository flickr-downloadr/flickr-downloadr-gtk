using System;
using System.Collections.Generic;
using System.IO;
using FloydPink.Flickr.Downloadr.Model.Enums;

namespace FloydPink.Flickr.Downloadr.Model
{
  public enum FileNameMode {
    Title,
    OriginalOrder,
    PhotoId
  }

  public class Preferences
  {
    public string DownloadLocation { get; set; }
    public string AlbumSearchName { get; set; }
    public FileNameMode FileNameMode { get; set; }
    public PhotoDownloadSize DownloadSize { get; set; }
    public int PhotosPerPage { get; set; }
    public string SafetyLevel { get; set; }
    public List<string> Metadata { get; set; }
    public bool NeedOriginalTags { get; set; }
    public string CacheLocation { get; set; }
    public bool CheckForUpdates { get; set; }
    public LogLevel LogLevel { get; set; }
    public string LogLocation { get; set; }
    public bool Visited { get; set; }

    public static Preferences GetDefault()
    {
      return new Preferences
      {
        FileNameMode = FileNameMode.PhotoId,
        PhotosPerPage = 25,
        AlbumSearchName = "",
        DownloadLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        Metadata =
          new List<string>
          {
            PhotoMetadata.Title,
            PhotoMetadata.Description,
            PhotoMetadata.Tags
          },
        DownloadSize = PhotoDownloadSize.Original,
        SafetyLevel = SafeSearch.Safe,
        NeedOriginalTags = false,
        CacheLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
          "flickr-downloadr", "Cache"),
        CheckForUpdates = true,
        LogLevel = LogLevel.Info,
        LogLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
          "flickr-downloadr", "Logs")
      };
    }
  }
}
