using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quiz.Data;
using Quiz.Models;

namespace Quiz.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public AnswerService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public int Add(string title, int points, bool isCorrect, int questionId)
        {
            var answer = new Answer
            {
                Title = title,
                Points = points,
                IsCorrect = isCorrect,
                QuestionId = questionId
            };

            this._applicationDbContext.Answers.Add(answer);
            this._applicationDbContext.SaveChanges();

            return answer.Id;
        }
    }
}
