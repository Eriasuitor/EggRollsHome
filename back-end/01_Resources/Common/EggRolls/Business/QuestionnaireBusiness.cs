using System;
using System.Collections.Generic;
using System.Transactions;
using Newegg.FrameworkAPI.SDK.Mail;
using Newegg.MIS.API.EggRolls.DataAccess;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.Extensions;
using Newegg.MIS.API.Utilities.Helpers;

namespace Newegg.MIS.API.EggRolls.Business
{
    public interface IQuestionnaireBusiness
    {
        QuestionnaireSearchResponse Search(QuestionnaireSearchRequest request);
        QuestionnaireResponse Add(QuestionnaireRequest request);
        int Delete(QuestionnaireRequest request);
        int Update(QuestionnaireRequest request);
        Questionnaire Query(int questionnaireID);
        List<Participator> QueryParticipator(int questionnaireID);
        int QuestionnaireStatusRefresh();
    }

    public class QuestionnaireBusiness : IQuestionnaireBusiness
    {
        static QuestionnaireBusiness()
        {
            InstanceManager.RegisterBuilder<IQuestionnaireBusiness, QuestionnaireBusiness>(() => new QuestionnaireBusiness());
        }

        public static IQuestionnaireBusiness Instance
        {
            get
            {
                return InstanceManager.GetInstance<IQuestionnaireBusiness>();
            }
        }

        public QuestionnaireSearchResponse Search(QuestionnaireSearchRequest request)
        {
            return QuestionnaireDao.Instance.Search(request);
        }

        public QuestionnaireResponse Add(QuestionnaireRequest request)
        {
            var questionnaireResponse = new QuestionnaireResponse
            {
                Questionnaire = new Questionnaire()
            };

            TimeLegalityJudgment(request.DueDate, request.Status);

            using (var scope = new TransactionScope())
            {
                request.QuestionnaireID = QuestionnaireDao.Instance.Add(request);
                TopicDao.Instance.Add(request);
                OptionDao.Instance.Add(request);
                scope.Complete();
            }

            questionnaireResponse.Questionnaire.QuestionnaireID = request.QuestionnaireID;

            if (request.MailTo != null)
            {
                try
                {
                    var mailRequest = new MailRequest();
                    mailRequest.Priority = MailPriority.Normal;
                    mailRequest.ContentType = MailContentType.Html;
                    mailRequest.MailType = MailType.Smtp;

                    SmtpSetting smtpSetting = new SmtpSetting();
                    smtpSetting.SubjectEncoding = MailEncoding.UTF8;
                    smtpSetting.BodyEncoding = MailEncoding.UTF8;

                    mailRequest.To = "Lory.Y.Jiang@newegg.com";
                    mailRequest.Subject = "[EggRolls] " + request.Title;
                    mailRequest.SmtpSetting = smtpSetting;
                    mailRequest.From = "Egg_Rolls@newegg.com";
                    mailRequest.Body = "<html><h1>飘</h1><h2>来了一张</h2>调查表《" + request.Title + "》需要您来填写，请点击链接：" +
                                       request.QuestionnaireID + "</html>";
                    questionnaireResponse.MailSucceeded = MailSenderHelper.Instance.Send(mailRequest).IsSendSuccess;
                }
                catch (Exception ex)
                {
                    questionnaireResponse.MailSucceeded = false;
                    questionnaireResponse.CaptureException(ex);
                    questionnaireResponse.Succeeded = true;
                }
            }
            return questionnaireResponse;
        }

        public int Delete(QuestionnaireRequest request)
        {
            QuestionnaireExistenceJudgment(request.QuestionnaireID);
            int effecRowsQuestionnaire;
            using (var scope = new TransactionScope())
            {
                effecRowsQuestionnaire = QuestionnaireDao.Instance.Delete(request.QuestionnaireID);
                TopicDao.Instance.Delete(request.QuestionnaireID);
                OptionDao.Instance.Delete(request.QuestionnaireID);
                AnswerDao.Instance.Delete(request.QuestionnaireID);
                scope.Complete();
            }

            return effecRowsQuestionnaire;
        }

        public int Update(QuestionnaireRequest request)
        {
            TimeLegalityJudgment(request.DueDate, request.Status);

            QuestionnaireExistenceJudgment(request.QuestionnaireID);
         
            int effecRowsQuestionnaire;
            using (var scope = new TransactionScope())
            {
                effecRowsQuestionnaire = QuestionnaireDao.Instance.Update(request);
                TopicDao.Instance.Delete(request.QuestionnaireID);
                TopicDao.Instance.Add(request);
                OptionDao.Instance.Delete(request.QuestionnaireID);
                OptionDao.Instance.Add(request);
                scope.Complete();
            }

            return effecRowsQuestionnaire;
        }

        public Questionnaire Query(int questionnaireID)
        {
            var questionnaire = QuestionnaireExistenceJudgment(questionnaireID);

            questionnaire.Topics = TopicDao.Instance.Query(questionnaireID);
            var options = OptionDao.Instance.Query(questionnaireID);

            foreach (var t in questionnaire.Topics)
            {
                var op = options.FindAll(p => p.TopicID == t.TopicID);
                t.Options = op;
            }

            return questionnaire;
        }

        public void TimeLegalityJudgment(DateTime? dateTime, QuestionnaireStatus status)
        {
            if (status != QuestionnaireStatus.Processing)
            {
                return;
            }

            if (dateTime == null)
            {
                throw new ApplicationException("Deadline have to be assigned if you want to publish a questionnaire.");
            }

            if (dateTime <= DateTime.Now)
            {
                throw new ApplicationException("Deadline have to be later than now if you want to publish a questionnaire. Now is " +  DateTime.Now + " but you are + " + dateTime);
            }
        }

        public static Questionnaire QuestionnaireExistenceJudgment(int questionnaireID)
        {
            var questionnaire = QuestionnaireDao.Instance.Query(questionnaireID);
            if (questionnaire == null)
            {
                throw new ApplicationException("No questionnaire was found whose id is " + questionnaireID);
            }
            return questionnaire;
        }

        public List<Participator> QueryParticipator(int questionnaireID)
        {
            return AnswerDao.Instance.QueryParticipator(questionnaireID);
        }

        public int QuestionnaireStatusRefresh()
        {
            return QuestionnaireDao.Instance.StatusRefresh();
        }
    }
}