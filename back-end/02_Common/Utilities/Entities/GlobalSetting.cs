using Newegg.MIS.API.Utilities.Helpers;

namespace Newegg.MIS.API.Utilities.Entities
{
    public static class GlobalSetting
    {
        public const string EnvironmentSettingName = "Environment";
        public const string MessageVersionSettingName = "MessageVersion";

        public static string Environment
        {
            get
            {
                return ConfigurationHelper.GetConfiguration(
                    EnvironmentSettingName,
                    string.Empty);
            }
        }

        public static string MessageVersion
        {
            get
            {
                return ConfigurationHelper.GetConfiguration(
                    MessageVersionSettingName,
                    string.Empty);
            }
        }
    }
}
