using System.Collections.Generic;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.EggRolls.ResponseEntities
{
    public class AnswerSheetResponse:GeneralResponse
    {
        public List<Answer> AnswerList { set; get; }
    }
}
