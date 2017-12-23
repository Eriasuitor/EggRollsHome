using System;

namespace Newegg.MIS.API.Utilities.DataAccess
{
    public static class DataCommandFactory
    {
        private static IDataCommand defaultCommand = null;

        public static void SetDefaultCommand(IDataCommand cmd)
        {
            defaultCommand = cmd;
        }

        public static void Reset()
        {
            defaultCommand = null;
        }

        public static IDataCommand Get(string commandName)
        {
            return defaultCommand ?? new DataCommandWrapper(commandName);
        }

        public static object ToDBValue(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return DBNull.Value;

            return value.Trim();
        }

        public static object ToDBValue<TValue>(this TValue? value)
            where TValue : struct
        {
            if (!value.HasValue) return DBNull.Value;
            return value.Value;
        }
    }
}