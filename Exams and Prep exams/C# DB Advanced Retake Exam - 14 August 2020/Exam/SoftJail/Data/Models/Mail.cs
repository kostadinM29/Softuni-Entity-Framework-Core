using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SoftJail.Data.Models
{
    [Table("Mails")]
    public class Mail
    {
        /*•	Id – integer, Primary Key
•	Description– text (required)
•	Sender – text (required)
•	Address – text, consisting only of letters, spaces and numbers, which ends with “ str.” (required) (Example: “62 Muir Hill str.“)
•	PrisonerId - integer, foreign key (required)
•	Prisoner – the mail's Prisoner (required)
*/
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [ForeignKey(nameof(Prisoner))]
        public int PrisonerId { get; set; }
        [Required]
        public virtual Prisoner Prisoner { get; set; }
    }
}
