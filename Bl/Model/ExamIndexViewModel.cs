using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl.Model
{
    public class ExamIndexViewModel()
    {
        public Guid ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int Currentstate { get; set; }
        public List<QuestionIndexViewModel> Questions { get; set; }
        public List<ExamResultsViewModel> ExamResults { get; set; }

    }
    public class QuestionIndexViewModel()
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

    }
    public class ExamResultsViewModel()
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsPassed { get; set; }

    }


}
