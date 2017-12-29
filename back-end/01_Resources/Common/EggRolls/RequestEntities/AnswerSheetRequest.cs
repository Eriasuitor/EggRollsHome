using System.Collections.Generic;
using Newegg.API.Attributes;
using Newegg.MIS.API.EggRolls.Entities;

namespace Newegg.MIS.API.EggRolls.RequestEntities
{
    [RestService("/answer-sheet")]
    public class AnswerSheetRequest
    {
        public int QuestionnaireID { get; set; }

        public string ShortName { get; set; }

        public string FullName { get; set; }

        public string Department { get; set; }

        public List<Answer> AnswerList { get; set; }
    }
}
