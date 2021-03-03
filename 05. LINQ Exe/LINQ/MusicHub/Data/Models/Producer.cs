using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Producer
    {
        //•	Id – Integer, Primary Key
        //•	Name – text with max length 30 (required)
        //•	Pseudonym – text
        //•	PhoneNumber – text
        //•	Albums – collection of type Album
        public Producer()
        {
            Albums = new HashSet<Album>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        public string Pseudonym { get; set; }

        public string PhoneNumber { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
    }
}
