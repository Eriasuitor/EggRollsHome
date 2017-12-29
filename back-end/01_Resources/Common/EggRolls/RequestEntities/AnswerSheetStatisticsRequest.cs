using Newegg.API.Attributes;

namespace Newegg.MIS.API.EggRolls.RequestEntities
{
    [RestService("/answer-sheet/statistics")]
    public class AnswerSheetStatisticsRequest
    {
        public int QuestionnaireID { set; get; }
    }
}
