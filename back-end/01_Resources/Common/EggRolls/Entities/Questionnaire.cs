using System;
using System.Collections.Generic;

namespace Newegg.MIS.API.EggRolls.Entities
{
    /// <summary>
    /// Questionnaire has Topics
    /// </summary>
    public class Questionnaire
    {
        public int QuestionnaireID { get; set; }
        public string Title { get; set; }
        public QuestionnaireStatus Status { get; set; }
        public bool IsRealName { get; set; }
        public DateTime? DueDate { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public int Participants { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string MailTo { get; set; }
        public List<Topic> Topics { get; set; }
    }
}