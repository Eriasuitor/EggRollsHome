
using System.Collections.Generic;

namespace Newegg.MIS.API.EggRolls.Entities
{
    /// <summary>
    /// Topic has Options
    /// </summary>
    public class Topic
    {
        public int TopicID { get; set; }
        public TopicType Type { get; set; }
        public bool IsRequired { get; set; }
        public string TopicTitle { get; set; }
        public int? Limited { get; set; }
        public List<Option> Options { get; set; }
    }
}
