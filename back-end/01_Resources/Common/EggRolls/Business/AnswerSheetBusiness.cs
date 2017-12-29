using System;
using System.Collections.Generic;
using Newegg.MIS.API.EggRolls.DataAccess;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.Extensions;
using Newegg.MIS.API.Utilities.Helpers;

namespace Newegg.MIS.API.EggRolls.Business
{
    public interface IAnswerSheetBusiness
    {
        void Add(AnswerSheetRequest request);
        List<Answer> Query(int questionnaireID, string shortName);
        List<ParticipatorStatistics> Statistics(int questionnaireID);
        List<Answer> Query(int questionnaireID, int topicID);
        List<Answer> Query(int questionnaireID, int topicID, string optionID);
    }

    public class AnswerSheetBusiness : IAnswerSheetBusiness
    {
        static AnswerSheetBusiness()
        {
            InstanceManager.RegisterBuilder<IAnswerSheetBusiness, AnswerSheetBusiness>(() => new AnswerSheetBusiness());
        }

        public static IAnswerSheetBusiness Instance
        {
            get
            {
                return InstanceManager.GetInstance<IAnswerSheetBusiness>();
            }
        }

        private static void RadioTopicValidation(Topic topic, List<Answer> topicAnswersList)
        {
            if (topic.Type != TopicType.Radio)
            {
                return;
            }

            foreach (var topicAnswer in topicAnswersList)
            {
                if (!topic.Options.Exists(p => p.OptionID == topicAnswer.Ans))
                {
                    throw new ApplicationException("Option " + topicAnswer.Ans + "is not found in topic " + topic.TopicID);
                }

                topicAnswer.Type = TopicType.Radio;
            }

            if (topic.IsRequired)
            {
                if (topicAnswersList.Count != 1)
                {
                    throw new ApplicationException("Wrong number of answer to the topic " + topic.TopicID);
                }
            }
            else
            {
                if (topicAnswersList.Count > 1)
                {
                    throw new ApplicationException("Wrong number of answer to the topic " + topic.TopicID);
                }
            }
        }

        private static void CheckboxTopicValidation(Topic topic, List<Answer> topicAnswersList)
        {
            if (topic.Type != TopicType.Checkbox)
            {
                return;
            }

            foreach (var topicAnswer in topicAnswersList)
            {
                if (!topic.Options.Exists(p => p.OptionID == topicAnswer.Ans))
                {
                    throw new ApplicationException("Option " + topicAnswer.Ans + "is not found in topic " + topic.TopicID);
                }

                topicAnswer.Type = TopicType.Checkbox;
            }

            if (topic.IsRequired)
            {
                if (topic.Limited != 0)
                {
                    if(topicAnswersList.Count != topic.Limited)
                    {
                        throw new ApplicationException("Wrong number of answer to the topic " + topic.TopicID);
                    }
                }
                else
                {
                    if (topicAnswersList.Count == 0)
                    {
                        throw new ApplicationException("Wrong number of answer to the topic " + topic.TopicID);
                    }
                }
            }
            else
            {
                if (topicAnswersList.Count != 0 
                    && topic.Limited != 0
                    && topicAnswersList.Count != topic.Limited)
                {
                    throw new ApplicationException("Wrong number of answer to the topic " + topic.TopicID);
                }
            }
        }

        private static void TextTopicValidation(Topic topic, List<Answer> topicAnswersList)
        {
            if (topic.Type != TopicType.Text)
            {
                return;
            }

            if (topic.IsRequired)
            {
                if (topicAnswersList.Count != 1)
                {
                    throw new ApplicationException("Answer is required to topic " + topic.TopicID);
                }

                if (string.IsNullOrWhiteSpace(topicAnswersList[0].Ans))
                {
                    throw new ApplicationException("Answer is required to topic " + topic.TopicID);
                }
            }

            foreach (var topicAnswer in topicAnswersList)
            {
                topicAnswer.Type = TopicType.Text;
            }
        }

        public void Add(AnswerSheetRequest request)
        {
            var questionnaire = QuestionnaireBusiness.Instance.Query(request.QuestionnaireID);

            var depatmentSplit = request.Department.Split();
            if (depatmentSplit.Length != 4)
            {
                throw new ApplicationException("Wrong format of Department field");
            }

            if (questionnaire.Status != QuestionnaireStatus.Processing)
            {
                throw new ApplicationException("Wrong status of questionnaire");
            }

            if (AnswerDao.Instance.QueryExisted(request.QuestionnaireID, request.ShortName))
            {
                throw new ApplicationException("Answer sheet has been existed");
            }

            foreach (var topic in questionnaire.Topics)
            {
                var topicAnswersList = request.AnswerList.FindAll(p => p.TopicID == topic.TopicID);

                RadioTopicValidation(topic, topicAnswersList);

                CheckboxTopicValidation(topic, topicAnswersList);

                TextTopicValidation(topic, topicAnswersList);
            }

            request.Department = depatmentSplit[3];

            AnswerDao.Instance.Add(request, depatmentSplit[0], depatmentSplit[1], depatmentSplit[2]);
        }

        public List<Answer> Query(int questionnaireID, string shortName)
        {
            if (!QuestionnaireDao.Instance.QuestionnaireExistenceJudgment(questionnaireID))
            {
                throw new ApplicationException("No questionnaire was found whose id is " + questionnaireID);
            }

            var answerList = AnswerDao.Instance.Query(questionnaireID, shortName);

            if (answerList.IsEmpty())
            {
                throw new ApplicationException("No answer was found");
            }

            return answerList;
        }

        public List<Answer> Query(int questionnaireID, int topicID)
        {
            return AnswerDao.Instance.Query(questionnaireID, topicID);
        }

        public List<Answer> Query(int questionnaireID, int topicID, string optionID)
        {
            if (optionID == null)
            {
                return AnswerDao.Instance.Query(questionnaireID, topicID);

            }
            return AnswerDao.Instance.Query(questionnaireID, topicID, optionID);
        }

        public List<ParticipatorStatistics> Statistics(int questionnaireID)
        {
            if (!QuestionnaireDao.Instance.QuestionnaireExistenceJudgment(questionnaireID))
            {
                throw new ApplicationException("No questionnaire was found whose id is " + questionnaireID);
            }

            var participatorStatisticsLists = AnswerDao.Instance.Statistics(questionnaireID);

            var participatorStatisticsList = (List<ParticipatorStatistics>)participatorStatisticsLists[0];
            var departmentStatisticsList = (List<AnswerStatistics>)participatorStatisticsLists[1];

            foreach (var participatorStatistics in participatorStatisticsList)
            {
                participatorStatistics.DepartmentStatisticsList = departmentStatisticsList.FindAll((p) =>
                    p.TopicID == participatorStatistics.TopicID && p.OptionID == participatorStatistics.OptionID);
            }

            return  participatorStatisticsList;
        }
    }
}
