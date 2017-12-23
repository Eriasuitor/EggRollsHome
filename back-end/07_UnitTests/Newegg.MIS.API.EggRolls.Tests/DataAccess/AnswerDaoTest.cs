using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        Options = new List<Option>(),
                        Answers = new List<Answer>()
                    }
                }

            };
            var country = "CN";
            var area = "CD";
            var supportCenter = "ENSC";
            var answerList = request.Topics.SelectMany(topic => topic.Answers).ToList();
            var answerListSerialize = SerializationHelper.Serialize(answerList, null);

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
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        Options = new List<Option>(),
                        Answers = new List<Answer>()
                    }
                }

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
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        Options = new List<Option>(),
                        Answers = new List<Answer>()
                    }
                }

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
            var exceptResp = new List<Units>();

            cmd.ExecuteEntityList<Units>()
                .Returns(exceptResp);

            var actualResp = AnswerDao.Instance.Statistics(12);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", 12);

            Assert.AreEqual(exceptResp, actualResp);
        }
    }
}
