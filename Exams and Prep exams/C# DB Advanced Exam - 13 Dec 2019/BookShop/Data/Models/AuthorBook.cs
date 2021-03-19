using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BookShop.Data.Models
{
    public class AuthorBook
    {
        /*•	AuthorId - integer, Primary Key, Foreign key (required)
•	Author -  Author
•	BookId - integer, Primary Key, Foreign key (required)
•	Book - Book
*/
        [Required]
        [ForeignKey(nameof(Author))]
        public int AuthorId { get; set; }
        public virtual Author Author { get; set; }

        [Required]
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
    }
}
