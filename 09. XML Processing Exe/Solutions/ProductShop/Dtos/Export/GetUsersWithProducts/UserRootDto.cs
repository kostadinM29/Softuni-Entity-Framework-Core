using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.GetUsersWithProducts
{
    [XmlType("Users")]
    public class UserRootDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public List<UserExportDto> Users { get; set; }
    }
}
