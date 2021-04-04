using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.Data.Models
{
    public class Employee
    {
        /*•	Id - integer, Primary Key
•	Username - text with length [3, 40]. Should contain only lower or upper case letters and/or digits. (required)
•	Email – text (required). Validate it! There is attribute for this job.
•	Phone - text. Consists only of three groups (separated by '-'), the first two consist of three digits and the last one - of 4 digits. (required)
•	EmployeesTasks - collection of type EmployeeTask
*/

        public int Id { get; set; }

        [Required]
        [MaxLength(40)] // min 3 // potential regex
        public string Username { get; set; }

        // email attrib
        public string Email { get; set; }

        [Required]
        // regex
        public string Phone { get; set; }

        public virtual ICollection<EmployeeTask> EmployeesTasks { get; set; } = new HashSet<EmployeeTask>();
    }
}
