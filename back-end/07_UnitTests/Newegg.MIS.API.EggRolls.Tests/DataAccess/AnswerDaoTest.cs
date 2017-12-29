using System.Collections;
using System.Collections.Generic;
using Newegg.MIS.API.EggRolls.DataAccess;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.DataAccess;
using Newegg.MIS.API.Utilities.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace Newegg.MIS.API.EggRolls.Tests.DataAccess
{
    [TestFixture]
    public class AnswerDaoTest
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
        public void Test_Add()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Answer_Add_Bunch");
            AnswerSheetRequest request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "MIS",
                ShortName = "sd52",
                FullName = "Sd.Red.Ya",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        QuestionnaireID = 255,
                        TopicID = 1,
                        Ans = "A"
                    },
                    new Answer
                    {
                        QuestionnaireID = 255,
                        TopicID = 1,
                        Ans = "B"
                    },
                    new Answer
                    {
                        QuestionnaireID = 255,
                        TopicID = 2,
                        Ans = "B"
                    }
                }

            };
            var country = "CN";
            var area = "CD";
            var supportCenter = "ENSC";
            var answerListSerialize = SerializationHelper.Serialize(request.AnswerList, null);

            AnswerDao.Instance.Add(request, country, area, supportCenter);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", request.QuestionnaireID);
            cmd.Received(1).SetParameterValue("@AnswerListSerialize", answerListSerialize);
            cmd.Received(1).SetParameterValue("@Department", request.Department);
            cmd.Received(1).SetParameterValue("@ShortName", request.ShortName);
            cmd.Received(1).SetParameterValue("@SupportCenter", supportCenter);
            cmd.Received(1).SetParameterValue("@FullName", request.FullName);
            cmd.Received(1).SetParameterValue("@Country", country);
            cmd.Received(1).SetParameterValue("@Area", area);
        }

        [Test]
        public void Test_Delete()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Answer_Delete");
            AnswerSheetRequest request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "MIS",
                ShortName = "sd52",
                FullName = "Sd.Red.Ya",
                AnswerList = new List<Answer>()
            };

            AnswerDao.Instance.Delete(request.QuestionnaireID);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", request.QuestionnaireID);

        }

        [Test]
        public void test_Query_Answer_sheeet()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Answer_Query");
            var exceptResp = new List<Answer>();

            cmd.ExecuteEntityList<Answer>()
                .Returns(exceptResp);
            AnswerSheetRequest request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "MIS",
                ShortName = "sd52",
                FullName = "Sd.Red.Ya",
                AnswerList = new List<Answer>()

            };

            var actualResp = AnswerDao.Instance.Query(request.QuestionnaireID, request.ShortName);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", request.QuestionnaireID);
            cmd.Received(1).SetParameterValue("@ShortName", request.ShortName);

            Assert.AreEqual(exceptResp, actualResp);
        }

        [Test]
        public void test_Query_Topic_Participator()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Answer_Query_All_Answer_Of_A_Topic");
            var exceptResp = new List<Answer>();

            cmd.ExecuteEntityList<Answer>()
                .Returns(exceptResp);

            var actualResp = AnswerDao.Instance.Query(12, 2);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", 12);
            cmd.Received(1).SetParameterValue("@TopicID", 2);

            Assert.AreEqual(exceptResp, actualResp);
        }

        [Test]
        public void test_Query_Option_Participator()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Answer_Query_All_Answer_Of_A_Option");
            var exceptResp = new List<Answer>();
            cmd.ExecuteEntityList<Answer>()
                .Returns(exceptResp);

            var actualResp = AnswerDao.Instance.Query(12, 2, "A");

            cmd.Received(1).SetParameterValue("@QuestionnaireID", 12);
            cmd.Received(1).SetParameterValue("@TopicID", 2);
            cmd.Received(1).SetParameterValue("@OptionID", "A");

            Assert.AreEqual(exceptResp, actualResp);
        }

        [Test]
        public void test_Query_Questionnaire_Participator()
        {
            var cmd = DataCommandFactory.Get("EggRolls_Common_Questionnaire_Participator");
            var exceptResp = new List<Participator>();

            cmd.ExecuteEntityList<Participator>()
                .Returns(exceptResp);

            var actualResp = AnswerDao.Instance.QueryParticipator(12);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", 12);

            Assert.AreEqual(exceptResp, actualResp);
        }

        [Test]
        public void test_Statistics_Answer_Sheet()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_Answer_Sheet_Statistics");
            var participator = new List<ParticipatorStatistics>();
            var department = new List<AnswerStatistics>();
            var exceptResp = new List<IList>
            {
                participator,department
            };

            var reader = Substitute.For<IGridReader>();

            reader.Read<ParticipatorStatistics>()
                .Returns(participator);

            reader.Read<AnswerStatistics>()
                .Returns(department);

            cmd.ExecuteMultiple()
                .Returns(reader);

            var actualResp = AnswerDao.Instance.Statistics(12);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", 12);

            Assert.AreEqual(exceptResp, actualResp);
        }

        [Test]
        public void test_Query_Existed()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_Answer_Sheet_Exists");

            cmd.ExecuteScalar<int>()
                .Returns(1);

            var actualResp = AnswerDao.Instance.QueryExisted(255,"Loo");

            cmd.Received(1).SetParameterValue("@QuestionnaireID", 255);
            cmd.Received(1).SetParameterValue("@ShortName", "Loo");

            Assert.AreEqual(actualResp, true);
        }
    }
}
