using System;
using Newegg.API.Interfaces;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.Extensions;

namespace Newegg.MIS.API.EggRolls.Services
{
    public class QuestionnaireService : RestServiceBase<QuestionnaireRequest>
    {
        public override object OnPost(QuestionnaireRequest request)
        {
            var resp = new QuestionnaireResponse();

            try
            {
                resp =  QuestionnaireBusiness.Instance.Add(request);
            }
            catch (Exception ex)
            {
                resp.CaptureException(ex);
            }

            return resp;
        }

        public override object OnDelete(QuestionnaireRequest request)
        {
            var resp = new QuestionnaireResponse();
            try
            {
                QuestionnaireBusiness.Instance.Delete(request);
            }
            catch(Exception ex)
            {
                resp.CaptureException(ex);
            }
            return resp;
        }

        public override object OnPut(QuestionnaireRequest request)
        {
            var resp = new QuestionnaireResponse();
            try
            {
                QuestionnaireBusiness.Instance.Update(request);
            }
            catch (Exception ex)
            {
                resp.CaptureException(ex);
            }
            return resp;
        }

        public override object OnGet(QuestionnaireRequest request)
        {
            var resp = new QuestionnaireResponse();
            try
            {
                resp.Questionnaire = QuestionnaireBusiness.Instance.Query(request.QuestionnaireID);
            }
            catch (Exception ex)
            {
                resp.CaptureException(ex);
            }
            return resp;
        }
    }
}