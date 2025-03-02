using Bl.Enumration;
using Bl.Infrastructure;
using Bl.Model;
using Domain;
using Domain.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl.services
{
    public interface IClsExam : ITable<TbExam>
    {
        Task<TbExam> GetExamWithDetailsAsync(Guid examId);
        Task UpdateExamAsync(EditExamViewModel exam);
        Task CreateExamAsync(ExamViewModel exam);
    }

    public class ClsExam : ClsTable<TbExam>, IClsExam
    {
        private readonly ExamAppDbContext Db;
        public ClsExam(ExamAppDbContext _db)
        {
            Db = _db;
        }

        public async Task<TbExam> GetExamWithDetailsAsync(Guid examId)
        {
            try
            {

                var Exam = await Db.Exams
                    .Include(e => e.Questions)
                        .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(e => e.Id == examId);
                return Exam;
            }
            catch
            {
                return new TbExam();
            }
        }



        public async Task UpdateExamAsync(EditExamViewModel exam)
        {
            try
            {


                //get Old Exam
                var oldExam = Db.Exams.Include(q => q.Questions).FirstOrDefault(x => x.Id == exam.ExamID);
                //Remove Old Answer 

                //foreach(var question in oldExam.Questions)
                //{

                //   var Answers= Db.Answers.Where(z => z.QuestionId == question.Id).ToList();
                //    Db.Answers.RemoveRange(Answers);
                //}

                Db.Questions.RemoveRange(oldExam.Questions);
                Db.SaveChanges();
                var Questions = new List<TbQuestion>();
                foreach (var question in exam.Questions)
                {
                    var Idd = Guid.NewGuid();
                    Questions.Add(new TbQuestion
                    {
                        ExamId = oldExam.Id,
                        Title = question.QuestionTitle,
                        CurrentState = (int)CurrentStateEnum.Active,
                        CreatedDate = DateTime.Now,
                        Id = Idd,
                    });
                    foreach (var answer in question.Answers)
                    {
                        Questions.FirstOrDefault(x => x.Id == Idd).Answers.Add(new TbAnswer
                        {
                            IsCorrect = answer.IsCorrect,
                            Title = answer.AnswerTitle,
                            CurrentState = (int)CurrentStateEnum.Active,
                            CreatedDate = DateTime.Now,
                        });
                    }
                }
                oldExam.Title = exam.ExamTitle;
                oldExam.CurrentState = Convert.ToInt32(exam.ShowInHomePage);
                Db.Exams.Update(oldExam);
                Db.Questions.AddRange(Questions);
                await Db.SaveChangesAsync();
            }
            catch
            {

            }

        }
        public async Task CreateExamAsync(ExamViewModel exam)
        {
            try
            {


                var Newexam = new TbExam
                {
                    Id = Guid.NewGuid(),
                    Title = exam.ExamTitle,
                    CurrentState = exam.ShowInHomePage ? (int)CurrentStateEnum.Active : (int)CurrentStateEnum.Deleted,
                    Questions = exam.Questions.Select(q => new TbQuestion
                    {
                        Id = Guid.NewGuid(),
                        Title = q.QuestionTitle,
                        CurrentState = (int)CurrentStateEnum.Active,
                        CreatedDate = DateTime.Now,
                        Answers = q.Answers.Select(a => new TbAnswer
                        {
                            Id = Guid.NewGuid(),
                            Title = a.AnswerTitle,
                            IsCorrect = a.IsCorrect,
                            CreatedDate = DateTime.Now,
                            CurrentState = (int)CurrentStateEnum.Active
                        }).ToList()
                    }).ToList()
                };
                await Db.Exams.AddAsync(Newexam);
                await Db.SaveChangesAsync();
            }
            catch
            {

            }

        }
    }
}
