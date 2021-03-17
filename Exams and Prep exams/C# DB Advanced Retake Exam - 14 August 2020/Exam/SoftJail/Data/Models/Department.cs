using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SoftJail.Data.Models
{
    [Table("Departments")]
    public class Department
    {
        /*•	Id – integer, Primary Key
•	Name – text with min length 3 and max length 25 (required)
•	Cells - collection of type Cell
*/
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Name { get; set; }

        public virtual List<Cell> Cells { get; set; } = new List<Cell>();
    }
}
