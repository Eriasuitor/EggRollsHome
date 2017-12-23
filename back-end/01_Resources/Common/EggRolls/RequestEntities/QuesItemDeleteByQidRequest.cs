
using Newegg.API.Attributes;

namespace Newegg.MIS.API.EggRolls.RequestEntities
{
    /// <summary>
    /// 删除调查表
    /// Delete the questionnaire
    /// </summary>
    [RestService("/questionnaires/{QuestionnaireID}")]
    public class QuesItemDeleteByQidRequest
    {
        public int QuestionnaireID { get; set; }
    }
}
