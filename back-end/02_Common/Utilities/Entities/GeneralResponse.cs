using System.Collections.Generic;
using Newegg.API.Attributes;

namespace Newegg.MIS.API.Utilities.Entities
{
	public class GeneralResponse : IResponse, IQueryResult
    {
        protected GeneralResponse()
        {
            Succeeded = true;
        }

        public bool? Succeeded { get; set; }
        public bool ShouldSerializeSucceeded()
        {
            return Succeeded.HasValue;
        }
        public List<Error> Errors { get; set; }

        public List<ValidationErrorResponse> ValidationErrors { get; set; }

		public int? TotalRecordCount { get; set; }

		public int? TotalPageCount { get; set; }

		public bool ShouldSerializeTotalRecordCount()
		{
			return TotalRecordCount.HasValue;
		}

		public bool ShouldSerializeTotalPageCount()
		{
			return TotalPageCount.HasValue;
		}

#if !DEBUG
        [IgnoreDataMember]
#endif
		public string StackTrace { get; set; }

		public bool ShouldSerializeStackTrace()
		{
#if DEBUG
			return true;
#else
            return false;
#endif
		}
    }
}
