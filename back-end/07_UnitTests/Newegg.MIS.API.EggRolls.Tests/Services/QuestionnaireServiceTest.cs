using System;
using Newegg.API.Common;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.EggRolls.Services;
using NUnit.Framework;
using NSubstitute;
using Newegg.MIS.API.Utilities.Helpers;
using NSubstitute.ExceptionExtensions;

namespace Newegg.MIS.API.EggRolls.Tests.Services
{
    [TestFixture]
    public class QuestionnaireServiceTest
    {
        [SetUp]
        public void Setup()
        {
            var questionnaireBusiness = Substitute.For<IQuestionnaireBusiness>();
            InstanceManager.Register<IQuestionnaireBusiness>().Use(questionnaireBusiness);
        }

        [TearDown]
        public void Clearup()
        {
            InstanceManager.Register<IQuestionnaireBusiness>().Reset();
        }

        [Test]
        public void Test_On_Post_Right()
        {
            var request = new QuestionnaireRequest
            {
                ShortName = "tr29",
                Title = "123"
            };

            //Mock
            QuestionnaireBusiness.Instance
                .Add(request)
                .Returns(new QuestionnaireResponse
                {
                    Questionnaire = new Questionnaire
                    {
                        QuestionnaireID = 255
                    }
                });

            var actualResp = new QuestionnaireService().OnPost(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreNotEqual(actualResp.TranslateTo<QuestionnaireResponse>().Questionnaire, null);
            Assert.AreEqual(actualResp.TranslateTo<QuestionnaireResponse>().Questionnaire.QuestionnaireID, 255);
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Succeeded, true);
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Errors, null);
        }

        [Test]
        public void Test_On_Post_Wrong()
        {
            var request = new QuestionnaireRequest
            {
                ShortName = "tr29",
                Title = "123"
            };
            var exception = new AccessViolationException();
            //Mock
            QuestionnaireBusiness.Instance
                .Add(request)
                .Throws(exception);

            var actualResp = new QuestionnaireService().OnPost(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.GetType(), typeof(QuestionnaireResponse));
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Succeeded, false);
            Assert.AreNotEqual(((QuestionnaireResponse)actualResp).Errors, null);
        }

        [Test]
        public void Test_On_Delete_Right()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 100
            };
            //Mock
            QuestionnaireBusiness.Instance
                .Delete(request)
                .Returns(1);

            var actualResp = new QuestionnaireService().OnDelete(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.GetType(), typeof(QuestionnaireResponse));
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Succeeded, true);
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Errors, null);
        }

        [Test]
        public void Test_On_Delete_Wrong()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 100
            };
            var exception = new AccessViolationException();
            //Mock
            QuestionnaireBusiness.Instance
                .Delete(request)
                .Throws(exception);

            var actualResp = new QuestionnaireService().OnDelete(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.GetType(), typeof(QuestionnaireResponse));
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Succeeded, false);
            Assert.AreNotEqual(((QuestionnaireResponse)actualResp).Errors, null);
        }

        [Test]
        public void Test_On_Put_Right()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255,
                ShortName = "tr29",
                Title = "123"
            };

            var actualResp = new QuestionnaireService().OnPut(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.GetType(), typeof(QuestionnaireResponse));
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Succeeded, true);
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Errors, null);
        }

        [Test]
        public void Test_On_Put_Wrong()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255,
                ShortName = "tr29",
                Title = "123"
            };
            var exception = new AccessViolationException();
            //Mock
            QuestionnaireBusiness.Instance
                .Update(request)
                .Throws(exception);

            var actualResp = new QuestionnaireService().OnPut(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.GetType(), typeof(QuestionnaireResponse));
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Succeeded, false);
            Assert.AreNotEqual(((QuestionnaireResponse)actualResp).Errors, null);
        }

        [Test]
        public void Test_On_Get_Right()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255,
            };

            var questionnaire = new Questionnaire();

            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            var actualResp = new QuestionnaireService().OnGet(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.GetType(), typeof(QuestionnaireResponse));
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Succeeded, true);
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Questionnaire, questionnaire);
        }

        [Test]
        public void Test_On_Get_Wrong()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255
            };
            var exception = new AccessViolationException();
            //Mock
            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Throws(exception);

            var actualResp = new QuestionnaireService().OnGet(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.GetType(), typeof(QuestionnaireResponse));
            Assert.AreEqual(((QuestionnaireResponse)actualResp).Succeeded, false);
            Assert.AreNotEqual(((QuestionnaireResponse)actualResp).Errors, null);
        }
    }
}
