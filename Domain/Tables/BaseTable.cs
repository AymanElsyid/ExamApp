using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tables
{
    public class BaseTable
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; } 
        public DateTime CreatedDate { get; set; }= DateTime.Now;
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int CurrentState { get; set; }
    }
}
