using System;
using System.Collections.Generic;
using Newegg.FrameworkAPI.SDK.Mail;
using Newegg.MIS.API.EggRolls.Business;
using Newegg.MIS.API.EggRolls.DataAccess;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.EggRolls.RequestEntities;
using Newegg.MIS.API.EggRolls.ResponseEntities;
using Newegg.MIS.API.Utilities.Helpers;
using NUnit.Framework;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Newegg.MIS.API.EggRolls.Tests.Business
{
    [TestFixture]
    public class QuestionnaireBusinessTest
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
        public void Test_Search_Questionnaire()
        {
            var request = new QuestionnaireSearchRequest()
            {
                ShortName = "tr29",
                Title = "123"
            };

            var expectedResp = new QuestionnaireSearchResponse()
            {
                Questionnaires = null
            };

            //Mock
            QuestionnaireDao.Instance
                .Search(request)
                .Returns(expectedResp);

            var actualResp = QuestionnaireBusiness.Instance.Search(request);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp, expectedResp);
            Assert.AreEqual(actualResp.Questionnaires, null);
        }

        [Test]
        public void Test_Add_Questionnaire()
        {
            var request = new QuestionnaireRequest()
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = QuestionnaireStatus.Draft,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = DateTime.Now.AddDays(1),
                BackgroundImageUrl = "123",
                MailTo = "Lory.Y.j@newegg.com",
                Participants = 0,
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>
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

            //Mock
            QuestionnaireDao.Instance
                .Add(request)
                .Returns(154);
            var mailSenderHelper = Substitute.For<IMailSenderHelper>();
            InstanceManager.Register<IMailSenderHelper>().Use(mailSenderHelper);

            MailSenderHelper.Instance.Send(Arg.Any<MailRequest>())
                .Returns(new MailSendResult()
                {
                    IsSendSuccess = true
                });

            var actualResp = QuestionnaireBusiness.Instance.Add(request);

            Assert.AreEqual(actualResp.Questionnaire.QuestionnaireID, 154);
            Assert.AreEqual(actualResp.MailSucceeded, true);

            QuestionnaireDao.Instance.Received(1).Add(request);
            TopicDao.Instance.Received(1).Add(request);
            OptionDao.Instance.Received(1).Add(request);
        }

        [Test]
        public void Test_Add_Questionnaire_But_Main_Wrong()
        {
            var request = new QuestionnaireRequest()
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = QuestionnaireStatus.Draft,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = DateTime.Now.AddDays(1),
                BackgroundImageUrl = "123",
                MailTo = "Lory.Y.j@newegg.com",
                Participants = 0,
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>
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

            //Mock
            QuestionnaireDao.Instance
                .Add(request)
                .Returns(154);
            var mailSenderHelper = Substitute.For<IMailSenderHelper>();
            InstanceManager.Register<IMailSenderHelper>().Use(mailSenderHelper);

            MailSenderHelper.Instance.Send(Arg.Any<MailRequest>())
                .Throws(new ApplicationException());

            var actualResp = QuestionnaireBusiness.Instance.Add(request);

            Assert.AreEqual(actualResp.Questionnaire.QuestionnaireID, 154);
            Assert.AreEqual(actualResp.MailSucceeded, false);

            QuestionnaireDao.Instance.Received(1).Add(request);
            TopicDao.Instance.Received(1).Add(request);
            OptionDao.Instance.Received(1).Add(request);
        }

        [Test]
        public void Test_Add_Questionnaire_Duedate_Arrived_But_Is_Draft()
        {
            var request = new QuestionnaireRequest
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = QuestionnaireStatus.Draft,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = DateTime.Now,
                Participants = 0,
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>
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

            //Mock
            QuestionnaireDao.Instance
                .Add(request)
                .Returns(154);

            var actualResp = QuestionnaireBusiness.Instance.Add(request);

            Assert.AreEqual(actualResp.Questionnaire.QuestionnaireID, 154);
            QuestionnaireDao.Instance.Received(1).Add(request);
            TopicDao.Instance.Received(1).Add(request);
            OptionDao.Instance.Received(1).Add(request);
        }

        [Test]
        public void Test_Add_Questionnaire_Duedate_Arrived()
        {
            var request = new QuestionnaireRequest()
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = QuestionnaireStatus.Processing,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = DateTime.Now,
                Participants = 0,
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>
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

            //Mock
            QuestionnaireDao.Instance
                .Add(request)
                .Returns(154);

            Assert.Throws<ApplicationException>(() => QuestionnaireBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Questionnaire_Duedate_Is_Null()
        {
            var request = new QuestionnaireRequest
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = QuestionnaireStatus.Processing,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = null,
                Participants = 0,
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>
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

            //Mock
            QuestionnaireDao.Instance
                .Add(request)
                .Returns(154);

            Assert.Throws<ApplicationException>(() => QuestionnaireBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Delete_Questionnaire()
        {
            var request = new QuestionnaireRequest()
            {
                QuestionnaireID = 255
            };

            //Mock
            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(new Questionnaire());
            QuestionnaireDao.Instance
                .Delete(request.QuestionnaireID)
                .Returns(1);

            var actualResp = QuestionnaireBusiness.Instance.Delete(request);

            Assert.AreEqual(actualResp, 1);
            QuestionnaireDao.Instance.Received(1).Delete(request.QuestionnaireID);
            TopicDao.Instance.Received(1).Delete(request.QuestionnaireID);
            OptionDao.Instance.Received(1).Delete(request.QuestionnaireID);
            AnswerDao.Instance.Received(1).Delete(request.QuestionnaireID);
        }

        [Test]
        public void Test_Delete_Questionnaire_Not_Found()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255
            };

            //Mock
            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns((Questionnaire)null);

            Assert.Throws<ApplicationException>(() => QuestionnaireBusiness.Instance.Delete(request));

            QuestionnaireDao.Instance.Received(0).Delete(request.QuestionnaireID);
        }

        [Test]
        public void Test_Update_Questionnaire()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255,
                ShortName = "tr29",
                Title = "123"
            };
            var questionnaire = new Questionnaire();

            //Mock
            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);
            QuestionnaireDao.Instance
                .Update(request)
                .Returns(1);

            var actualResp = QuestionnaireBusiness.Instance.Update(request);

            Assert.AreEqual(actualResp, 1);
            TopicDao.Instance.Received(1).Delete(request.QuestionnaireID);
            OptionDao.Instance.Received(1).Delete(request.QuestionnaireID);
            TopicDao.Instance.Received(1).Add(request);
            OptionDao.Instance.Received(1).Add(request);
        }

        [Test]
        public void Test_Update_Questionnaire_Not_Found()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255,
                ShortName = "tr29",
                Title = "123"
            };

            //Mock
            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns((Questionnaire)null);

            Assert.Throws<ApplicationException>(() => QuestionnaireBusiness.Instance.Update(request));

            QuestionnaireDao.Instance.Received(0).Update(request);
        }

        [Test]
        public void Test_Update_Questionnaire_Duedate_Arrived_But_Is_Draft()
        {
            var request = new QuestionnaireRequest
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = QuestionnaireStatus.Draft,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = DateTime.Now,
                Participants = 0,
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>
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

            //Mock
            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(new Questionnaire());
            QuestionnaireDao.Instance
                .Update(request)
                .Returns(1);

            var actualResp = QuestionnaireBusiness.Instance.Update(request);

            Assert.AreEqual(actualResp, 1);
            QuestionnaireDao.Instance.Received(1).Update(request);
            TopicDao.Instance.Received(1).Add(request);
            OptionDao.Instance.Received(1).Add(request);
            TopicDao.Instance.Received(1).Delete(request.QuestionnaireID);
            OptionDao.Instance.Received(1).Delete(request.QuestionnaireID);
        }

        [Test]
        public void Test_Update_Questionnaire_Duedate_Arrived()
        {
            var request = new QuestionnaireRequest
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = QuestionnaireStatus.Processing,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = DateTime.Now,
                Participants = 0,
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>
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

            //Mock
            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(new Questionnaire());

            Assert.Throws<ApplicationException>(() => QuestionnaireBusiness.Instance.Update(request));
        }

        [Test]
        public void Test_Update_Questionnaire_Duedate_Is_Null()
        {
            var request = new QuestionnaireRequest
            {
                ShortName = "tr29",
                Title = "123",
                FullName = "Se.E.As",
                Status = QuestionnaireStatus.Processing,
                Description = "<p>Description</p>",
                IsRealName = true,
                DueDate = null,
                Participants = 0,
                Topics = new List<Topic>
                {
                    new Topic()
                    {
                        TopicTitle = "Topic1",
                        TopicID = 1,
                        Options = new List<Option>
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

            //Mock
            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(new Questionnaire());

            Assert.Throws<ApplicationException>(() => QuestionnaireBusiness.Instance.Update(request));
        }

        [Test]
        public void Test_Query_Questionnaire_Found()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255
            };
            var questionnaire = new Questionnaire
            {
                QuestionnaireID = 255,
                Title = "Yes this is title.",
                Participants = 0
            };
            var topics = new List<Topic>
            {
                new Topic
                {
                    TopicID = 1,
                    TopicTitle = "This is topic 1"
                },
                new Topic
                {
                    TopicID = 2,
                    TopicTitle = "This is topic 2"
                },
                new Topic
                {
                    TopicID = 3,
                    TopicTitle = "This is topic 3"
                }
            };
            var options = new List<Option>
            {
                new Option
                {
                    TopicID = 1,
                    OptionID = "A",
                    OptionTitle = "This is option 1"
                },
                new Option
                {
                    TopicID = 1,
                    OptionID = "B",
                    OptionTitle = "This is option 2"
                },
                new Option
                {
                    TopicID = 4,
                    OptionID = "A",
                    OptionTitle = "This is option 1"
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

            var actualResp = QuestionnaireBusiness.Instance.Query(request.QuestionnaireID);

            Assert.AreEqual(actualResp, questionnaire);
            Assert.AreEqual(actualResp.Participants, questionnaire.Participants);


            QuestionnaireDao.Instance.Received(1).Query(request.QuestionnaireID);
            TopicDao.Instance.Received(1).Query(request.QuestionnaireID);
            OptionDao.Instance.Received(1).Query(request.QuestionnaireID);
        }

        [Test]
        public void Test_Query_Questionnaire_Not_Found()
        {
            var request = new QuestionnaireRequest
            {
                QuestionnaireID = 255
            };

            var topics = new List<Topic>();
            var options = new List<Option>();

            QuestionnaireDao.Instance
                .Query(request.QuestionnaireID)
                .Returns((Questionnaire)null);
            TopicDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(topics);
            OptionDao.Instance
                .Query(request.QuestionnaireID)
                .Returns(options);

            Assert.Throws<ApplicationException>(() => QuestionnaireBusiness.Instance.Query(request.QuestionnaireID));

            QuestionnaireDao.Instance.Received(1).Query(request.QuestionnaireID);
        }

        [Test]
        public void Test_Query_Questionnaire_Participator()
        {
            var expectResp = new List<Participator>();

            AnswerDao.Instance.QueryParticipator(3)
                .Returns(expectResp);

            var actualResp = QuestionnaireBusiness.Instance.QueryParticipator(3);

            Assert.AreEqual(actualResp, expectResp);
        }

        [Test]
        public void Test_Query_Questionnaire_Status_Refresh()
        {
            QuestionnaireDao.Instance.StatusRefresh().Returns(1);

            var actualResp = QuestionnaireBusiness.Instance.QuestionnaireStatusRefresh();

            Assert.AreEqual(actualResp, 1);
        }
    }
}