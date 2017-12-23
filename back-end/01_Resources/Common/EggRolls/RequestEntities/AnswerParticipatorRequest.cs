using Newegg.API.Attributes;

namespace Newegg.MIS.API.EggRolls.RequestEntities
{
    [RestService("/answer-sheet/answer/participator")]
    public class AnswerParticipatorRequest
    {
        public int QuestionnaireID { set; get; }
        public int TopicID { set; get; }
        public string OptionID { set; get; }
    }
}
