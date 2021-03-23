using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Models
{
    public class Question
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();

        public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new HashSet<UserAnswer>();
    }
}
