using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ProductShop.DTO.GetUsersWithProducts
{
    public class GetAllUsersDTO
    {
        [JsonProperty("usersCount")]
        public int Count { get; set; }

        [JsonProperty("users")]
        public List<GetUsersWithProductsDTO> Users { get; set; }
    }
}
