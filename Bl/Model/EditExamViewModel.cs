using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl.Model
{
    
    public class EditExamViewModel
    {
        public Guid ExamID { get; set; }
        public string ExamTitle { get; set; }
        public bool ShowInHomePage { get; set; }
        public List<EditQuestionDto> Questions { get; set; }
    }

    public class EditQuestionDto
    {
        public Guid QuestionID { get; set; }
        public string QuestionTitle { get; set; }
        public List<EditAnswerDto> Answers { get; set; }
    }

    public class EditAnswerDto
    {
        public Guid AnswerID { get; set; }
        public string AnswerTitle { get; set; }
        public bool IsCorrect { get; set; }
    }
}
