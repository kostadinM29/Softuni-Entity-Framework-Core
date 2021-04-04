﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TeisterMask.Data.Models
{
    public class EmployeeTask
    {
        /*•	EmployeeId - integer, Primary Key, foreign key (required)
•	Employee -  Employee
•	TaskId - integer, Primary Key, foreign key (required)
•	Task - Task
*/
        [Required]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Required]
        [ForeignKey(nameof(Task))]
        public int TaskId { get; set; }
        public virtual Task Task { get; set; }
    }
}
