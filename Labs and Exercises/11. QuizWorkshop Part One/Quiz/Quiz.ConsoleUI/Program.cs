using System;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quiz.Data;
using Quiz.Services;

namespace Quiz.ConsoleUI
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var buildServiceProvider = serviceCollection.BuildServiceProvider();

            var dbContext = buildServiceProvider.GetService<ApplicationDbContext>();

            //var quizService = buildServiceProvider.GetService<IQuizService>();
            //var quiz = quizService.GetQuizById(1);
            //quizService.Add("C# DB");


            //Console.WriteLine(quiz.Title);
            //foreach (var question in quiz.Question)
            //{
            //    Console.WriteLine(question.Title);
            //    foreach (var answer in question.Answers)
            //    {
            //        Console.WriteLine(answer.Title);

            //    }
            //}

            //var questionService = buildServiceProvider.GetService<IQuestionService>();
            //questionService.Add("1+1", 1);

            //var answerService = buildServiceProvider.GetService<IAnswerService>();
            //answerService.Add("2", 5, true, 2);

            var userAnswersService = buildServiceProvider.GetService<IUserAnswerService>();

            //userAnswersService.AddUserAnswer("8d12d34a-1de4-4335-ae2e-f3164d47feae", 1, 2, 1);

            var userResult = userAnswersService.GetUserResult("8d12d34a-1de4-4335-ae2e-f3164d47feae", 1);

            Console.WriteLine(userResult);
            //userAnswersService.AddUserAnswer("8d12d34a-1de4-4335-ae2e-f3164d47feae", 1, 1, 2);

        }
        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IQuizService, QuizService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddTransient<IAnswerService, AnswerService>();
            services.AddTransient<IUserAnswerService, UserAnswerService>();
        }
    }
}
