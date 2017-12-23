namespace Newegg.MIS.API.EggRolls.Entities
{
    public class Answer
    {
        public int? TopicID { set; get; }
        public string Ans { set; get; }
        public TopicType? Type { get; set; }
        public string ShortName { set; get; }
        public string FullName { set; get; }
        public string Department { set; get; }
    }
}
