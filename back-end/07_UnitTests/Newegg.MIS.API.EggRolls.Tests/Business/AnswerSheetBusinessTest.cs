using System;
using System.Collections.Generic;
using Newegg.API.Common;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.DataAccess;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.Utilities.Helpers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Newegg.MIS.API.EggRolls.Tests.Business
{
    [TestFixture]
    public class AnswerSheetBusinessTest
    {
        [SetUp]
        public void Setup()
        {
            var questionnaireDao = Substitute.For<IQuestionnaireDao>();
            var topicDao = Substitute.For<ITopicDao>();
            var optionDao = Substitute.For<IOptionDao>();
            var answerDao = Substitute.For<IAnswerSheetDao>();
            InstanceManager.Register<IQuestionnaireDao>().Use(questionnaireDao);
            InstanceManager.Register<ITopicDao>().Use(topicDao);
            InstanceManager.Register<IOptionDao>().Use(optionDao);
            InstanceManager.Register<IAnswerSheetDao>().Use(answerDao);
        }

        [TearDown]
        public void Clearup()
        {
            InstanceManager.Register<IQuestionnaireDao>().Reset();
            InstanceManager.Register<ITopicDao>().Reset();
            InstanceManager.Register<IOptionDao>().Reset();
            InstanceManager.Register<IAnswerSheetDao>().Reset();
        }

        [Test]
        public void Test_Add_Right()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS"
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Status = QuestionnaireStatus.Processing
            };

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);
            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            AnswerSheetBusiness.Instance.Add(request);

            AnswerDao.Instance.Received(1).Add(request, "CN", "CD", "NESC");
        }

        [Test]
        public void Test_Add_Not_Found()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS"
            };

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Throws(new ApplicationException());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Existed()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS"
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Status = QuestionnaireStatus.Processing
            };

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);
            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>
                {
                    new Answer()
                });

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Is_Draft()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS"
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Status = QuestionnaireStatus.Draft
            };

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Is_Ended()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS"
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Status = QuestionnaireStatus.Ended
            };

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Wrong_Department()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC"
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Status = QuestionnaireStatus.Processing
            };

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);
            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Query_Found()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                ShortName = "sd45"
            };
            var questionnaire = new Questionnaire
            {
                QuestionnaireID = 255
            };
            var topics = new List<Topic>()
            {
                new Topic()
                {
                    TopicID = 1,
                    TopicTitle = "Topic 1's title"
                }
            };
            var options = new List<Option>()
            {
                new Option
                {
                    TopicID = 1,
                    OptionID = "A",
                    OptionTitle = "Option A's title"
                },
                new Option
                {
                    TopicID = 1,
                    OptionID = "B",
                    OptionTitle = "Option B's title"
                }
            };
            var answers = new List<Answer>
            {
                new Answer
                {
                    TopicID = 1,
                    Ans = "A"
                }
            };

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);
            TopicDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(topics);
            OptionDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(options);
            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(answers);

            var actualResp = AnswerSheetBusiness.Instance.Query(request);

            AnswerDao.Instance.Received(1).Query(request.QuestionnaireID, request.ShortName);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.QuestionnaireID, 255);
            Assert.AreEqual(actualResp.Topics.Count, 1);
            Assert.AreEqual(actualResp.Topics[0].Options.Count, 2);
            Assert.AreEqual(actualResp.Topics[0].Answers.Count, 1);
            Assert.AreEqual(actualResp.Topics[0].Answers[0].Ans, "A");
        }

        [Test]
        public void Test_Query_Questionnaire_Not_Found()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                ShortName = "sd45"
            };

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Throws(new ApplicationException());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Query(request));

            AnswerDao.Instance.Received(0).Query(request.QuestionnaireID, request.ShortName);
        }

        [Test]
        public void Test_Query_Anser_Not_Found()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                ShortName = "sd45"
            };
            var questionnaire = new Questionnaire
            {
                QuestionnaireID = 255
            };
            var topics = new List<Topic>()
            {
                new Topic()
                {
                    TopicID = 1,
                    TopicTitle = "Topic 1's title"
                }
            };
            var options = new List<Option>()
            {
                new Option
                {
                    TopicID = 1,
                    OptionID = "A",
                    OptionTitle = "Option A's title"
                },
                new Option
                {
                    TopicID = 1,
                    OptionID = "B",
                    OptionTitle = "Option B's title"
                }
            };
            var answers = new List<Answer>();

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);
            TopicDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(topics);
            OptionDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(options);
            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(answers);

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Query(request));

            AnswerDao.Instance.Received(1).Query(request.QuestionnaireID, request.ShortName);
        }

        [Test]
        public void Test_Query_Topic_Participator()
        {
            var expectResp = new List<Answer>();

            AnswerDao.Instance.Query(3, 4)
                .Returns(expectResp);

            var actualResp = AnswerSheetBusiness.Instance.Query(3, 4);

            Assert.AreEqual(actualResp, expectResp);
        }

        [Test]
        public void Test_Query_Topic_Participator_By_Option_Method()
        {
            var expectResp = new List<Answer>();

            AnswerDao.Instance.Query(3, 4)
                .Returns(expectResp);

            var actualResp = AnswerSheetBusiness.Instance.Query(3, 4, null);

            Assert.AreEqual(actualResp, expectResp);
        }

        [Test]
        public void Test_Query_Option_Participator()
        {
            var expectResp = new List<Answer>();

            AnswerDao.Instance.Query(3, 4, "A")
                .Returns(expectResp);

            var actualResp = AnswerSheetBusiness.Instance.Query(3, 4, "A");

            Assert.AreEqual(actualResp, expectResp);
        }

        [Test]
        public void Test_Query_Questionnaire_Participator()
        {
            var expectResp = new List<Participator>();

            AnswerDao.Instance.QueryParticipator(3)
                .Returns(expectResp);

            var actualResp = AnswerSheetBusiness.Instance.QueryParticipator(3);

            Assert.AreEqual(actualResp, expectResp);
        }

        [Test]
        public void Test_Query_Answer_Sheet_Statistics_Found()
        {
            var expectResp = new AnswerSheet();

            var optionList = new List<Option>
            {
                new Option
                {
                    TopicID = 1,
                    OptionID = "A"
                },
                new Option()
                {
                    TopicID = 1,
                    OptionID = "B"
                }
            };
            var topicList = new List<Topic>
            {
                new Topic
                {
                    TopicID = 1,
                }
            };
            var questionnaire = new Questionnaire
            {
                QuestionnaireID = 255,
                Participants = 12,
            };
            var unitsList = new List<Units>
            {
                new Units
                {
                    TopicID = 1,
                    ChosenNumber = 2,
                    Answer = "A",
                    Department = "MIS"
                },
                new Units
                {
                    TopicID = 1,
                    ChosenNumber = 10,
                    Answer = "A",
                    Department = "HR"
                },
                new Units
                {
                    TopicID = 1,
                    ChosenNumber = 5,
                    Answer = "B",
                    Department = "HR"
                },
            };

            QuestionnaireDao.Instance
                .Query(255)
                .Returns(questionnaire);
            TopicDao.Instance
                .Query(255)
                .Returns(topicList);
            OptionDao.Instance
                .Query(255)
                .Returns(optionList);
            AnswerDao.Instance
                .Statistics(255)
                .Returns(unitsList);

            var actualResp = AnswerSheetBusiness.Instance.Statistics(255);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerSheet>().QuestionnaireID, 255);
            Assert.AreEqual(actualResp.Topics.Count, 1);
            Assert.AreEqual(actualResp.Topics[0].Options.Count, 2);
            Assert.AreEqual(actualResp.Topics[0].Options[0].ChosenNumber, 12);
            Assert.AreEqual(actualResp.Topics[0].Options[0].PersonalUnits, "100.00%");
            Assert.AreEqual(actualResp.Topics[0].Options[0].DepartmentUnits.Count, 2);
            Assert.AreEqual(actualResp.Topics[0].Options[0].DepartmentUnits[0].ChosenNumber, 2);
            Assert.AreEqual(actualResp.Topics[0].Options[0].DepartmentUnits[0].Percentage, "16.67%");
            Assert.AreEqual(actualResp.Topics[0].Options[0].DepartmentUnits[1].ChosenNumber, 10);
            Assert.AreEqual(actualResp.Topics[0].Options[0].DepartmentUnits[1].Percentage, "83.33%");
            Assert.AreEqual(actualResp.Topics[0].Options[1].ChosenNumber, 5);
            Assert.AreEqual(actualResp.Topics[0].Options[1].PersonalUnits, "41.67%");
            Assert.AreEqual(actualResp.Topics[0].Options[1].DepartmentUnits.Count, 2);
            Assert.AreEqual(actualResp.Topics[0].Options[1].DepartmentUnits[0].ChosenNumber, 0);
            Assert.AreEqual(actualResp.Topics[0].Options[1].DepartmentUnits[0].Percentage, "0.00%");
            Assert.AreEqual(actualResp.Topics[0].Options[1].DepartmentUnits[1].ChosenNumber, 5);
            Assert.AreEqual(actualResp.Topics[0].Options[1].DepartmentUnits[1].Percentage, "100.00%");
        }

        [Test]
        public void Test_Query_Answer_Sheet_Statistics_Found_But_No_Participator()
        {
            var expectResp = new AnswerSheet();

            var optionList = new List<Option>
            {
                new Option
                {
                    TopicID = 1,
                    OptionID = "A"
                },
                new Option()
                {
                    TopicID = 1,
                    OptionID = "B"
                }
            };
            var topicList = new List<Topic>
            {
                new Topic
                {
                    TopicID = 1,
                }
            };
            var questionnaire = new Questionnaire
            {
                QuestionnaireID = 255,
                Participants = 0
            };

            QuestionnaireDao.Instance
                .Query(255)
                .Returns(questionnaire);
            TopicDao.Instance
                .Query(255)
                .Returns(topicList);
            OptionDao.Instance
                .Query(255)
                .Returns(optionList);

            var actualResp = AnswerSheetBusiness.Instance.Statistics(255);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.TranslateTo<AnswerSheet>().QuestionnaireID, 255);
            Assert.AreEqual(actualResp.Topics.Count, 1);
            Assert.AreEqual(actualResp.Topics[0].Options.Count, 2);
            Assert.AreEqual(actualResp.Topics[0].Options[0].ChosenNumber, 0);
            Assert.AreEqual(actualResp.Topics[0].Options[0].PersonalUnits, "0.00%");
            Assert.AreEqual(actualResp.Topics[0].Options[0].DepartmentUnits, null);
            Assert.AreEqual(actualResp.Topics[0].Options[1].ChosenNumber, 0);
            Assert.AreEqual(actualResp.Topics[0].Options[1].PersonalUnits, "0.00%");
            Assert.AreEqual(actualResp.Topics[0].Options[1].DepartmentUnits, null);
        }

        [Test]
        public void Test_Query_Answer_Sheet_Statistics_Not_Found()
        {
            QuestionnaireDao.Instance.Query(3)
                .Returns((Questionnaire)null);

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Statistics(3));
        }

    }
}
