using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;
using SoftJail.Data.Models;
using SoftJail.Data.Models.Enums;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Officer")]
    public class ImportOfficerDto
    {
        /*
           <Officer>
           <Name>Minerva Kitchingman</Name>
           <Money>2582</Money>
           <Position>Invalid</Position>
           <Weapon>ChainRifle</Weapon>
           <DepartmentId>2</DepartmentId>
            <Prisoners>
                    <Prisoner id="15" />
            </Prisoners>
           </Officer>
        */
        [Required]
        [StringLength(30, MinimumLength = 3)]
        [XmlElement("Name")]
        public string FullName { get; set; }

        [Required]
        [Range(0, Double.PositiveInfinity)]
        [XmlElement("Money")]
        public decimal Salary { get; set; }

        [Required]
        [XmlElement("Position")]
        public string Position { get; set; }

        [Required]
        [XmlElement("Weapon")]
        public string Weapon { get; set; }

        [Required]
        [XmlElement("DepartmentId")]
        public int DepartmentId { get; set; }

        [XmlArray("Prisoners")]
        public List<ImportOfficerPrisonerDto> Prisoners { get; set; }
    }
}
