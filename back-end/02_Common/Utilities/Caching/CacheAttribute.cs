using System;

namespace Newegg.MIS.API.Utilities.Caching
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CacheAttribute : Attribute
    {
        public string Slot { get; set; }
    }
}
