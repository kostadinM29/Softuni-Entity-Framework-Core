﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    [Table("Prisoners")]
    public class Prisoner
    {
        /*Prisoner
           •	Id – integer, Primary Key
           •	FullName – text with min length 3 and max length 20 (required)
           •	Nickname – text starting with "The " and a single word only of letters with an uppercase letter for beginning(example: The Prisoner) (required)
           •	Age – integer in the range [18, 65] (required)
           •	IncarcerationDate ¬– Date (required)
           •	ReleaseDate– Date
           •	Bail– decimal (non-negative, minimum value: 0)
           •	CellId - integer, foreign key
           •	Cell – the prisoner's cell
           •	Mails - collection of type Mail
           •	PrisonerOfficers - collection of type OfficerPrisoner
           */

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"^The [A-Z][a-z]+$")]
        public string Nickname { get; set; }

        [Required]
        [Range(18, 65)]
        public int Age { get; set; }

        [Required]
        public DateTime IncarcerationDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Range(0, Double.PositiveInfinity)]
        public decimal? Bail { get; set; }

        [ForeignKey(nameof(Cell))]
        public int? CellId { get; set; }
        public virtual Cell Cell { get; set; }

        public virtual List<Mail> Mails { get; set; } = new List<Mail>();

        public virtual List<OfficerPrisoner> PrisonerOfficers { get; set; } = new List<OfficerPrisoner>();
    }
}