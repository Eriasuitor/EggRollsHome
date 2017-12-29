using System.Collections.Generic;
using Newegg.MIS.API.EggRolls.Entities;
using Newegg.MIS.API.Utilities.Entities;

namespace Newegg.MIS.API.EggRolls.ResponseEntities
{
    public class AnswerSheetStatisticsResponse : GeneralResponse
    {
        public List<ParticipatorStatistics> ParticipatorStatisticsList { get; set; }
    }
}
