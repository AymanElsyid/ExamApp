using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tables
{
    public class TbUserAnswer:BaseTable
    {

        public Guid QuestionId { get; set; }
        public Guid UserAnswerId { get; set; }
        [ForeignKey("ExamResult")]
        public Guid ExamResultId { get; set; }
        public int CorrectState{ get; set; }
        public virtual TbExamResult ExamResult { get; set; }
    }
}
