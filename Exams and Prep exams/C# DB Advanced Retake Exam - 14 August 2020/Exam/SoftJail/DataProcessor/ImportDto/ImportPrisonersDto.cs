using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using SoftJail.Data.Models;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonersDto
    {
      

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
        public string IncarcerationDate { get; set; }

        public string ReleaseDate { get; set; }

        [Range(0, Double.PositiveInfinity)]
        public decimal? Bail { get; set; }

        public int? CellId { get; set; }

        public List<ImportPrisonerMailDto> Mails { get; set; }

    }
}
