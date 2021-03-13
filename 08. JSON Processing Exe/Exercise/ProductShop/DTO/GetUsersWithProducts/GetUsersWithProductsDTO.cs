﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ProductShop.DTO.GetUsersWithProducts
{
    public class GetUsersWithProductsDTO
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("age")]
        public int? Age { get; set; } // not sure if it can be null

        [JsonProperty("soldProducts")]
        public GetSoldProductsDTO SoldProducts { get; set; }
    }
}
