using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ProductShop.DTO.GetUsersWithProducts
{
    public class GetSoldProductsDTO
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("products")]
        public List<GetProductsDTO> Products { get; set; }
    }
}
