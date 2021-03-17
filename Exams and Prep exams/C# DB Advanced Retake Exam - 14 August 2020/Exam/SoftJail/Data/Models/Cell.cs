using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;

namespace SoftJail.Data.Models
{
    [Table("Cells")]
    public class Cell
    {
        /*•	Id – integer, Primary Key
•	CellNumber – integer in the range [1, 1000] (required)
•	HasWindow – bool (required)
•	DepartmentId - integer, foreign key (required)
•	Department – the cell's department (required)
•	Prisoners - collection of type Prisoner
*/

        [Key]
        public int Id { get; set; }

        [Required]
        [Range(1, 1000)]
        public int CellNumber { get; set; }

        [Required]
        public bool HasWindow { get; set; }

        [Required]
        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        [Required]
        public virtual Department Department { get; set; }

        public virtual List<Prisoner> Prisoners { get; set; } = new List<Prisoner>();
    }
}
