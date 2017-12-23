using System;
using Newegg.API.Common;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.EggRolls.Services;
using Newegg.MIS.API.Utilities.Helpers;
using NUnit.Framework;
using NSubstitute;

namespace Newegg.MIS.API.EggRolls.Tests.Services
{
    [TestFixture]
    public class AnswerSheetServiceTest
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
        public void Test_On_Post_Right()
        {
            var request = new AnswerSheetRequest();

            var actualResp = new AnswerSheetService().OnPost(request);

            Assert.AreEqual(actualResp.GetType(), typeof(AnswerParticipatorResponse));
            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerParticipatorResponse>().Succeeded, true);
        }

        [Test]
        public void Test_On_Post_Wrong()
        {
            var request = new AnswerSheetRequest();

            AnswerSheetBusiness.Instance
                .When(x => x.Add(request))
                .Do(x => throw new ApplicationException());

            var actualResp = new AnswerSheetService().OnPost(request);

            Assert.AreEqual(actualResp.GetType(), typeof(AnswerParticipatorResponse));
            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerParticipatorResponse>().Succeeded, false);
            Assert.AreNotEqual(((AnswerParticipatorResponse)actualResp).Errors, null);
        }

        [Test]
        public void Test_On_Get_Right()
        {
            var request = new AnswerSheetRequest();

            var actualResp = new AnswerSheetService().OnGet(request);

            Assert.AreEqual(actualResp.GetType(), typeof(AnswerSheetResponse));
            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerSheetResponse>().Succeeded, true);
        }

        [Test]
        public void Test_On_Get_Wrong()
        {
            var request = new AnswerSheetRequest();

            AnswerSheetBusiness.Instance
                .When(x => x.Query(request))
                .Do(x => throw new ApplicationException());

            var actualResp = new AnswerSheetService().OnGet(request);

            Assert.AreEqual(actualResp.GetType(), typeof(AnswerSheetResponse));
            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerParticipatorResponse>().Succeeded, false);
            Assert.AreNotEqual(((AnswerSheetResponse)actualResp).Errors, null);
        }
    }
}