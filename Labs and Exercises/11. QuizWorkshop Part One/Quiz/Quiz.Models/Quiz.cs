using System;
using System.Collections;
using System.Collections.Generic;

namespace Quiz.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new HashSet<Question>();

        public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new HashSet<UserAnswer>();
    }
}
