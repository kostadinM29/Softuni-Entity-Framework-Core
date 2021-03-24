using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class ImportPurchasesDto
    {
        /*  <Purchase title="Dungeon Warfare 2">
    <Type>Digital</Type>
    <Key>ZTZ3-0D2S-G4TJ</Key>
    <Card>1833 5024 0553 6211</Card>
    <Date>07/12/2016 05:49</Date>
  </Purchase>
*/
        [Required]
        [XmlAttribute("title")]
        public string Game { get; set; }

        [Required]
        [XmlElement("Type")]
        public string PurchaseType { get; set; }

        [Required]
        [RegularExpression(@"^([A-z0-9]{4})-([A-z0-9]{4})-([A-z0-9]{4})$")]
        [XmlElement("Key")]
        public string ProductKey { get; set; }

        [Required]
        [RegularExpression(@"^(\d{4}) (\d{4}) (\d{4}) (\d{4})$")]
        [XmlElement("Card")]
        public string CardNumber { get; set; }

        [Required]
        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
