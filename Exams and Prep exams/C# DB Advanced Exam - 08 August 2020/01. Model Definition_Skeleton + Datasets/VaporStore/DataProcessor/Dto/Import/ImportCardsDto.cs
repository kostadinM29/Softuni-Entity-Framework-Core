using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    public class ImportCardsDto
    {
        /*Cards": [
      {
        "Number": "1111 1111 1111 1111",
        "CVC": "111",
        "Type": "Debit"
      }
*/

   

        [Required]
        [RegularExpression(@"^(\d{4}) (\d{4}) (\d{4}) (\d{4})$")]
        public string Number { get; set; }

        [Required]
        [MaxLength(3)]
        [RegularExpression(@"^(\d{3})$")]
        [JsonProperty("CVC")]
        public string Cvc { get; set; }

        [Required]
        public string Type { get; set; }

    }
}
