
using Newegg.API.Interfaces;
using Newegg.MIS.API.Utilities.Extensions;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using System;

namespace Newegg.MIS.API.EggRolls.Services
{
    public class QuesItemDeleteByQidService : RestServiceBase<QuesItemDeleteByQidRequest>
    {
        public override object OnDelete(QuesItemDeleteByQidRequest request)
        {
            var response = new QuesItemDeleteByQidResponse();

            try
            {
                QuesItemDeleteByQidBusiness.DeleteQuestionnaire(request,response);
            }
            catch (Exception ex)
            {
                this.RequestContext.HttpRes.StatusCode = 405;
                this.RequestContext.HttpRes.StatusDescription = "what the fuck";
                response.CaptureException(ex);
                //response.ShouldSerializeSucceeded();
                //response.ShouldSerializeStackTrace();
                //response.CaptureValidationResult();
            }
            return response;
        }
    }
}
