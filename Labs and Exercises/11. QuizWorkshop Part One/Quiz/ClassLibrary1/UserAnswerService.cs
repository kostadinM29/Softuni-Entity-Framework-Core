using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quiz.Data;
using Quiz.Models;
using Quiz.Services.Models;

namespace Quiz.Services
{
    public class UserAnswerService : IUserAnswerService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserAnswerService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public void AddUserAnswer(string userId, int quizId, int questionId, int answerId)
        {
            var userAnswer = new UserAnswer
            {
                IdentityUserId = userId,
                QuizId = quizId,
                QuestionId = questionId,
                AnswerId = answerId
            };

            this._applicationDbContext.UserAnswers.Add(userAnswer);
            this._applicationDbContext.SaveChanges();
        }

        public void BulkAddUserAnswer(QuizInputModel quizInputModel)
        {
            var userAnswers = new List<UserAnswer>();

            foreach (var question in quizInputModel.Questions)
            {
                var userAnswer = new UserAnswer
                {
                    IdentityUserId = quizInputModel.UserId,
                    QuizId = quizInputModel.QuizId,
                    AnswerId = question.AnswerId,
                    QuestionId = question.QuestionId
                };

                userAnswers.Add(userAnswer);
            }

            this._applicationDbContext.UserAnswers.AddRange(userAnswers);
            this._applicationDbContext.SaveChanges();
        }

        public int GetUserResult(string userId, int quizId)
        {
            var totalPoints = this._applicationDbContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .ThenInclude(a => a.UserAnswers)
                .Where(q => q.Id == quizId && q.UserAnswers.Any(ua => ua.IdentityUserId == userId))
                .SelectMany(q => q.UserAnswers)
                .Where(ua => ua.Answer.IsCorrect)
                .Sum(ua => ua.Answer.Points);

            //var userAnswers = this._applicationDbContext.UserAnswers
            //    .Where(ua => ua.IdentityUserId == userId && ua.QuizId == quizId)
            //    .ToList();

            //int? totalPoints = 0;

            //foreach (var userAnswer in userAnswers)
            //{
            //    totalPoints += originalQuiz.Questions
            //        .FirstOrDefault(q => q.Id == userAnswer.QuestionId)
            //        .Answers
            //        .Where(a => a.IsCorrect)
            //        .FirstOrDefault(a => a.Id == userAnswer.AnswerId)
            //        ?.Points;


            //}

            return totalPoints;
        }
    }
}
