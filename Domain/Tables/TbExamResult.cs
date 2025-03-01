using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tables
{
    public class TbExamResult : BaseTable
    {

        [ForeignKey("User")]
        public string UserId { get; set; } 
        [ForeignKey("Exam")]
        public Guid ExamId { get; set; } 
        public int Score { get; set; } 
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsPassed { get; set; }
        public DateTime ExamDate { get; set; } = DateTime.Now;

        public ApplicationUser User { get; set; }
        public TbExam Exam { get; set; }
    }
}
