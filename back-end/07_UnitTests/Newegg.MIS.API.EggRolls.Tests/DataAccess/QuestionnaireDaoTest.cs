using System.Collections.Generic;
using Newegg.MIS.API.EggRolls.DataAccess;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.DataAccess;
using NUnit.Framework;
using NSubstitute;

namespace Newegg.MIS.API.EggRolls.Tests.DataAccess
{
    [TestFixture]
    public class QuestionnaireDaoTest
    {
        [SetUp]
        public void Setup()
        {
            //mock command
            var cmd = Substitute.For<IDataCommand>();

            DataCommandFactory.SetDefaultCommand(cmd);

            cmd.SetParameterValue(Arg.Any<string>(), Arg.Any<object>())
                .Returns(cmd);
        }

        [TearDown]
        public void Clearup()
        {
            DataCommandFactory.Reset();
        }

        [Test]
        public void Test_Search_OK_But_No_Records()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Questionnaire_QueryByTitle_WithPaging");

            var reader = Substitute.For<IGridReader>();
            //no records
            reader.Read<int>()
                .Returns(new List<int> { 0 });

            //questionnaire
            reader.Read<Questionnaire>()
                .Returns(new List<Questionnaire>());

            cmd.ExecuteMultiple()
                .Returns(reader);

            var request = new QuestionnaireSearchRequest()
            {
                Title = "~!@@",
                ShortName = "tr29",
                PageIndex = 0,
                PageSize = 10
            };

            var actual = QuestionnaireDao.Instance.Search(request);

            cmd.Received(1).SetParameterValue("@ShortName", request.ShortName);
            cmd.Received(1).SetParameterValue("@Title", request.Title);
            cmd.Received(1).SetParameterValue("@StartPageIndex", request.PageIndex);
            cmd.Received(1).SetParameterValue("@EndPageIndex", request.PageIndex);
            cmd.Received(1).SetParameterValue("@PageSize", request.PageSize);

            Assert.AreNotEqual(actual, null);
            Assert.AreEqual(actual.PageIndex, 0);
            Assert.AreEqual(actual.PageSize, 10);
            Assert.AreEqual(actual.Pages, 0);
            Assert.AreNotEqual(actual.Questionnaires, null);
            Assert.AreEqual(actual.Questionnaires.Count, 0);
        }

        [Test]
        public void Test_Search_OK_With_Records()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Questionnaire_QueryByTitle_WithPaging");

            var reader = Substitute.For<IGridReader>();
            //no records
            reader.Read<int>()
                .Returns(new List<int> { 2 });

            //questionnaire
            reader.Read<Questionnaire>()
                .Returns(new List<Questionnaire>() { new Questionnaire(), new Questionnaire() });

            cmd.ExecuteMultiple()
                .Returns(reader);

            var request = new QuestionnaireSearchRequest()
            {
                Title = "~!@@",
                ShortName = "tr29",
                PageIndex = 0,
                PageSize = 10
            };

            var actual = QuestionnaireDao.Instance.Search(request);

            cmd.Received(1).SetParameterValue("@ShortName", request.ShortName);
            cmd.Received(1).SetParameterValue("@Title", request.Title);
            cmd.Received(1).SetParameterValue("@StartPageIndex", request.PageIndex);
            cmd.Received(1).SetParameterValue("@EndPageIndex", request.PageIndex);
            cmd.Received(1).SetParameterValue("@PageSize", request.PageSize);

            Assert.AreNotEqual(actual, null);
            Assert.AreEqual(actual.PageIndex, 0);
            Assert.AreEqual(actual.PageSize, 10);
            Assert.AreEqual(actual.Pages, 1);
            Assert.AreNotEqual(actual.Questionnaires, null);
            Assert.AreEqual(actual.Questionnaires.Count, 2);
        }

        [Test]
        public void Test_Add()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Questionnaire_Creation");
            var request = new QuestionnaireRequest()
            {
                Title = "~!@@",
                ShortName = "tr29"
            };

            cmd.GetParameterValue("QuestionnaireID")
                .Returns(255);

            var actual = QuestionnaireDao.Instance.Add(request);

            cmd.Received(1).SetParameterValue("@ShortName", request.ShortName);
            cmd.Received(1).SetParameterValue("@FullName", request.FullName);
            cmd.Received(1).SetParameterValue("@Status", request.Status);
            cmd.Received(1).SetParameterValue("@Title", request.Title);
            cmd.Received(1).SetParameterValue("@Description", request.Description);
            cmd.Received(1).SetParameterValue("@BackgroundImageUrl", request.BackgroundImageUrl);
            cmd.Received(1).SetParameterValue("@IsRealName", request.IsRealName);
            cmd.Received(1).SetParameterValue("@DueDate", request.DueDate);
            cmd.Received(1).ExecuteNonQuery();

            Assert.AreEqual(actual, 255);
        }

        [Test]
        public void Test_Delete()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_DeleteQuestionnaire_Only");
            var questionnaireID = 255;

            cmd.ExecuteNonQuery().Returns(1);

            var actual = QuestionnaireDao.Instance.Delete(questionnaireID);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", questionnaireID);
            cmd.Received(1).ExecuteNonQuery();

            Assert.AreEqual(actual, 1);
        }

        [Test]
        public void Test_Update()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Questionnaire_Update");
            var request = new QuestionnaireRequest()
            {
                QuestionnaireID = 255,
                ShortName = "tr29",
                Title = "123"
            };
            cmd.ExecuteNonQuery()
                .Returns(1);

            var actual = QuestionnaireDao.Instance.Update(request);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", request.QuestionnaireID);
            cmd.Received(1).SetParameterValue("@ShortName", request.ShortName);
            cmd.Received(1).SetParameterValue("@FullName", request.FullName);
            cmd.Received(1).SetParameterValue("@Status", request.Status);
            cmd.Received(1).SetParameterValue("@Title", request.Title);
            cmd.Received(1).SetParameterValue("@Description", request.Description);
            cmd.Received(1).SetParameterValue("@BackgroundImageUrl", request.BackgroundImageUrl);
            cmd.Received(1).SetParameterValue("@IsRealName", request.IsRealName);
            cmd.Received(1).SetParameterValue("@DueDate", request.DueDate);
            cmd.Received(1).ExecuteNonQuery();

            Assert.AreEqual(actual, 1);
        }

        [Test]
        public void Test_Query()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_QueryQuestionnaire");
            var questionnaireID = 255;
            var questionnaire = new Questionnaire();
            cmd.ExecuteEntity<Questionnaire>()
                .Returns(questionnaire);

            var actual = QuestionnaireDao.Instance.Query(questionnaireID);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", questionnaireID);
            Assert.AreNotEqual(actual, null);
            Assert.AreEqual(actual, questionnaire);
        }

        [Test]
        public void Test_Query_No_Records()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_QueryQuestionnaire");
            var questionnaireID = 255;
            Questionnaire questionnaire = null;
            cmd.ExecuteEntity<Questionnaire>()
                .Returns(questionnaire);

            var actual = QuestionnaireDao.Instance.Query(questionnaireID);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", questionnaireID);
            Assert.AreEqual(actual, null);
        }
    }
}
