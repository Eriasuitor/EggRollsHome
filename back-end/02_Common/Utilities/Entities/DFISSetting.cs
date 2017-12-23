namespace Newegg.MIS.API.Utilities.Entities
{
    public class DFISSetting
    {
        public string UploadLink { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string DownloadLink { get; set; }
        public int? UploadTimeoutInMilliSecond { get; set; }
    }
}
