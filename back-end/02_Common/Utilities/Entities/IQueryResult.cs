namespace Newegg.MIS.API.Utilities.Entities
{
	public interface IQueryResult
	{
		int? TotalRecordCount { get; set; }
		int? TotalPageCount { get; set; }
	}
}
