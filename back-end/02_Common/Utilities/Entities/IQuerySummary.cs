using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newegg.MIS.API.Utilities.Entities
{
    public interface IQuerySummary
    {
        int? TotalRecordCount { get; set; }

        int? TotalPageCount { get; set; }
    }
}
