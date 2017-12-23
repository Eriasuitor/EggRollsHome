using System;
using Newegg.API.Interfaces;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.Extensions;

namespace Newegg.MIS.API.EggRolls.Services
{
    public class AnswerParticipatorService:RestServiceBase<AnswerParticipatorRequest>
    {
        public override Object OnGet(AnswerParticipatorRequest request)
        {
            var resp = new AnswerParticipatorResponse();
            try
            {
                resp.Answers = AnswerSheetBusiness.Instance.Query(request.QuestionnaireID, request.TopicID,request.OptionID);
            }
            catch(Exception ex)
            {
                resp.CaptureException(ex);
            }
            return resp;
        }
    }
}
