using System;
using Newegg.API.Interfaces;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.Extensions;

namespace Newegg.MIS.API.EggRolls.Services
{
    public class QuestionnaireParticipatorService:RestServiceBase<QuestionnaireParticipatorRequest>
    {
        public override object OnGet(QuestionnaireParticipatorRequest request)
        {
            var resp = new QuestionnaireParticipatorResponse();
            try
            {
                resp.Participators = QuestionnaireBusiness.Instance.QueryParticipator(request.QuestionnaireID);
            }
            catch(Exception ex)
            {
                resp.CaptureException(ex);
            }
            return resp;
        }
    }
}
