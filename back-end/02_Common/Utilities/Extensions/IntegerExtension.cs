using System;

namespace Newegg.MIS.API.Utilities.Extensions
{
    public static class IntegerExtension
    {
        public static int? ToInteger(this object value)
        {
            if (null == value) return null;
            if (DBNull.Value == value) return null;

            int integerValue;

            if (int.TryParse(value.ToString(), out integerValue))
            {
                return integerValue;
            }

            return null;
        }
    }
}
