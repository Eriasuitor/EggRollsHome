using System.Collections.Generic;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.EggRolls.ResponseEntities
{
    public class QuestionnaireSearchResponse: GeneralResponse
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public int? Pages { get; set; }
        public List<Questionnaire> Questionnaires { get; set; }
    }
}
