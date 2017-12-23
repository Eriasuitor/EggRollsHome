namespace Newegg.MIS.API.Utilities.Entities
{
    public interface IPagingAndSort
    {
        PagingInfo PagingInfo { get; set; }

        SortInfo SortInfo { get; set; }
    }
}
