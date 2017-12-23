
using System.Collections.Generic;

namespace Newegg.MIS.API.EggRolls.Entities
{
    /// <summary>
    /// Option
    /// </summary>
    public class Option
    {
        public int TopicID { get; set; }
        public string OptionID { get; set; }
        public string OptionTitle { get; set; }
        public int ChosenNumber { set; get; }
        public string PersonalUnits { set; get; }
        public List<Units> DepartmentUnits { set; get; }
    }
}
