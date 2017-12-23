using System;
using Newegg.API.Interfaces;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.Extensions;

namespace Newegg.MIS.API.EggRolls.Services
{
    public class QuestionnaireSearchService: RestServiceBase<QuestionnaireSearchRequest>
    {
        public override object OnGet(QuestionnaireSearchRequest request)
        {
            var resp = new QuestionnaireSearchResponse();

            try
            {
                return QuestionnaireBusiness.Instance.Search(request);
            }
            catch (Exception ex)
            {
                resp.CaptureException(ex);
            }

            return resp;
        }
    }
}
