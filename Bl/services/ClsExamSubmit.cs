using Bl.Enumration;
using Bl.Infrastructure;
using Bl.Model;
using Domain;
using Domain.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bl.services
{
    public interface IClsExamSubmit : ITable<TbExamResult>
    {
        Task<bool> SaveResult(ExamSubmit Resullt, string UserId);
    }
    public class ClsExamSubmit : ClsTable<TbExamResult>, IClsExamSubmit
    {
        private readonly ExamAppDbContext Db;

        public ClsExamSubmit(ExamAppDbContext _Db) : base(_Db)
        {
            Db = _Db;
        }
        public async Task<bool> SaveResult(ExamSubmit Resullt, string UserId)
        {
            try
            {
                TbExamResult Resul = CalculateResult(Resullt);
                Resul.UserId = UserId;

                await Db.ExamResults.AddAsync(Resul);
                await Db.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private TbExamResult CalculateResult(ExamSubmit model)
        {
            try
            {
                var Questions = Db.Questions.Include(x => x.Answers).Where(a => a.ExamId == model.ExamId).ToList();
                var UserAnswer = new List<TbUserAnswer>();
                int Correct = 0;
                int Wrong = 0;

                foreach (var question in model.Questions)
                {
                    var IsRight = Questions.FirstOrDefault(x => x.Id == question.QuestionId).Answers.FirstOrDefault(a => a.Id == question.AnswerId).IsCorrect;
                    if (IsRight)
                        Correct += 1;
                    else
                        Wrong += 1;


                    var Uanswer = new TbUserAnswer
                    {
                        Id = Guid.NewGuid(),
                        CreatedDate = DateTime.Now,
                        CurrentState = (int)CurrentStateEnum.Active,
                        QuestionId = question.QuestionId,
                        UserAnswerId = question.AnswerId,
                        CorrectState = IsRight ? (int)AswerStateEnum.Right : (int)AswerStateEnum.Wrong,
                    };
                    UserAnswer.Add(Uanswer);
                }
                int SScore = (int)(((double)Correct / Questions.Count) * 100);
                var Newexam = new TbExamResult
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    CurrentState = (int)CurrentStateEnum.Active,
                    TotalQuestions = Questions.Count,
                    ExamId = model.ExamId,
                    Score = SScore,
                    IsPassed=SScore>60?true:false,
                    CorrectAnswers =Correct,
                    ExamDate = DateTime.Now,
                    UserAnswers=UserAnswer
                };
                return Newexam;
            }
            catch
            {
                return new TbExamResult();
            }
        }
    }
}
