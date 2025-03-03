﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Bl.Model
{
    public class ExamViewModel
    {
        public Guid Id { get; set; }
        public string ExamTitle { get; set; }
        public bool ShowInHomePage { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
    }

    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        public string QuestionTitle { get; set; }
        public List<AnswerViewModel> Answers { get; set; }
    }

    public class AnswerViewModel
    {
        public Guid Id { get; set; }
        public string AnswerTitle { get; set; }
        public bool IsCorrect { get; set; }  
    }
}
