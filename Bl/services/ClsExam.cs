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
        Task<ExamViewModel> GetExamWithDetailsAsync(Guid examId);
        Task<ReviewExamSubmit> GetReviewExamWithDetailsAsync(Guid examId, string UserId);
        Task UpdateExamAsync(EditExamViewModel exam);
        Task CreateExamAsync(ExamViewModel exam);
        List<ExamIndexViewModel> GetIndexData(string UserId);
    }

    public class ClsExam : ClsTable<TbExam>, IClsExam
    {
        private readonly ExamAppDbContext Db;

        public ClsExam(ExamAppDbContext _Db) : base(_Db)
        {
            Db = _Db;
        }

        public async Task<ExamViewModel> GetExamWithDetailsAsync(Guid examId)
        {
            try
            {

                var Exam = await (from E in Db.Exams
                            join Q in Db.Questions on E.Id equals Q.ExamId into QuestionGroup
                            from Q in QuestionGroup.DefaultIfEmpty() // Left join to include exams without questions
                            join A in Db.Answers on Q.Id equals A.QuestionId into AnswerGroup
                            from A in AnswerGroup.DefaultIfEmpty() // Left join to include questions without answers
                            where E.Id == examId // 👈 Filter exam by ExamId
                            group new { E, Q, A } by new { E.Id, E.Title, E.CurrentState } into groupedExams
                            select new ExamViewModel
                            {
                                Id = groupedExams.Key.Id,
                                ExamTitle = groupedExams.Key.Title,
                                ShowInHomePage = Convert.ToBoolean(groupedExams.Key.CurrentState),
                                Questions = groupedExams
                                    .Where(g => g.Q != null) // Ensure question is not null
                                    .GroupBy(g => new { g.Q.Id, g.Q.Title }) // Group by Question
                                    .Select(qGroup => new QuestionViewModel
                                    {
                                        Id = qGroup.Key.Id,
                                        QuestionTitle = qGroup.Key.Title,
                                        Answers = qGroup
                                            .Where(g => g.A != null) // Ensure answer is not null
                                            .Select(g => new AnswerViewModel
                                            {
                                                Id = g.A.Id,
                                                AnswerTitle = g.A.Title,
                                                IsCorrect = g.A.IsCorrect
                                            }).ToList()
                                    }).ToList()
                            }).FirstOrDefaultAsync();
                return Exam;
            }
            catch
            {
                return new ExamViewModel();
            }
        }
        public async Task<ReviewExamSubmit> GetReviewExamWithDetailsAsync(Guid examId, string UserId)
        {
            try
            {

                var Exam = await (from ER in Db.ExamResults
                                  join E in Db.Exams on ER.ExamId equals E.Id // Get ExamTitle from TbExam
                                  join UA in Db.UserAnswers on ER.Id equals UA.ExamResultId // Link user answers to exam results
                                  join Q in Db.Questions on UA.QuestionId equals Q.Id // Get question details
                                  join A in Db.Answers on Q.Id equals A.QuestionId // Get all possible answers for each question
                                  join CA in Db.Answers on new { Q.Id, IsCorrect = true } equals new { Id = CA.QuestionId, CA.IsCorrect } // Get correct answers
                                  where ER.ExamId == examId // Filter by ExamId
                                  group new { ER, UA, Q, A, CA } by new { ER.ExamId, E.Title, E.CurrentState } into groupedExams
                                  select new ReviewExamSubmit
                                  {
                                      Id = groupedExams.Key.ExamId,
                                      ExamTitle = groupedExams.Key.Title, // Get ExamTitle from TbExam
                                      ShowInHomePage = Convert.ToBoolean(groupedExams.Key.CurrentState),

                                      Questions = groupedExams
                                          .Where(g => g.Q != null) // Ensure question is not null
                                          .GroupBy(g => new { g.Q.Id, g.Q.Title }) // Group by Question
                                          .Select(qGroup => new QuestionViewModel
                                          {
                                              Id = qGroup.Key.Id,
                                              QuestionTitle = qGroup.Key.Title,
                                              Answers = qGroup // Select all answers, not just correct ones
                                                  .Select(g => new AnswerViewModel
                                                  {
                                                      Id = g.A.Id,
                                                      AnswerTitle = g.A.Title,
                                                      IsCorrect = g.A.IsCorrect // Keep track of correct answers
                                                  }).Distinct().ToList() // Ensure unique answers
                                          }).ToList(),

                                      UQuestions = groupedExams
                                          .Select(g => new QuestionReviewViewModel
                                          {
                                              QuestionId = g.UA.QuestionId, // User attempted question
                                              AnswerId = g.UA.UserAnswerId, // User's selected answer
                                              CorrectAnswerId = g.CA.Id // Actual correct answer
                                          }).Distinct().ToList() // Ensure unique question attempts
                                  }).FirstOrDefaultAsync();



                return Exam;
            }
            catch
            {
                return new ReviewExamSubmit();
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

        public List<ExamIndexViewModel> GetIndexData(string UserId)
        {
            try
            {


                var ExamIndex = (from E in Db.Exams
                                 join Q in Db.Questions on E.Id equals Q.ExamId
                                 join ER in Db.ExamResults on Q.ExamId equals ER.ExamId into ExamResultsGroup
                                 from ER in ExamResultsGroup.DefaultIfEmpty() // Left join to include exams without results
                                 where ER == null || ER.UserId == UserId // Include exams without results or those matching UserId
                                 group new { E, Q, ER } by new { E.Id, E.Title ,E.CurrentState } into groupedExams
                                 select new ExamIndexViewModel
                                 {
                                     ExamId = groupedExams.Key.Id,
                                     ExamTitle = groupedExams.Key.Title,
                                     Currentstate=groupedExams.Key.CurrentState,
                                     Questions = groupedExams.Select(a => new QuestionIndexViewModel
                                     {
                                         Id = a.Q.Id,
                                         Title = a.Q.Title
                                     }).ToList(),
                                     ExamResults = groupedExams.Where(g => g.ER != null).Select(g => new ExamResultsViewModel
                                     {
                                         Id=g.ER.Id,
                                         CorrectAnswers=g.ER.CorrectAnswers,
                                         IsPassed= g.ER.IsPassed,
                                         Score = g.ER.Score,
                                         TotalQuestions = g.ER.TotalQuestions
                                     }).ToList() // Avoid nulls
                                 }).ToList();
                return ExamIndex;
            }
            catch
            {
                return new List<ExamIndexViewModel>();
            }
        }

       
    }
}
