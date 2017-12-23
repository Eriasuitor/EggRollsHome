using System;
using System.Collections.Generic;
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
    public class AnswerPaticipatorServiceTest
    {

        [SetUp]
        public void Setup()
        {
            var answerSheetBusiness = Substitute.For<IAnswerSheetBusiness>();
            InstanceManager.Register<IAnswerSheetBusiness>().Use(answerSheetBusiness);
        }

        [TearDown]
        public void Clearup()
        {
            InstanceManager.Register<IAnswerSheetBusiness>().Reset();
        }

        [Test]
        public void Test_Get_Right()
        {
            var answers = new List<Answer>();

            AnswerSheetBusiness.Instance
                .Query(1, 1, "A")
                .Returns(answers);

            var actualResp = new AnswerParticipatorService().OnGet(new AnswerParticipatorRequest
            {
                QuestionnaireID = 1,
                TopicID = 1,
                OptionID = "A"
            });

            Assert.AreEqual(actualResp.GetType(), typeof(AnswerParticipatorResponse));
            Assert.AreEqual(actualResp.TranslateTo<AnswerParticipatorResponse>().Answers, answers);
        }

        [Test]
        public void Test_Get_Wrong()
        {
            AnswerSheetBusiness.Instance
                .Query(1, 1, "A")
                .Throws(new ApplicationException());

            var actualResp = new AnswerParticipatorService().OnGet(new AnswerParticipatorRequest
            {
                QuestionnaireID = 1,
                TopicID = 1,
                OptionID = "A"
            });
            
            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerParticipatorResponse>().Succeeded, false);
            Assert.AreNotEqual(actualResp.TranslateTo<AnswerParticipatorResponse>().Errors, null);
        }
    }
}
