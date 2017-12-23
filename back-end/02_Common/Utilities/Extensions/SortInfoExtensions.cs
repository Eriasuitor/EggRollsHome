using System.Collections.Generic;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.Utilities.Extensions
{
	public static class SortInfoExtensions
	{
		private const string SortTypeAsc = "ASC";
		private const string SortTypeDesc = "DESC";
		private const string DefaultSortType = SortTypeAsc;

		public static string BuildSortFiled(this SortInfo sortInfo, string defaultSortField,
			Dictionary<string, string> sortFiledMapper)
		{
			return string.Format(" {0} {1} ", GetSortField(sortInfo, defaultSortField, sortFiledMapper), GetSortType(sortInfo));
		}

		private static string GetSortField(SortInfo sortInfo, string defaultSortField,
			IDictionary<string, string> sortFiledMapper)
		{
			if (null == sortInfo) return defaultSortField;

			if (string.IsNullOrWhiteSpace(sortInfo.SortField)) return defaultSortField;

			var normalizedSortField = sortInfo.SortField.Trim().ToLower();

			string sortField;

			return sortFiledMapper.TryGetValue(normalizedSortField, out sortField) ? sortField : defaultSortField;
		}

		private static string GetSortType(SortInfo sortInfo)
		{
			if (null == sortInfo) return DefaultSortType;

			return sortInfo.SortType == SortType.ASC ? SortTypeAsc : SortTypeDesc;
		}
	}
}
