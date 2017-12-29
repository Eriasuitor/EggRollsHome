using System;
using System.Collections;
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
            var questionnaireBussiness = Substitute.For<IQuestionnaireBusiness>();
            InstanceManager.Register<IQuestionnaireDao>().Use(questionnaireDao);
            InstanceManager.Register<ITopicDao>().Use(topicDao);
            InstanceManager.Register<IOptionDao>().Use(optionDao);
            InstanceManager.Register<IAnswerSheetDao>().Use(answerDao);
            InstanceManager.Register<IQuestionnaireBusiness>().Use(questionnaireBussiness);
        }

        [TearDown]
        public void Clearup()
        {
            InstanceManager.Register<IQuestionnaireDao>().Reset();
            InstanceManager.Register<ITopicDao>().Reset();
            InstanceManager.Register<IOptionDao>().Reset();
            InstanceManager.Register<IAnswerSheetDao>().Reset();
            InstanceManager.Register<IQuestionnaireBusiness>().Reset();
        }

        [Test]
        public void Test_Add_Right()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    },
                    new Answer
                    {
                        TopicID = 2,
                        Ans = "A"
                    },
                    new Answer
                    {
                        TopicID = 3,
                        Ans = "A"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        TopicID = 1,
                        IsRequired = true,
                        Type = TopicType.Radio,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            }
                        }
                    },
                    new Topic
                    {
                        TopicID = 2,
                        Limited = 0,
                        IsRequired = true,
                        Type = TopicType.Checkbox,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            }
                        }
                    },
                    new Topic
                    {
                        TopicID = 3,
                        IsRequired = true,
                        Type = TopicType.Text,
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);
            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            AnswerSheetBusiness.Instance.Add(request);

            AnswerDao.Instance.Received(1).Add(request, "CN", "CD", "NESC");
        }

        [Test]
        public void Test_Add_Radio_Not_Found()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        Type = TopicType.Radio,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Checkbox_Not_Found()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        Type = TopicType.Checkbox,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Radio_No_Answer()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>()
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = true,
                        Type = TopicType.Radio,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Chechbox_No_Answer()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>()
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = true,
                        Type = TopicType.Checkbox,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Text_No_Answer()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>()
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = true,
                        Type = TopicType.Text,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Text_No_Answer_Is_Null()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer()
                    {
                        TopicID = 1,
                        Ans = "    "
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = true,
                        Type = TopicType.Text,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Radio_Answer_Out()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    },
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "B"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = false,
                        Type = TopicType.Radio,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            },
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Radio_Answer_NotRequired_Right()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = false,
                        Type = TopicType.Radio,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            },
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            AnswerSheetBusiness.Instance.Add(request);
        }

        [Test]
        public void Test_Add_Check_Answer_Num_Not_Much()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    },
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "B"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = true,
                        Limited = 3,
                        Type = TopicType.Checkbox,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Check_Answer_Req_Limit_Right()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = true,
                        Limited = 1,
                        Type = TopicType.Checkbox,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            AnswerSheetBusiness.Instance.Add(request);
        }

        [Test]
        public void Test_Add_Check_Answer_Req_ULimit_Wrong()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>()
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = true,
                        Limited = 0,
                        Type = TopicType.Checkbox,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Check_Answer_Ureq_Limit_Wrong()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = false,
                        Limited = 2,
                        Type = TopicType.Checkbox,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            },
                            new Option
                            {
                                OptionID = "B"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Add(request));
        }

        [Test]
        public void Test_Add_Check_Answer_Ureq_Limit_Right()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS",
                ShortName = "En.J.Hij",
                AnswerList = new List<Answer>
                {
                    new Answer
                    {
                        TopicID = 1,
                        Ans = "A"
                    }
                }
            };
            var questionnaire = new Questionnaire()
            {
                QuestionnaireID = 255,
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        IsRequired = false,
                        Limited = 0,
                        Type = TopicType.Checkbox,
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };


            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(new List<Answer>());

            AnswerSheetBusiness.Instance.Add(request);
        }

        [Test]
        public void Test_Add_Not_Found()
        {
            var request = new AnswerSheetRequest
            {
                QuestionnaireID = 255,
                Department = "CN CD NESC MIS"
            };

            QuestionnaireBusiness.Instance
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
                Topics = new List<Topic>
                {
                    new Topic
                    {
                        TopicID = 1,
                        Options = new List<Option>
                        {
                            new Option
                            {
                                OptionID = "A"
                            }
                        }
                    }
                },
                Status = QuestionnaireStatus.Processing
            };

            QuestionnaireBusiness.Instance
                .Query(request.QuestionnaireID)
                .Returns(questionnaire);

            AnswerDao.Instance
                .QueryExisted(request.QuestionnaireID, request.ShortName)
                .Returns(true);

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

            QuestionnaireBusiness.Instance
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

            QuestionnaireBusiness.Instance
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

            QuestionnaireBusiness.Instance
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
            var answers = new List<Answer>
            {
                new Answer
                {
                    TopicID = 1,
                    Ans = "A"
                }
            };

            QuestionnaireDao.Instance
                .QuestionnaireExistenceJudgment(request.QuestionnaireID)
                .Returns(true);

            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(answers);

            var actualResp = AnswerSheetBusiness.Instance.Query(request.QuestionnaireID, request.ShortName);

            AnswerDao.Instance.Received(1).Query(request.QuestionnaireID, request.ShortName);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp, answers);
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
                .QuestionnaireExistenceJudgment(request.QuestionnaireID)
                .Returns(false);

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Query(request.QuestionnaireID,request.ShortName));

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
            var answers = new List<Answer>();

            QuestionnaireDao.Instance
                .QuestionnaireExistenceJudgment(request.QuestionnaireID)
                .Returns(true);
            AnswerDao.Instance
                .Query(request.QuestionnaireID, request.ShortName)
                .Returns(answers);

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Query(request.QuestionnaireID,request.ShortName));

            AnswerDao.Instance.Received(1).Query(request.QuestionnaireID, request.ShortName);
        }

        [Test]
        public void Test_Query_Topic_Participator()
        {
            var expectResp = new List<Answer>
            {
                new Answer
                {
                    Department = "MIS",
                    TopicID = 1,
                    Ans = "A",
                    FullName = "Lo",
                    ShortName = "L"
                }
            };

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
        public void Test_Query_Answer_Sheet_Statistics_Found()
        {
            var participator = new List<ParticipatorStatistics>
            {
                new ParticipatorStatistics
                {
                    TopicID = 1,
                    OptionID = "A",
                    ChosenNumber = 2,
                    Percentage = 1
                }
            };
            var department = new List<AnswerStatistics>
            {
                new AnswerStatistics
                {
                    TopicID = 1,
                    OptionID = "A",
                    Department = "HR",
                    ChosenNumber = 1,
                    Percentage = 0.5m
                },
                new AnswerStatistics
                {
                    TopicID = 1,
                    OptionID = "A",
                    Department = "MIS",
                    ChosenNumber = 1,
                    Percentage = 0.5m
                }
            };
            var participatorList = new List<IList>
            {
                participator,department
            };

            QuestionnaireDao.Instance
                .QuestionnaireExistenceJudgment(255)
                .Returns(true);

            AnswerDao.Instance
                .Statistics(255)
                .Returns(participatorList);

            var actualResp = AnswerSheetBusiness.Instance.Statistics(255);

            Assert.AreNotEqual(actualResp, null);
            Assert.AreEqual(actualResp.Count, 1);
            Assert.AreEqual(actualResp[0].ChosenNumber, 2);
            Assert.AreEqual(actualResp[0].Percentage,1);
            Assert.AreEqual(actualResp[0].DepartmentStatisticsList.Count, 2);
            Assert.AreEqual(actualResp[0].DepartmentStatisticsList[0].ChosenNumber, 1);
            Assert.AreEqual(actualResp[0].DepartmentStatisticsList[0].Percentage, 0.5);
            Assert.AreEqual(actualResp[0].DepartmentStatisticsList[0].Department, "HR");
            Assert.AreEqual(actualResp[0].DepartmentStatisticsList[1].ChosenNumber, 1);
            Assert.AreEqual(actualResp[0].DepartmentStatisticsList[1].Percentage, 0.5);
            Assert.AreEqual(actualResp[0].DepartmentStatisticsList[1].Department, "MIS");
        }

        [Test]
        public void Test_Query_Answer_Sheet_Statistics_Not_Found()
        {
            QuestionnaireDao.Instance.QuestionnaireExistenceJudgment(3)
                .Returns(false);

            Assert.Throws<ApplicationException>(() => AnswerSheetBusiness.Instance.Statistics(3));
        }

    }
}