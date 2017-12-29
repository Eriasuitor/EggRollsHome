using System;
using Newegg.API.Interfaces;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.Extensions;

namespace Newegg.MIS.API.EggRolls.Services
{
    public class AnswerSheetService: RestServiceBase<AnswerSheetRequest>
    {
        public override object OnPost(AnswerSheetRequest request)
        {
            var resp = new AnswerParticipatorResponse();
            try
            {
                AnswerSheetBusiness.Instance.Add(request);
            }
            catch (Exception ex)
            {
                resp.CaptureException(ex);
            }

            return resp;
        }

        public override object OnGet(AnswerSheetRequest request)
        {
            var resp = new AnswerSheetResponse();
            try
            {
                resp.AnswerList = AnswerSheetBusiness.Instance.Query(request.QuestionnaireID,request.ShortName);
            }
            catch (Exception ex)
            {
                resp.CaptureException(ex);
            }
            return resp;
        }
    }
}