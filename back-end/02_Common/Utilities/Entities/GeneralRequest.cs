namespace Newegg.MIS.API.Utilities.Entities
{
	public class GeneralRequest
	{
        public int? QuestionnaireID { get; set; }

        public int? TopicID { get; set;}

        public string OptionID { get; set; }

		public int? PageSize { get; set; }
		public int? PageIndex { get; set; }
	}
}