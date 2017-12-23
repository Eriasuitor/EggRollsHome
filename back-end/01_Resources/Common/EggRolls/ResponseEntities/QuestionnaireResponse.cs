using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.EggRolls.ResponseEntities
{
    public class QuestionnaireResponse : GeneralResponse
    {
        public Questionnaire Questionnaire { set; get; }
        public bool MailSucceeded { set; get; }
    }
}