using System;
using System.Configuration;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.Utilities.Helpers
{
    public static class ConfigurationHelper
    {
        public static string GetConfiguration(string cfg, string defaultValue)
        {
            var value = ConfigurationManager.AppSettings[cfg];
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            var controlCfg = cfg + "Encrypted";

            var isCfgEncrypted = GetBoolean(controlCfg, false);

            if (isCfgEncrypted)
            {
                return EncryptionHelper.Decrypt(value.Trim());
            }

            return value.Trim();
        }

        public static string GetConfiguration(string cfg)
        {
            return GetConfiguration(cfg, string.Empty);
        }

        public static string GetConfigurationReqired(string cfg)
        {
            var value = GetConfiguration(cfg, string.Empty);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationException(
                    string.Format(
                        "The value of configuration [{0}] was empty, but it's required.",
                        cfg));
            }

            return value;
        }

        public static TEnum GetEnumValue<TEnum>(string cfg) 
            where TEnum : struct
        {
            return GetEnumValue<TEnum>(cfg, null);
        }

        public static TEnum GetEnumValue<TEnum>(string cfg, TEnum? defaultValue)
            where TEnum : struct
        {
            if (!typeof (TEnum).IsEnum)
            {
                throw new ArgumentException(
                    "An enum type paramter must be specified for TEnum.");
            }

            var value = GetConfiguration(cfg);

            if (string.IsNullOrWhiteSpace(value))
            {
                if (defaultValue.HasValue) return defaultValue.Value;

                throw new ApplicationException(
                    string.Format("Unable to get a {0} value from configuration [{1}].",
                                  typeof (TEnum).Name,
                                  cfg));
            }

            TEnum convertedValue;
            if (Enum.TryParse(value, true, out convertedValue))
            {
                return convertedValue;
            }

            throw new InvalidCastException(
                string.Format("Unable to convert value {0} of configuration [{1}] to a {2} type.",
                value,
                cfg,
                typeof(TEnum).Name));
        }

        public static int GetInteger(string cfg)
        {
            var value = GetConfigurationReqired(cfg);

            return int.Parse(value);
        }

        public static int GetInteger(string cfg, int defaultValue)
        {
            var value = GetConfiguration(cfg);
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            int integerValue;
            if (int.TryParse(value, out integerValue))
            {
                return integerValue;
            }

            return defaultValue;
        }

        public static bool GetBoolean(string cfg)
        {
            return GetBoolean(cfg, false);
        }

        public static bool GetBoolean(string cfg, bool defaultValue)
        {
            var value = GetConfiguration(cfg);
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            bool booleanValue;
            if (bool.TryParse(value, out booleanValue))
            {
                return booleanValue;
            }

            return defaultValue;
        }

        public static TSetting GetSetting<TSetting>(string settingPrefix)
          where TSetting : class , new()
        {
            var settingType = typeof(TSetting);

            var properties =
                settingType.GetProperties();

            var settingInstance = new TSetting();

            foreach (var propertyInfo in properties)
            {
                propertyInfo.SetValue(settingInstance,
                    GetConfigurationValue(
                    settingPrefix + propertyInfo.Name,
                    propertyInfo.PropertyType),
                    null);
            }

            return settingInstance;
        }

        public static TSetting GetSetting<TSetting>()
            where TSetting : class , new()
        {
            return GetSetting<TSetting>(typeof(TSetting).Name + ".");
        }

        private static object GetConfigurationValue(string configurationKey, Type targetType)
        {
            var value = GetConfiguration(configurationKey, string.Empty);

            if (string.IsNullOrWhiteSpace(value)) return GetDefaultValue(targetType);

            return Convert.ChangeType(value, targetType);
        }

        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType) return Activator.CreateInstance(t);
            return null;
        }

        public static ApiSetting GetApiSetting(
    string apiName)
        {
            return GetApiSetting(apiName, false);
        }

        public static ApiSetting GetApiSetting(
            string apiName,
            bool throwExceptionOnNoAuthorization)
        {
            if (string.IsNullOrWhiteSpace(apiName))
            {
                throw new ArgumentException("The argument apiName must not be empty.");
            }
            var normalizedApiName = apiName.Trim();
            var apiUrlCfgName = normalizedApiName + "URL";
            var apiAuthorizationCfgName = normalizedApiName + "Authorization";

            var apiInfo = new ApiSetting
            {
                Name = apiName,
                Url = GetConfiguration(apiUrlCfgName),
                Authorization = GetConfiguration(apiAuthorizationCfgName)
            };

            if (string.IsNullOrEmpty(apiInfo.Url))
            {
                throw new ApplicationException("Unable to get API url for " +
                    apiUrlCfgName +
                    ". please make sure it's get configured in Web.config properly.");
            }

            if (string.IsNullOrWhiteSpace(apiInfo.Authorization) &&
                throwExceptionOnNoAuthorization)
            {
                throw new ApplicationException("Unable to get API authorization setting for " +
                    apiAuthorizationCfgName +
                    ". please make sure it's get configured in Web.config properly.");
            }

            return apiInfo;
        }
    }
}
