using Newegg.API.Attributes;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.EggRolls.RequestEntities
{
    [RestService("/questionnaire/search")]
    public class QuestionnaireSearchRequest: GeneralRequest
    {
        public string ShortName { get; set; }
        public string Title { get; set; }
    }
}