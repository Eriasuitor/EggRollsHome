using System;
using System.IO;
using Newegg.DFIS.Uploader;
using Newegg.MIS.API.Utilities.Configuration;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.Utilities.Helpers
{
    public enum DFISUploadType
    {
        Add,
        Update
    }

    public static class DFISUploadHelper
    {
        public static DFISSetting GetDFISSetting()
        {
            // Domain/System/Project Name: EDI/VF/VendorPortal
            var dfisUpload = DFISConfigurationManager.Instance.Configuration.UploadLink;
            var dfisGroup = DFISConfigurationManager.Instance.Configuration.Group;
            var dfisDowload = DFISConfigurationManager.Instance.Configuration.DownloadLink;
            var dfisType = DFISConfigurationManager.Instance.Configuration.Type;

            var setting = new DFISSetting
            {
                DownloadLink = dfisDowload,
                UploadLink = dfisUpload,
                Group = dfisGroup,
                Type = dfisType,
                UploadTimeoutInMilliSecond =
                    DFISConfigurationManager.Instance.Configuration.UploadTimeoutInMilliSecond
            };

            return setting;
        }

        public static DFISSetting GetDFISSetting(string fileGroup, string fileType)
        {
            var setting = GetDFISSetting();

            if (!string.IsNullOrWhiteSpace(fileGroup))
            {
                setting.Group = fileGroup.Trim();
            }

            if (!string.IsNullOrWhiteSpace(fileType))
            {
                setting.Type = fileType.Trim();
            }

            return setting;
        }

        public static string Upload(string localFilePath)
        {
            var setting = GetDFISSetting();

            return Upload(setting, localFilePath);
        }

        public static string Upload(DFISSetting dfisSetting, string localFilePath)
        {
            return Upload(dfisSetting, localFilePath, DFISUploadType.Add);
        }

        public static string Upload(
            DFISSetting dfisSetting,
            string localFilePath,
            DFISUploadType uploadType)
        {
            if (string.IsNullOrWhiteSpace(localFilePath))
            {
                throw new ArgumentException("Local file path must be specified.");
            }

            try
            {
                HttpUploader.UploadFile(
                    dfisSetting.UploadLink,
                    dfisSetting.Group,
                    dfisSetting.Type,
                    localFilePath,
                    null,
                    ToUploadMethod(uploadType));
            }
            finally
            {
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }
            }

            return BuildDFISDownloadURL(dfisSetting, Path.GetFileName(localFilePath));
        }


        public static string Upload(
            DFISSetting dfisSetting,
            string filename,
            Stream fileStream,
            DFISUploadType uploadType)
        {
            try
            {
                HttpUploader.UploadFile(
                    fileStream,
                    filename,
                    dfisSetting.UploadLink,
                    dfisSetting.Group,
                    dfisSetting.Type,
                    null,
                    "EDI",
                    ToUploadMethod(uploadType),
                    GetTimeout(dfisSetting));
            }
            finally
            {
                if (fileStream.CanSeek)
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                }
            }

            return BuildDFISDownloadURL(dfisSetting, filename);
        }

        private static UploadMethod ToUploadMethod(DFISUploadType uploadType)
        {
            if (uploadType == DFISUploadType.Update) return UploadMethod.Update;

            return UploadMethod.Add;
        }

        public static string Upload(DFISSetting dfisSetting, string filename, Stream fileStream)
        {
            return Upload(dfisSetting, filename, fileStream, DFISUploadType.Add);
        }

        public const int Second = 1000; // 1000 ms

        public const int DefaultTimeout = 600 * Second;

        public const int MinimumTimeout = 30 * Second;

        private static int GetTimeout(DFISSetting dfisSetting)
        {
            if (!dfisSetting.UploadTimeoutInMilliSecond.HasValue)
            {
                return DefaultTimeout;
            }

            if (dfisSetting.UploadTimeoutInMilliSecond.Value <
                MinimumTimeout)
            {
                return MinimumTimeout;
            }

            return dfisSetting.UploadTimeoutInMilliSecond.Value;
        }

        private static string BuildDFISDownloadURL(DFISSetting dfisSetting, string filename)
        {
            return dfisSetting.DownloadLink + "/" +
                dfisSetting.Group + "/" +
                dfisSetting.Type + "/" +
                Uri.EscapeDataString(filename);
        }
    }
}
