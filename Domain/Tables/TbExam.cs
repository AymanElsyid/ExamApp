using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tables
{
    public class TbExam :BaseTable
    {
        public TbExam()
        {
            Questions = new HashSet<TbQuestion>();
        }
        [Key]
        public string Title { get; set; }
        public virtual ICollection<TbQuestion> Questions{ get; set; }
    }
}
