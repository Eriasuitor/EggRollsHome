using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Newegg.API.Common;
using Newegg.MIS.API.EggRolls.DataAccess;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.Helpers;

namespace Newegg.MIS.API.EggRolls.Business
{
    public interface IAnswerSheetBusiness
    {
        void Add(AnswerSheetRequest request);
        AnswerSheet Query(AnswerSheetRequest request);
        AnswerSheet Statistics(int questionnaireID);
        List<Participator> QueryParticipator(int questionnaireID);
        List<Answer> Query(int questionnaireID,int topicID);
        List<Answer> Query(int questionnaireID, int topicID,string optionID);
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

        public void Add(AnswerSheetRequest request)
        {
            var questionnaire = QuestionnaireBusiness.QuestionnaireExistenceJudgment(request.QuestionnaireID);
            if (questionnaire.Status == QuestionnaireStatus.Draft)
            {
                throw new ApplicationException("Can't submit an answer for the draft");
            }
            if (questionnaire.Status == QuestionnaireStatus.Ended)
            {
                throw new ApplicationException("Can't submit an answer for a completed questionnaire");
            }
            if (AnswerDao.Instance.Query(request.QuestionnaireID, request.ShortName).Count != 0)
            {
                throw new ApplicationException("Answer sheet has been existed");
            }
            var depatmentSplit = request.Department.Split();
            if (depatmentSplit.Length != 4)
            {
                throw new ApplicationException("Wrong format of Department field");
            }
            request.Department = depatmentSplit[3];
            AnswerDao.Instance.Add(request, depatmentSplit[0], depatmentSplit[1], depatmentSplit[2]);
        }

        public AnswerSheet Query(AnswerSheetRequest request)
        {
            var answerSheet = QuestionnaireBusiness.QuestionnaireExistenceJudgment(request.QuestionnaireID).TranslateTo<AnswerSheet>();

            answerSheet.Topics = TopicDao.Instance.Query(request.QuestionnaireID);
            var answers = AnswerDao.Instance.Query(request.QuestionnaireID, request.ShortName);
            if (answers.Count == 0)
            {
                throw new ApplicationException("No answer sheet was found");
            }
            var options = OptionDao.Instance.Query(request.QuestionnaireID);

            foreach (var t in answerSheet.Topics)
            {
                var op = options.FindAll(p => p.TopicID == t.TopicID);
                var an = answers.FindAll(p => p.TopicID == t.TopicID);
                t.Options = op;
                t.Answers = an;
            }

            return answerSheet;
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

        public List<Participator> QueryParticipator(int questionnaireID)
        {
            return AnswerDao.Instance.QueryParticipator(questionnaireID);
        }

        public AnswerSheet Statistics(int questionnaireID)
        {
            var answerSheet = QuestionnaireBusiness.Instance.Query(questionnaireID).TranslateTo<AnswerSheet>();
            if (answerSheet.Participants == 0)
            {
                for (int i = 0; i < answerSheet.Topics.Count; i++)
                {
                    for (int j = 0; j < answerSheet.Topics[i].Options.Count; j++)
                    {
                        answerSheet.Topics[i].Options[j].PersonalUnits = "0.00%";
                    }
                }
                return answerSheet;
            }
            var unitsList = AnswerDao.Instance.Statistics(questionnaireID);
            var departmentList = new List<string>();
            for (int i = 0; i < unitsList.Count; i++)
            {
                departmentList.Add(unitsList[i].Department);
            }
            departmentList = departmentList.Distinct().ToList();
            var departmentUnites = new List<Units>();
            for (int i = 0; i < departmentList.Count; i++)
            {
                departmentUnites.Add(new Units
                {
                    Department = departmentList[i],
                    ChosenNumber = 0,
                    Percentage = "0.00%"
                });
            }
            for (int i = 0; i < answerSheet.Topics.Count; i++)
            {
                for (int j = 0; j < answerSheet.Topics[i].Options.Count; j++)
                {
                    var op = new List<Units>();
                    answerSheet.Topics[i].Options[j].DepartmentUnits = new List<Units>(departmentUnites);
                    for (int k = 0; k < unitsList.Count; k++)
                    {
                        if (unitsList[k].TopicID == answerSheet.Topics[i].TopicID && unitsList[k].Answer == answerSheet.Topics[i].Options[j].OptionID)
                        {
                            op.Add(unitsList[k]);
                        }
                    }
                    var totNumber = 0;
                    for (int k = 0; k < op.Count; k++)
                    {
                        totNumber += op[k].ChosenNumber;
                    }
                    for (int k = 0; k < unitsList.Count; k++)
                    {
                        if (unitsList[k].TopicID == answerSheet.Topics[i].TopicID && unitsList[k].Answer == answerSheet.Topics[i].Options[j].OptionID)
                        {
                            unitsList[k].Percentage = ((decimal)unitsList[k].ChosenNumber / totNumber * 100).ToString("0.00") + "%";
                            for (int m = 0; m < departmentUnites.Count; m++)
                            {
                                if (answerSheet.Topics[i].Options[j].DepartmentUnits[m].Department == unitsList[k].Department)
                                {
                                    answerSheet.Topics[i].Options[j].DepartmentUnits[m] = unitsList[k];
                                }
                            }
                        }
                    }
                    answerSheet.Topics[i].Options[j].ChosenNumber = totNumber;
                    answerSheet.Topics[i].Options[j].PersonalUnits = ((decimal)totNumber / answerSheet.Participants * 100).ToString("0.00") + "%";
                }
            }
            return answerSheet;
        }
    }
}
