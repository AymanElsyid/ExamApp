using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl.Model
{
    public class ExamSubmit
    {
        public Guid ExamId { get; set; }
        
        public List<QuestionSubmitViewModel> Questions { get; set; }
    }

    public class QuestionSubmitViewModel
    {
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }
    }
}
