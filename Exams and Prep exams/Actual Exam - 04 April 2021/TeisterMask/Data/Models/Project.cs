﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.Data.Models
{
    public class Project
    {
        /*•	Id - integer, Primary Key
•	Name - text with length [2, 40] (required)
•	OpenDate - date and time (required)
•	DueDate - date and time (can be null)
•	Tasks - collection of type Task
*/

        public int Id { get; set; }

        [Required]
        [MaxLength(40)] // min 2
        public string Name { get; set; }

        [Required]
        public DateTime OpenDate { get; set; }

        public DateTime? DueDate { get; set; }

        public virtual ICollection<Task> Tasks { get; set; } = new HashSet<Task>();
    }
}
