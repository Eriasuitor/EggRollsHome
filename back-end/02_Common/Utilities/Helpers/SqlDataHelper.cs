using System;
using System.Collections.Generic;
using System.Linq;

namespace Newegg.MIS.API.Utilities.Helpers
{
    public static class SqlDataHelper
    {
        public static string GetSqlSortFields(
            string sortFields,
            string sortType,
            Dictionary<string, string> sqlFieldsMappingTable)
        {
            MakeSureSortFieldsIsValid(sortFields);
            MakeSureSortTypeIsValid(sortType);

            var sortFieldsList = new List<string>();

            foreach (var sortField in sortFields.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var lowerFieldName = sortField.ToLower();
                if (!sqlFieldsMappingTable.ContainsKey(lowerFieldName))
                {
                    throw new Exception("Sort field " + sortField + " is not supported.");
                }
                sortFieldsList.Add(
                    string.Format("{0} {1}",
                        sqlFieldsMappingTable[lowerFieldName],
                        sortType));

            }

            return string.Join(",", sortFieldsList);
        }

        public static string GetSqlSortFields(string sortFields, string sortType)
        {
            MakeSureSortFieldsIsValid(sortFields);
            MakeSureSortTypeIsValid(sortType);

            var sortFieldsList = sortFields.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(sortField => sortField.ToLower())
                .Select(lowerFieldName => string.Format("{0} {1}", lowerFieldName, sortType)).ToList();

            return string.Join(",", sortFieldsList);
        }

        public static void MakeSureSortFieldsIsValid(string sortFields)
        {
            if (string.IsNullOrWhiteSpace(sortFields))
            {
                throw new Exception("Sort fields cannot be empty, at least one one field must be provided.");
            }
        }

        public static void MakeSureSortTypeIsValid(string sortType)
        {
            if (string.IsNullOrWhiteSpace(sortType))
            {
                throw new Exception("Sort type cannot be empty, it must be ASC or DESC.");
            }

            if (string.Compare(SortFieldAsc, sortType, StringComparison.InvariantCultureIgnoreCase) != 0 &&
                string.Compare(SortFieldDesc, sortType, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new Exception("Sort type [" + sortType + "] is invalid, it must be ASC or DESC.");
            }
        }

        private const string SortFieldAsc = "ASC";
        private const string SortFieldDesc = "DESC";

    }
}
