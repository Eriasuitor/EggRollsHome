﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.DataAccess;
using Newegg.MIS.API.Utilities.Helpers;

namespace Newegg.MIS.API.EggRolls.DataAccess
{
    public interface IAnswerSheetDao
    {
        void Add(AnswerSheetRequest request, string country, string area, string supportCenter);
        int Delete(int questionnaireID);
        List<Answer> Query(int questionnaireID, string shortName);
        List<Units> Statistics(int questionnaireID);
        List<Participator> QueryParticipator(int questionnaireID);
        List<Answer> Query(int questionnaireID, int topicID);
        List<Answer> Query(int questionnaireID, int topicID,string optionID);
    }
    public class AnswerDao : IAnswerSheetDao
    {
        static AnswerDao()
        {
            InstanceManager.RegisterBuilder<IAnswerSheetDao, AnswerDao>(() => new AnswerDao());
        }

        public static IAnswerSheetDao Instance
        {
            get { return InstanceManager.GetInstance<IAnswerSheetDao>(); }
        }

        public void Add(AnswerSheetRequest request, string country,string area,string supportCenter)
        {
            var answerList = request.Topics.SelectMany(topic => topic.Answers).ToList();
            var answerListSerialize = SerializationHelper.Serialize(answerList, null);
            var command = DataCommandFactory.Get("EggRolls_Common_Answer_Add_Bunch")
                .SetParameterValue("@QuestionnaireID", request.QuestionnaireID)
                .SetParameterValue("@AnswerListSerialize", answerListSerialize)
                .SetParameterValue("@Department", request.Department)
                .SetParameterValue("@ShortName", request.ShortName)
                .SetParameterValue("@SupportCenter", supportCenter)
                .SetParameterValue("@FullName", request.FullName)
                .SetParameterValue("@Country", country)
                .SetParameterValue("@Area", area);

            command.ExecuteNonQuery();
        }

        public int Delete(int questionnaireID)
        {
            var command = DataCommandFactory.Get("EggRolls_Common_Answer_Delete")
                .SetParameterValue("@QuestionnaireID", questionnaireID);

            return command.ExecuteNonQuery();
        }

        public List<Answer> Query(int questionnaireID, string shortName)
        {
            var command = DataCommandFactory.Get("EggRolls_Common_Answer_Query")
                .SetParameterValue("@QuestionnaireID", questionnaireID)
                .SetParameterValue("@ShortName", shortName);

            return command.ExecuteEntityList<Answer>();
        }

        public List<Answer> Query(int questionnaireID, int topicID)
        {
            var command = DataCommandFactory.Get("EggRolls_Common_Answer_Query_All_Answer_Of_A_Topic")
                .SetParameterValue("@QuestionnaireID", questionnaireID)
                .SetParameterValue("@TopicID", topicID);

            return command.ExecuteEntityList<Answer>();
        }

        public List<Answer> Query(int questionnaireID, int topicID, string optionID)
        {
            var command = DataCommandFactory.Get("EggRolls_Common_Answer_Query_All_Answer_Of_A_Option")
                .SetParameterValue("@QuestionnaireID", questionnaireID)
                .SetParameterValue("@TopicID", topicID)
                .SetParameterValue("@OptionID", optionID);

            return command.ExecuteEntityList<Answer>();
        }

        public List<Participator> QueryParticipator(int questionnaireID)
        {
            var command = DataCommandFactory.Get("EggRolls_Common_Questionnaire_Participator")
                .SetParameterValue("@QuestionnaireID", questionnaireID);

            return command.ExecuteEntityList<Participator>();
        }

        public List<Units> Statistics(int questionnaireID)
        {
            var command = DataCommandFactory.Get("MIS_EggRolls_Answer_Sheet_Statistics")
                .SetParameterValue("@QuestionnaireID", questionnaireID);

            return command.ExecuteEntityList<Units>();
        }
    }
}
