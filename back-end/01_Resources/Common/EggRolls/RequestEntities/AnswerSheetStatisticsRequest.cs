using Newegg.API.Attributes;
using Newegg.MIS.API.EggRolls.Entities;

namespace Newegg.MIS.API.EggRolls.RequestEntities
{
    [RestService("/answer-sheet/statistics")]
    public class AnswerSheetStatisticsRequest
    {
        public int QuestionnaireID { set; get; }
    }
}
