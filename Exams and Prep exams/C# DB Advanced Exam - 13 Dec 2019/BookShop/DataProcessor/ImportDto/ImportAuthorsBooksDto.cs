using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BookShop.DataProcessor.ImportDto
{
    public class ImportAuthorsBooksDto
    {
        /*"Books": [
      {
        "Id": 79
      },
      {
        "Id": 40
      }
    ]*/
        [JsonProperty("Id")]
        public int? BookId { get; set; } // some are null in the json, but required in the db ?
    }
}
