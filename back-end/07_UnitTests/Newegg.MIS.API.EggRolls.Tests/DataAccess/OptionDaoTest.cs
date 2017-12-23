using System;
using System.Collections.Generic;
using System.Linq;
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
    public class OptionDaoTest
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
            var cmd = DataCommandFactory.Get("EggRolls_Common_Option_Add_Bunch");
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
            var optionList = request.Topics.SelectMany(topic => topic.Options).ToList();
            var optionListSerialize = SerializationHelper.Serialize(optionList, null);

            OptionDao.Instance.Add(request);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", request.QuestionnaireID);
            cmd.Received(1).SetParameterValue("@ShortName", request.ShortName);
            cmd.Received(1).SetParameterValue("@OptionListSerialize", optionListSerialize);
            cmd.Received(1).ExecuteNonQuery();
        }

        [Test]
        public void Test_Delete()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_DeleteOption_Only");
            var questionnaireID = 255;

            cmd.ExecuteNonQuery().Returns(5);

            var actual = OptionDao.Instance.Delete(questionnaireID);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", questionnaireID);
            cmd.Received(1).ExecuteNonQuery();

            Assert.AreEqual(actual, 5);
        }

        [Test]
        public void Test_Query()
        {
            var cmd = DataCommandFactory.Get("MIS_EggRolls_QueryOption");
            var questionnaireID = 255;
            var optionList = new List<Option>();
            cmd.ExecuteEntityList<Option>()
                .Returns(optionList);

            var actual = OptionDao.Instance.Query(questionnaireID);

            cmd.Received(1).SetParameterValue("@QuestionnaireID", questionnaireID);
            Assert.AreNotEqual(actual, null);
            Assert.AreEqual(actual, optionList);
            Assert.AreEqual(actual.Count, 0);
        }
    }
}
