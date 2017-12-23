
using System;

namespace Newegg.MIS.API.Utilities.Entities
{
    /// <summary>
    /// CURD对数据库操作必须有的字段
    /// CURD Operation
    /// </summary>
    public class BasicOperationRequest
    {
        public int QuestionnaireID { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BackgroundImageUrl { get; set; }
                      
        public bool IsRealName { get; set; }
        public string DueDate { get; set; }


    }
}
