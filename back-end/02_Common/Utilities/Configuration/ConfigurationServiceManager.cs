using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EggKeeper.Sdk;
using Newegg.API.Logging;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.Utilities.Configuration
{
    public enum ConfigurationDataType
    {
        Xml,
        Json,
        String
    }

    public abstract class ConfigurationServiceManager<T> : IConfigurationServiceListener
        where T : class
    {
        private const string VendorPortalConfigurationSystemName = "EDI_API_Common";

        private T _configuration;

        public T Configuration
        {
            get
            {
                return _configuration ?? (_configuration = ConfigurationServiceHelper.GetConfiguration<T>(
                    ConfigurationSystem,
                    ConfigurationName,
                    ToEggKeeperDataType(DataType)));
            }
            private set { _configuration = value; }
        }

        private NodeDataType ToEggKeeperDataType(
            ConfigurationDataType configurationDataType)
        {
            switch (configurationDataType)
            {
                case ConfigurationDataType.Json:
                    return NodeDataType.Json;
                case ConfigurationDataType.Xml:
                    return NodeDataType.Xml;
                case ConfigurationDataType.String:
                    return NodeDataType.Text;
                default:
                    throw new NotSupportedException(
                        string.Format(
                            "The configuration data type [{0}] was not supported.",
                            configurationDataType));
            }
        }

        protected string GetEnvironmentSuffix()
        {
            var environmentSuffix = string.Empty;

            if (!string.IsNullOrWhiteSpace(GlobalSetting.Environment))
            {
                environmentSuffix = "_" + GlobalSetting.Environment;
            }

            return environmentSuffix;
        }

        public string EnvironmentSuffix
        {
            get { return GetEnvironmentSuffix(); }
        }

        /// <summary>
        /// Override this property to match the configuration data type stored in 
        /// configuration service. Xml is the default DataType
        /// </summary>
        public virtual ConfigurationDataType DataType
        {
            get
            {
                return ConfigurationDataType.Xml;
            }
        }

        public virtual string ConfigurationSystem
        {
            get { return VendorPortalConfigurationSystemName; }
        }

        public virtual string ConfigurationName
        {
            get { return typeof(T).Name; }
        }

        public event EventHandler ConfigurationModified;

        private void RefreshConfiguration()
        {
            try
            {
                Configuration =
                    ConfigurationServiceHelper.GetConfiguration<T>(
                        ConfigurationSystem,
                        ConfigurationName,
                        ToEggKeeperDataType(DataType));
            }
            catch (Exception ex)
            {
                LogFactory.Log.Error(
                    string.Format("Get Configuration failed, System:{0},ConfigurationName:{1}. Error Message:{2} \r\n Error Detail:{3}"
                        , ConfigurationSystem
                        , ConfigurationName
                        , ex.Message
                        , ex.StackTrace));
            }
        }

        public void ConfigurationModify()
        {
            RefreshConfiguration();
            OnConfigurationModified();
        }

        protected virtual void OnConfigurationModified()
        {
            var handler = ConfigurationModified;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void Setup()
        {
            ConfigurationServiceHelper.AddConfigurationListener(this);
        }
    }
}
