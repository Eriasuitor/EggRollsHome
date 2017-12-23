using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.EggRolls.DataAccess;

namespace Newegg.MIS.API.EggRolls.Business
{
    public class QuesItemDeleteByQidBusiness
    {
        public static void DeleteQuestionnaire(QuesItemDeleteByQidRequest request, QuesItemDeleteByQidResponse response)
        {
            response.EffectRows = QuesItemDeleteByQidDao.Instance.DeleteQuestionnaire(request.QuestionnaireID);
        }
         
    }
}
