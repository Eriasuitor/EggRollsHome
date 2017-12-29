using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newegg.API.Common;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.EggRolls.Services;
using Newegg.MIS.API.Utilities.Helpers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Newegg.MIS.API.EggRolls.Tests.Services
{
    [TestFixture]
    public class QuestionnaireParticipatorServiceTest
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
        public void Test_Get_Right()
        {
            var participatorList = new List<Participator>();

            QuestionnaireBusiness.Instance
                .QueryParticipator(1)
                .Returns(participatorList);

            var actualResp = new QuestionnaireParticipatorService().OnGet(new QuestionnaireParticipatorRequest
            {
                QuestionnaireID = 1
            });

            Assert.AreEqual(actualResp.TranslateTo<QuestionnaireParticipatorResponse>().Participators, participatorList);
        }

        [Test]
        public void Test_Get_Wrong()
        {
            QuestionnaireBusiness.Instance
                .QueryParticipator(1)
                .Throws(new ApplicationException());

            var actualResp = new QuestionnaireParticipatorService().OnGet(new QuestionnaireParticipatorRequest
            {
                QuestionnaireID = 1
            });

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerParticipatorResponse>().Succeeded, false);
            Assert.AreNotEqual(actualResp.TranslateTo<AnswerParticipatorResponse>().Errors, null);
        }
    }
}
