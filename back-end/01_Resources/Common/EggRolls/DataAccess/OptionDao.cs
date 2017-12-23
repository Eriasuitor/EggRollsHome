using System.Collections.Generic;
using System.Linq;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.DataAccess;
using Newegg.MIS.API.Utilities.Helpers;

namespace Newegg.MIS.API.EggRolls.DataAccess
{
    public interface IOptionDao
    {
        void Add(QuestionnaireRequest request);
        int Delete(int questionnaireID);
        List<Option> Query(int questionnaireID);
    }
    public class OptionDao : IOptionDao
    {
        static OptionDao()
        {
            InstanceManager.RegisterBuilder<IOptionDao, OptionDao>(() => new OptionDao());
        }

        public static IOptionDao Instance
        {
            get { return InstanceManager.GetInstance<IOptionDao>(); }
        }

        public void Add(QuestionnaireRequest request)
        {
            var optionList = request.Topics.SelectMany(topic => topic.Options).ToList();
            var optionListSerialize = SerializationHelper.Serialize(optionList, null);
            var command = DataCommandFactory.Get("EggRolls_Common_Option_Add_Bunch")
                .SetParameterValue("@QuestionnaireID", request.QuestionnaireID)
                .SetParameterValue("@ShortName", request.ShortName)
                .SetParameterValue("@OptionListSerialize", optionListSerialize);

            var effectRows = command.ExecuteNonQuery();
        }

        public int Delete(int questionnaireID)
        {
            var command = DataCommandFactory.Get("MIS_EggRolls_DeleteOption_Only")
                .SetParameterValue("@QuestionnaireID", questionnaireID);
            return command.ExecuteNonQuery();
        }

        public List<Option> Query(int questionnaireID)
        {
            var command = DataCommandFactory.Get("MIS_EggRolls_QueryOption")
                .SetParameterValue("@QuestionnaireID", questionnaireID);

            return command.ExecuteEntityList<Option>();
        }
    }
}
