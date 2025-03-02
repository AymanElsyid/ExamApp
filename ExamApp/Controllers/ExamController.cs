using Bl.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Design;

namespace ExamApp.Controllers
{
    [Authorize(Roles ="User")]
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
    }
}
