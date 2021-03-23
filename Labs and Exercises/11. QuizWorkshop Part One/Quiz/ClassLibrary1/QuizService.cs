using System.Linq;
using Microsoft.EntityFrameworkCore;
using Quiz.Data;
using Quiz.Services.Models;

namespace Quiz.Services
{
    public class QuizService : IQuizService
    {

        private readonly ApplicationDbContext _applicationDbContext;

        public QuizService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public void Add(string title)
        {
            var quiz = new Quiz.Models.Quiz
            {
                Title = title
            };

            this._applicationDbContext.Quizzes.Add(quiz);
            this._applicationDbContext.SaveChanges();
        }

        public QuizViewModel GetQuizById(int quizId)
        {
            var quiz = this._applicationDbContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefault(q => q.Id == quizId);


            var quizViewModel = new QuizViewModel
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Question = quiz.Questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    Answers = q.Answers.Select(a => new AnswerViewModel
                    {
                        Id = a.Id,
                        Title = a.Title
                    })
                })
            };

            return quizViewModel;
        }
    }
}
