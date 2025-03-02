using Bl.Enumration;
using Bl.Infrastructure;
using Bl.Model;
using Bl.services;
using Domain.Tables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ExamApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ExamController : Controller
    {
        private readonly IClsExam clsExam;
        public ExamController(IClsExam _clsExam)
        {
            clsExam = _clsExam;
        }
        public IActionResult Index()
        {
            var Exams = clsExam.GetAll();
            return View(Exams);
        }

        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public bool Delete(Guid Id)
        {
            var Result = clsExam.Delete(Id);
            return Result;
        }

        [HttpPost]
        public bool Active(Guid Id)
        {
            var Result = clsExam.Active(Id);
            return Result;
        }

        [HttpGet]
        public async Task<IActionResult> GetExam(Guid Id)
        {
            var exam =await  clsExam.GetExamWithDetailsAsync(Id);
            if (exam == null)
            {
                return NotFound(new { message = "Exam not found." });
            }


            return Ok(exam);
        }


        [HttpPost]
        public async Task<IActionResult> CreateExamAsync([FromBody] ExamViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ExamTitle) || model.Questions == null || model.Questions.Count == 0)
            {
                return BadRequest("Invalid data.");
            }

            try
            {

                await clsExam.CreateExamAsync(model);

                return Ok(new { message = "Exam created successfully" });
            }
            catch
            {
                return BadRequest(new { message = "Exam created unsuccessfully" });
            }

            

        }
        [HttpPut]
        public async Task<IActionResult> EditExam([FromBody] EditExamViewModel model)
        {

            try
            {

                await clsExam.UpdateExamAsync(model);

                return Ok(new { message = "Exam updated successfully!" });
            }
            catch
            {
                return BadRequest(new { message = "Exam updated unsuccessfully" });
            }
        }
    }
}
