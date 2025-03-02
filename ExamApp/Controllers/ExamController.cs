using Bl.Model;
using Bl.services;
using Domain.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExamApp.Controllers
{
    [Authorize(Roles = "User")]
    public class ExamController : Controller
    {
        private readonly IClsExamSubmit clsExamSubmit;
        private readonly IClsExam clsExam;
        private readonly IClsUser clsUser;
        public ExamController(IClsExam _clsExam, IClsUser _clsUser, IClsExamSubmit _clsExamSubmit)
        {
            clsExam = _clsExam;
            clsUser = _clsUser;
            clsExamSubmit = _clsExamSubmit;
        }
        public IActionResult Index()
        {
            var UserId = clsUser.GetUserId(User.Identity.Name);
            var Exams = clsExam.GetIndexData(UserId);
            Exams = Exams.Where(z => z.Currentstate == 1).ToList();
            return View(Exams);
        }

        public IActionResult Start()
        {
            return View();
        }
        public IActionResult Review()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> getExamData(Guid Id)
        {
            var Exam = await clsExam.GetExamWithDetailsAsync(Id);
            if (Exam == null)
            {
                return NotFound(new { message = "Exam not found." });
            }


            return Ok(Exam);

        }
        [HttpGet]
        public async Task<IActionResult> getReviewExam(Guid Id)
        {
            var Exam = await clsExam.GetReviewExamWithDetailsAsync(Id, clsUser.GetUserId(User.Identity.Name));
            if (Exam == null)
            {
                return NotFound(new { message = "Exam not found." });
            }


            return Ok(Exam);

        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam([FromBody] ExamSubmit model)
        {
            if (model == null || model.Questions == null || !model.Questions.Any())
            {
                return BadRequest("Invalid exam submission data.");
            }

            var result = await clsExamSubmit.SaveResult(model,clsUser.GetUserId(User.Identity.Name));
            if (result)
                return Ok(new { message = "Exam submitted successfully!" });
            else
                return BadRequest(new { message = "Exam submitted unsuccessfully" });
        }

    }
}
