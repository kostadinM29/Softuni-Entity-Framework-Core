using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SoftJail.DataProcessor.ImportDto
{
    [XmlType("Prisoner")]
    public class ImportOfficerPrisonerDto
    {
        /*<Prisoners>
                    <Prisoner id="15" />
            </Prisoners>
        */
        [XmlAttribute("id")] // attribute
        public int PrisonerId { get; set; }
    }
}
