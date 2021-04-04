using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TeisterMask.DataProcessor.ExportDto
{
    [XmlType("Task")]
    public class ExportProjectTasksDto
    {
        /*<Tasks>
      <Task>
        <Name>Broadleaf</Name>
        <Label>JavaAdvanced</Label>
      </Task>
      <Task>
        <Name>Bryum</Name>
        <Label>EntityFramework</Label>
      </Task>
      <Task>
        <Name>Cornflag</Name>
        <Label>CSharpAdvanced</Label>
      </Task>
*/
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Label")]
        public string Label { get; set; }
    }
}
