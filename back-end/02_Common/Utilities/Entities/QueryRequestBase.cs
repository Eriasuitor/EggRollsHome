using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Newegg.MIS.API.Utilities.Entities
{
    public abstract class QueryRequestBase
    {
        public new string ToString()
        {
            return BuildContent();
        }

        [ThreadStatic]
        public static string RequestID;

        private bool _isRetry;

        public void TagQueryAsRetry()
        {
            _isRetry = true;
        }

        public bool IsRetry()
        {
            return _isRetry;
        }

        protected string BuildContent()
        {
            var properties = GetType().GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);

            var parameters = new List<string>();

            foreach (var propertyInfo in properties)
            {
                AppendQueryParameter(parameters, propertyInfo, this);
            }

            if (parameters.Count == 0) return base.ToString();

            return string.Join(" ", parameters);
        }

        private static void AppendQueryParameter(
            ICollection<string> parameterCollection,
            PropertyInfo propertyInfo,
            QueryRequestBase request)
        {
            var value =
                propertyInfo.GetValue(request,
                                      BindingFlags.Instance |
                                      BindingFlags.Public,
                                      null,
                                      null,
                                      CultureInfo.CurrentCulture);

            if (null == value) return;

            if (propertyInfo.PropertyType == typeof(string))
            {
                value = Uri.EscapeDataString(value.ToString());
            }

            parameterCollection.Add(propertyInfo.Name + ":" + value);
        }
    }
}
