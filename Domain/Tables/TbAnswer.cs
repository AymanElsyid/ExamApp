using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tables
{
    public class TbAnswer:BaseTable
    {
        [Required]
        public string Title { get; set; }

        public bool IsCorrect { get; set; } = false;
        public Guid QuestionId { get; set; }
        public virtual TbQuestion Question { get; set; }
    }
}
