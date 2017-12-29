using NUnit.Framework;
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

namespace Newegg.MIS.API.EggRolls.Tests.Services
{
    [TestFixture]
    public class AnswerSheetStatisticsServiceTest
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
            var paticipatorList = new List<ParticipatorStatistics>();

            AnswerSheetBusiness.Instance
                .Statistics(1)
                .Returns(paticipatorList);

            var actualResp = new AnswerSheetStatisticsService().OnGet(new AnswerSheetStatisticsRequest
            {
                QuestionnaireID = 1
            });

            Assert.AreEqual(actualResp.TranslateTo<AnswerSheetStatisticsResponse>().ParticipatorStatisticsList, paticipatorList);
        }

        [Test]
        public void Test_Get_Wrong()
        {
            AnswerSheetBusiness.Instance
                .Statistics(1)
                .Throws(new ApplicationException());

            var actualResp = new AnswerSheetStatisticsService().OnGet(new AnswerSheetStatisticsRequest
            {
                QuestionnaireID = 1
            });

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerSheetStatisticsResponse>().Succeeded, false);
            Assert.AreNotEqual(actualResp.TranslateTo<AnswerSheetStatisticsResponse>().Errors, null);
        }
    }
}
