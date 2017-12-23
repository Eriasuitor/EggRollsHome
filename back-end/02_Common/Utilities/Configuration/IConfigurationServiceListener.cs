using System;

namespace Newegg.MIS.API.Utilities.Configuration
{
    public interface IConfigurationServiceListener
    {
        string ConfigurationSystem { get; }
        string ConfigurationName { get; }
        void ConfigurationModify();
        event EventHandler ConfigurationModified;
    }
}
