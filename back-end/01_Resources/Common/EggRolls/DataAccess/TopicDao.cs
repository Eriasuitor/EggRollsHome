using System.Collections.Generic;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.DataAccess;
using Newegg.MIS.API.Utilities.Helpers;

namespace Newegg.MIS.API.EggRolls.DataAccess
{
    public interface ITopicDao
    {
        void Add(QuestionnaireRequest request);
        int Delete(int questionnaireID);
        List<Topic> Query(int questionnaireID);
    }
    public class TopicDao : ITopicDao
    {
        static TopicDao()
        {
            InstanceManager.RegisterBuilder<ITopicDao, TopicDao>(() => new TopicDao());
        }

        public static ITopicDao Instance
        {
            get { return InstanceManager.GetInstance<ITopicDao>(); }
        }

        public void Add(QuestionnaireRequest request)
        {
            var topicListSerialize = SerializationHelper.Serialize(request.Topics, null);

            var command = DataCommandFactory.Get("EggRolls_Common_Topic_Add_Bunch")
                .SetParameterValue("@QuestionnaireID", request.QuestionnaireID)
                .SetParameterValue("@ShortName", request.ShortName)
                .SetParameterValue("@TopicListSerialize", topicListSerialize);

            command.ExecuteNonQuery();
        }

        public int Delete(int questionnaireID)
        {
            var commond = DataCommandFactory.Get("MIS_EggRolls_DeleteTopic_Only")
                .SetParameterValue("@QuestionnaireID", questionnaireID);

            return commond.ExecuteNonQuery();
        }

        public List<Topic> Query(int questionnaireID)
        {
            var command = DataCommandFactory.Get("MIS_EggRolls_QueryTopic")
                .SetParameterValue("@QuestionnaireID", questionnaireID);

            return command.ExecuteEntityList<Topic>();
        }
    }
}
