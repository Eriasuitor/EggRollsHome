using System.Runtime.Serialization;

namespace Newegg.MIS.API.Utilities.Entities
{
    public enum SortType
    {
        // default sorting is ASC
        [EnumMember]
        ASC = 0,

        [EnumMember]
        DESC = 1
    }

    public class SortInfo
    {
        public string SortField { get; set; }

        public SortType SortType { get; set; }

        public string GetSortTypeString()
        {
            return SortType == SortType.DESC ? "desc" : "asc";
        }
    }
}
