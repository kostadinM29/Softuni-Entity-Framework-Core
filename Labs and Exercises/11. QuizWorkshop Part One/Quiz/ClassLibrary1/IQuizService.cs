using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quiz.Services.Models;

namespace Quiz.Services
{
    public interface IQuizService
    {
        void Add(string title);

        QuizViewModel GetQuizById(int quizId);
    }
}
