namespace FloydPink.Flickr.Downloadr.Model {
    public class ProgressUpdate {
        public bool ShowPercent { get; set; }
        public int PercentDone { get; set; }
        public string OperationText { get; set; }
        public bool Cancellable { get; set; }
        public string DownloadedPath { get; set; }
    }
}
