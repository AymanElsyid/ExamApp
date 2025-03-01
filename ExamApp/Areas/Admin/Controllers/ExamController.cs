using Bl.Enumration;
using Bl.Infrastructure;
using Bl.Model;
using Domain.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ExamController : Controller
    {
        ClsTable<TbExam> clsExam = new ClsTable<TbExam>();
        public ExamController()
        {
            
        }
        public IActionResult Index()
        {
            var Exams = clsExam.GetAll();
            return View(Exams);
        }
        
        public IActionResult Add()
        {
            return View(new TbExam());
        }
        [HttpPost]
        public bool Delete(Guid Id)
        {
            var Result= clsExam.Delete(Id);
            return Result;
        }
        
        [HttpPost]
        public bool Active(Guid Id)
        {
            var Result= clsExam.Active(Id);
            return Result;
        }


        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] ExamViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ExamTitle) || model.Questions == null || model.Questions.Count == 0)
            {
                return BadRequest("Invalid data.");
            }

            var exam = new TbExam
            {
                Id = Guid.NewGuid(),
                Title = model.ExamTitle,
                CurrentState= model.ShowInHomePage?(int)CurrentStateEnum.Active: (int)CurrentStateEnum.Deleted,
                Questions = model.Questions.Select(q => new TbQuestion
                {
                    
                    Title = q.QuestionTitle,
                    Answers = q.Answers.Select(a => new TbAnswer
                    {
                        Id = Guid.NewGuid(),
                        Title = a.AnswerTitle,
                        IsCorrect= a.IsCorrect
                    }).ToList()
                }).ToList()
            };

            var Rersult =  clsExam.Add(exam);
                if(Rersult)
            return Ok(new { message = "Exam created successfully" });
                else
                return BadRequest(new { message = "Exam created successfully" });

        }
    }
}
