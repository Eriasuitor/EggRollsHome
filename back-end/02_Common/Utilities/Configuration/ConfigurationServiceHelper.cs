using System;
using System.Collections.Generic;
using EggKeeper.Sdk;
using Newegg.API.Logging;
using Newegg.MIS.API.Utilities.Helpers;
using Newtonsoft.Json;

namespace Newegg.MIS.API.Utilities.Configuration
{
    public sealed class ConfigurationServiceHelper
    {
        private static readonly EggKeeperClient EggKeeper =
            EggKeeperFactory.GetEggKeeper();

        private static readonly List<IConfigurationServiceListener>
            ConfigurationListenerList = new List<IConfigurationServiceListener>();

        public static void AddConfigurationListener(
            IConfigurationServiceListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException("listener");
            }

            if (string.IsNullOrWhiteSpace(listener.ConfigurationSystem))
            {
                throw new ArgumentException(
                    "listener.ConfigurationSystem is null or empty.",
                    "listener");
            }

            if (string.IsNullOrWhiteSpace(listener.ConfigurationName))
            {
                throw new ArgumentException(
                    "listener.ConfigurationName is null or empty.",
                    "listener");
            }

            if (ConfigurationListenerList.Contains(listener)) return;

            EggKeeper.Watch(
                listener.ConfigurationSystem,
                listener.ConfigurationName,
                ConfigurationMonitor);

            ConfigurationListenerList.Add(listener);
        }

        public static void RemoveConfigurationListener(
            IConfigurationServiceListener listener)
        {
            if (listener == null)
            {
                throw new ArgumentNullException("listener");
            }

            if (ConfigurationListenerList.Contains(listener))
            {
                ConfigurationListenerList.Remove(listener);
            }
        }

        public static string GetConfigurationString(
            string configurationSystem,
            string configurationName)
        {
            return GetConfiguration<string>(
                configurationSystem,
                configurationName,
                NodeDataType.Text);
        }

        public static T GetConfiguration<T>(
            string configurationSystem,
            string configurationName,
            NodeDataType dataType)
        {
            if (string.IsNullOrWhiteSpace(configurationSystem))
            {
                throw new ArgumentException(
                    "configurationSystem is null or empty.",
                    "configurationSystem");
            }
            if (string.IsNullOrWhiteSpace(configurationName))
            {
                throw new ArgumentException(
                    "configurationName is null or empty.",
                    "configurationName");
            }

            try
            {
                var dataString =
                    EggKeeper.GetData<string>(
                    configurationSystem,
                    configurationName);

                if (typeof(T) == typeof(string)) return (T)(object)dataString;

                if (dataType == NodeDataType.Xml)
                {
                    return SerializationHelper.Deserialize<T>(dataString);
                }

                if (dataType == NodeDataType.Json)
                {
                    return JsonConvert.DeserializeObject<T>(dataString);
                }

                throw new NotSupportedException(
                    string.Format(
                        "Unable to convert configuration data to type {0}, " +
                        "the original data string is {1}{2}",
                        typeof(T).FullName,
                        Environment.NewLine,
                        dataString));
            }
            catch (Exception ex)
            {
                var message =
                    string.Format(
                        "Failed to get configuration [{0}] for system [{1}], due to {2}. " +
                        "Please check the inner excpetion for detailed reason.",
                        configurationName,
                        configurationSystem,
                        ex.Message);

                throw new Exception(message, ex);
            }
        }

        private static void ConfigurationMonitor(DataWatchContext context)
        {
            LogFactory.Log.Info(
                string.Format("System:{0},ConfigurationName:{1} is changed."
                              , context.SystemName
                              , context.ConfigName));

            ConfigurationListenerList.ForEach(listener =>
            {
                if (string.Equals(context.SystemName, listener.ConfigurationSystem) &&
                    string.Equals(context.ConfigName, listener.ConfigurationName))
                {
                    listener.ConfigurationModify();
                }
            });

            LogFactory.Log.Info(
                string.Format("System:{0},ConfigurationName:{1} also notify all listners."
                , context.SystemName
                , context.ConfigName));
        }
    }
}
