using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class User
    {
        /*•	Id – integer, Primary Key
•	Username – text with length [3, 20] (required)
•	FullName – text, which has two words, consisting of Latin letters. Both start with an upper letter and are followed by lower letters. The two words are separated by a single space (ex. "John Smith") (required)
•	Email – text (required)
•	Age – integer in the range [3, 103] (required)
•	Cards – collection of type Card
*/
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        //[StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        //[RegularExpression(@"^([A-Z]{1}[a-z]+) ([A-Z]{1}[a-z]+)$")]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        //[Required]
        //[Range(typeof(int), "3", "103")]
        public int Age { get; set; }

        public virtual ICollection<Card> Cards { get; set; } = new List<Card>();

    }
}
