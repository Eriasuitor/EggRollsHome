namespace Newegg.MIS.API.Utilities.Configuration
{
    public class DFISConfiguration
    {
        public string UploadLink { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string DownloadLink { get; set; }
        public int? UploadTimeoutInMilliSecond { get; set; }
    }
}
