using System;
using Newegg.API.Interfaces;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.Extensions;

namespace Newegg.MIS.API.EggRolls.Services
{
    public class AnswerSheetStatisticsService: RestServiceBase<AnswerSheetStatisticsRequest>
    {
        public override Object OnGet(AnswerSheetStatisticsRequest request)
        {
            var resp = new AnswerSheetStatisticsResponse();
            try
            {
                resp.ParticipatorStatisticsList = AnswerSheetBusiness.Instance.Statistics(request.QuestionnaireID);
            }
            catch (Exception ex)
            {
                resp.CaptureException(ex);
            }
            return resp;
        }
    }
}
