using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                resp.AnswerSheet = AnswerSheetBusiness.Instance.Query(request);
            }
            catch (Exception ex)
            {
                resp.CaptureException(ex);
            }
            return resp;
        }
    }
}