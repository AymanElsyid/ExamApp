using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tables
{
    public class TbQuestion : BaseTable
    {
        public TbQuestion()
        {
            Answers = new HashSet<TbAnswer>();
        }

        public string Title { get; set; }

        [ForeignKey("Exam")]
        public Guid ExamId { get; set; }
        public virtual TbExam Exam { get; set; }
        public virtual ICollection<TbAnswer> Answers { get; set; }
    }
}
