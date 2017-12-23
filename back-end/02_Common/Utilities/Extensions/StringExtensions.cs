using System.Collections.Generic;
using System.Text;

namespace Newegg.MIS.API.Utilities.Extensions
{
	public static class StringExtensions
	{
		public static string Capitalize(this string value)
		{
			if (string.IsNullOrWhiteSpace(value)) return value;

			if (!char.IsLetter(value[0])) return value;

			var captialized = new string(value[0], 1).ToUpper();

			if (value.Length > 1)
			{
				captialized += value.Substring(1);
			}

			return captialized;
		}

		public static string ToSqlWhereStatement(this List<string> conditions)
		{
			var conditionBuilder = new StringBuilder();

			conditions.RemoveAll(string.IsNullOrWhiteSpace);

			if (conditions.Count <= 0) return conditionBuilder.ToString();

			conditionBuilder.Append(@" Where ");
			for (var i = 0; i < conditions.Count; i++)
			{
				if (i != 0) conditionBuilder.Append(" AND ");

				conditionBuilder.Append(conditions[i]);
			}

			return conditionBuilder.ToString();
		}
	}
}
