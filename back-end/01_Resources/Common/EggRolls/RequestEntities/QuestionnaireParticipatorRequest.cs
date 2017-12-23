using Newegg.API.Attributes;

namespace Newegg.MIS.API.EggRolls.RequestEntities
{
    [RestService("/questionnaire/participator")]
    public class QuestionnaireParticipatorRequest
    {
        public int QuestionnaireID { set; get; }
    }
}