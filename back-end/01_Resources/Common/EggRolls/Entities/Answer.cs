namespace Newegg.MIS.API.EggRolls.Entities
{
    public class Answer : Participator
    {
        public int? QuestionnaireID { get; set; }
        public int? TopicID { set; get; }
        public string Ans { set; get; }
        public TopicType? Type { get; set; }
    }
}
