namespace Newegg.MIS.API.Utilities.Entities
{
    public class PagingInfo
    {
        public int? PageSize { get; set; }
        public int? StartPageIndex { get; set; }
        public int? EndPageIndex { get; set; }
		public bool IsPaging { get; set; }

	    public PagingInfo()
	    {
		    IsPaging = true;
	    }
        
        public bool ShouldSerializePageSize()
        {
            return PageSize.HasValue;
        }

        public bool ShouldSerializeStartPageIndex()
        {
            return StartPageIndex.HasValue;
        }

        public bool ShouldSerializeEndPageIndex()
        {
            return EndPageIndex.HasValue;
        }
    }
}
