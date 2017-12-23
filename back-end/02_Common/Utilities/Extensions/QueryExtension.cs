using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.Utilities.Extensions
{
    public static class QueryExtension
    {
        public static bool HasMoreRecords<TEntity>(this TEntity entity)
            where TEntity : IPagingAndSort, IQuerySummary
        {
            if (!entity.TotalPageCount.HasValue) return false;
            if (null == entity.PagingInfo) return false;

            // steven use end page index to check if more records available
            if (!entity.PagingInfo.EndPageIndex.HasValue) return false;
            return entity.PagingInfo.EndPageIndex.Value < (entity.TotalPageCount.Value - 1);
        }
    }
}
