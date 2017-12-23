using System;

namespace Newegg.MIS.API.Utilities.Configuration
{
    public class DFISConfigurationManager : ConfigurationServiceManager<DFISConfiguration>
    {
        private static readonly Lazy<DFISConfigurationManager> LazyInstance =
            new Lazy<DFISConfigurationManager>(() =>
            {
                var instance = new DFISConfigurationManager();
                instance.Setup();
                return instance;
            }, true);

        private DFISConfigurationManager()
        {
        }

        public override ConfigurationDataType DataType
        {
            get
            {
                return ConfigurationDataType.Json;
            }
        }

        public override string ConfigurationName
        {
            get { return typeof(DFISConfiguration).Name + GetEnvironmentSuffix(); }
        }

        public static DFISConfigurationManager Instance
        {
            get { return LazyInstance.Value; }
        }
    }
}
