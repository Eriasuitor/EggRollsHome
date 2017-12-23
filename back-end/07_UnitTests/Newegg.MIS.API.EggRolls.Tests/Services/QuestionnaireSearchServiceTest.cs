using System;
using Newegg.API.Common;
using Newegg.MIS.API.EggRolls.Business;
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
    public class QuestionnaireSearchServiceTest
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
        public void Test_On_Get()
        {
            var request = new QuestionnaireSearchRequest
            {
                ShortName = "tr29",
                Title = "123"
            };

            var expectedResp = new QuestionnaireSearchResponse
            {
                Questionnaires = null
            };

            //Mock
            QuestionnaireBusiness.Instance
                .Search(request)
                .Returns(expectedResp);

            var actualResp = new QuestionnaireSearchService().OnGet(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp, expectedResp);
            Assert.AreEqual(actualResp.TranslateTo<QuestionnaireSearchResponse>().Questionnaires, null);
        }

        [Test]
        public void Test_On_Get_Wrong()
        {
            var request = new QuestionnaireSearchRequest
            {
                ShortName = "tr29",
                Title = "123"
            };
            var exception = new ApplicationException();
            //Mock
            QuestionnaireBusiness.Instance
                .Search(request)
                .Throws(exception);

            var actualResp = new QuestionnaireSearchService().OnGet(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.GetType(), typeof(QuestionnaireSearchResponse));
            Assert.AreEqual(((QuestionnaireSearchResponse)actualResp).Succeeded, false);
            Assert.AreNotEqual(((QuestionnaireSearchResponse)actualResp).Errors, null);
        }
    }
}
