using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export.GetSoldProductsTask
{
    [XmlType("User")]
    public class ExportUserSoldProductsDto
    {
        [XmlElement("firstName")] public string FirstName { get; set; }

        [XmlElement("lastName")] public string LastName { get; set; }

        [XmlArray("soldProducts")] public List<ExportSoldProductsDto> SoldProducts { get; set; }
    }
}