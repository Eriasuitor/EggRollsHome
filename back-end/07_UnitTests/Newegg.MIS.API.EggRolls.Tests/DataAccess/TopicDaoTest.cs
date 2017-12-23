using System;
using System.Collections.Generic;
using Newegg.MIS.API.EggRolls.DataAccess;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.DataAccess;
using Newegg.MIS.API.Utilities.Helpers;
using NUnit.Framework;
using NSubstitute;

namespace Newegg.MIS.API.EggRolls.Tests.DataAccess
{
    [TestFixture]
    public class TopicDaoTest
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
            var cmd = DataCommandFactory.Get("EggRolls_Common_Topic_Add_Bunch");
            var request = new QuestionnaireRequest()
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = 0,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = DateTime.Now,
                Participants = 0,
                Topics = new List<Topic>()
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>()
                        {
                            new Option()
                            {
                                OptionTitle = "option1",
                                OptionID = "A"
                            }
                        }
                    }
                }
            };
            var topicListSerialize = SerializationHelper.Serialize(request.Topics, null);

            TopicDao.Instance.Add(request);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", request.QuestionnaireID);
            cmd.Received(1).SetParameterValue("@ShortName", request.ShortName);
            cmd.Received(1).SetParameterValue("@TopicListSerialize", topicListSerialize);
            cmd.Received(1).ExecuteNonQuery();
        }

        [Test]
        public void Test_Delete()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_DeleteTopic_Only");
            var questionnaireID = 255;

            cmd.ExecuteNonQuery().Returns(5);

            var actual = TopicDao.Instance.Delete(questionnaireID);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", questionnaireID);
            cmd.Received(1).ExecuteNonQuery();

            Assert.AreEqual(actual, 5);
        }

        [Test]
        public void Test_Query()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_QueryTopic");
            var questionnaireID = 255;
            var topicList = new List<Topic>();
            cmd.ExecuteEntityList<Topic>()
                .Returns(topicList);

            var actual = TopicDao.Instance.Query(questionnaireID);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", questionnaireID);
            Assert.AreNotEqual(actual, null);
            Assert.AreEqual(actual, topicList);
            Assert.AreEqual(actual.Count, 0);
        }
    }
}
