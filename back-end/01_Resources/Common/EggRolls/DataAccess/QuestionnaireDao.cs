using System;
using System.Linq;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.DataAccess;
using Newegg.MIS.API.Utilities.Helpers;

namespace Newegg.MIS.API.EggRolls.DataAccess
{
    public interface IQuestionnaireDao
    {
        QuestionnaireSearchResponse Search(QuestionnaireSearchRequest request);
        int Add(Questionnaire request);
        int Delete(int questionnaireID);
        int Update(QuestionnaireRequest request);
        Questionnaire Query(int questionnaireID);
    }

    public class QuestionnaireDao : IQuestionnaireDao
    {
        static QuestionnaireDao()
        {
            InstanceManager.RegisterBuilder<IQuestionnaireDao, QuestionnaireDao>(() => new QuestionnaireDao());
        }

        public static IQuestionnaireDao Instance
        {
            get { return InstanceManager.GetInstance<IQuestionnaireDao>(); }
        }

        public QuestionnaireSearchResponse Search(QuestionnaireSearchRequest request)
        {
            var command = DataCommandFactory.Get("EggRolls_Common_Questionnaire_QueryByTitle_WithPaging")
                .SetParameterValue("@ShortName", request.ShortName)
                .SetParameterValue("@Title", request.Title ?? "")
                .SetParameterValue("@StartPageIndex", request.PageIndex)
                .SetParameterValue("@EndPageIndex", request.PageIndex)
                .SetParameterValue("@PageSize", request.PageSize);

            var questionnaireSearchResponse = new QuestionnaireSearchResponse
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            using (var reader = command.ExecuteMultiple())
            {
                questionnaireSearchResponse.Pages = reader.Read<int>().ToList()[0];
                questionnaireSearchResponse.Pages = questionnaireSearchResponse.Pages / questionnaireSearchResponse.PageSize + (questionnaireSearchResponse.Pages % questionnaireSearchResponse.PageSize == 0 ? 0 : 1);
                questionnaireSearchResponse.Questionnaires = reader.Read<Questionnaire>().ToList();
            }

            return questionnaireSearchResponse;
        }

        public int Add(Questionnaire request)
        {
            var command = DataCommandFactory.Get("EggRolls_Common_Questionnaire_Creation")
                .SetParameterValue("@ShortName", request.ShortName)
                .SetParameterValue("@FullName", request.FullName)
                .SetParameterValue("@Status", request.Status)
                .SetParameterValue("@Title", request.Title)
                .SetParameterValue("@Description", request.Description)
                .SetParameterValue("@BackgroundImageUrl", request.BackgroundImageUrl)
                .SetParameterValue("@IsRealName", request.IsRealName)
                .SetParameterValue("@DueDate", request.DueDate);

            //Ques here
            command.ExecuteNonQuery();
            return Convert.ToInt32(command.GetParameterValue("QuestionnaireID"));
        }

        public int Delete(int questionnaireID)
        {
            var commond = DataCommandFactory.Get("MIS_EggRolls_DeleteQuestionnaire_Only")
                .SetParameterValue("@QuestionnaireID", questionnaireID);

            return commond.ExecuteNonQuery();
        }

        public int Update(QuestionnaireRequest request)
        {

            var command = DataCommandFactory.Get("EggRolls_Common_Questionnaire_Update")
                .SetParameterValue("@QuestionnaireID",request.QuestionnaireID)
                .SetParameterValue("@ShortName", request.ShortName)
                .SetParameterValue("@FullName", request.FullName)
                .SetParameterValue("@Status", request.Status)
                .SetParameterValue("@Title", request.Title)
                .SetParameterValue("@Description", request.Description)
                .SetParameterValue("@BackgroundImageUrl", request.BackgroundImageUrl)
                .SetParameterValue("@IsRealName", request.IsRealName)
                .SetParameterValue("@DueDate", request.DueDate);

            //Ques here
            return command.ExecuteNonQuery();
        }

        public Questionnaire Query(int questionnaireID)
        {
            var command = DataCommandFactory.Get("MIS_EggRolls_QueryQuestionnaire")
                .SetParameterValue("@QuestionnaireID", questionnaireID);
            
            return command.ExecuteEntity<Questionnaire>();
        }
    }
}
