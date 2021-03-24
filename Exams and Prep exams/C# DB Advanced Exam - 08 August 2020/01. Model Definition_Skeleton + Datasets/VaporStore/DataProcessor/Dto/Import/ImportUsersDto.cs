using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportUsersDto
    {
        /*"FullName": "Anita Ruthven",
    "Username": "aruthven",
    "Email": "aruthven@gmail.com",
    "Age": 75,
    "Cards": [
      {
        "Number": "5208 8381 5687 8508",
        "CVC": "624",
        "Type": "Debit"
      }
    ]
*/
      

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^([A-Z]{1}[a-z]+) ([A-Z]{1}[a-z]+)$")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress] // not sure if required
        public string Email { get; set; }

        [Required]
        [Range(typeof(int), "3", "103")]
        public int Age { get; set; }

        public ICollection<ImportCardsDto> Cards { get; set; }
    }
}
