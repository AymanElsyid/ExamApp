using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl.Model
{
    public class ReviewExamSubmit: ExamViewModel
    {
       
        public List<QuestionReviewViewModel> UQuestions { get; set; }
    }

    public class QuestionReviewViewModel
    {
        public Guid QuestionId { get; set; }
        public Guid AnswerId { get; set; }
        public Guid CorrectAnswerId { get; set; }
    }
}
