using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Newegg.MIS.API.Utilities.Entities
{
    public class RequestInfo
    {
        /// <summary>
        /// Required,  e,g :VendorPortal, SellerPortal, Biztalk
        /// </summary>
        public string SystemSource { get; set; }
        /// <summary>
        /// Required except query.
        /// </summary>
        public Guid? RequestGuid { get; set; }
        /// <summary>
        /// Optional, default:en-US
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// Required, e,g: tw14, jack, Auto
        /// </summary>
        public string UserID { get; set;}

        /// <summary>
        /// Required except query.
        /// </summary>
        public DateTime? RequestTimeUtc { get; set; }

        /// <summary>
        /// Optional,  the system you want to process this message
        /// </summary>
        public string SystemDestination { get; set; }
        /// <summary>
        /// Optional
        /// </summary>
        public string CallbackAddress { get; set; }
        /// <summary>
        /// Optional, e.g:  VF
        /// </summary>
        public string GlobalBusinessType { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// Optional
        /// </summary>
        public string ClientTag { get; set; }
        //  public Dictionary<string, string> AdditionalInfo { get; set; }

        public bool? AsyncResponse { get; set; }

        public bool ShouldSerializeAsyncResponse()
        {
            return AsyncResponse.HasValue;
        }

#if DEBUG
        public bool? TestOverride { get; set; }
        public bool ShouldSerializeTestOverride()
        {
            return TestOverride.HasValue;
        }
#endif
    }

    public interface IRequest
    {
        RequestInfo RequestInfo { get; set; }
    }
}
