using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SoftJail.DataProcessor.ExportDto;

namespace SoftJail.DataProcessor
{

    using Data;
    using System;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            /*The given method in the project skeleton receives an array of prisoner ids. Export all prisoners that were processed which have these ids.
             For each prisoner, get their id, name, cell number they are placed in, their officers with each officer name, and the department name they are responsible for.
            At the end export the total officer salary with exactly two digits after the decimal place.
             Sort the officers by their name (ascending),  sort the prisoners by their name (ascending), then by the prisoner id (ascending).*/

            /*
            {
                           "Id": 3,
                           "Name": "Binni Cornhill",
                           "CellNumber": 503,
                           "Officers": [
                           {
                           "OfficerName": "Hailee Kennon",
                           "Department": "ArtificialIntelligence"
                           },
                           {
                           "OfficerName": "Theo Carde",
                           "Department": "Blockchain"
                           }
                           ],
                           "TotalOfficerSalary": 7127.93
                           },
            }
                           */

            var prisoners = context
                .Prisoners
                .ToList() // just in case
                .Where(p => ids.Any(i => i == p.Id))
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                        .ToList()
                        .Select(po => new
                        {
                            OfficerName = po.Officer.FullName,
                            Department = po.Officer.Department.Name
                        })
                        .OrderBy(o => o.OfficerName)
                        .ToList(),
                    TotalOfficerSalary = p.PrisonerOfficers.Sum(po => po.Officer.Salary) // appearently no F2 needed?
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(prisoners, Formatting.Indented);

            return jsonResult;
        }
        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            /*Use the method provided in the project skeleton, which receives a string of comma-separated prisoner names.
             Export the prisoners: for each prisoner, export its id, name, incarcerationDate in the format “yyyy-MM-dd” and their encrypted mails.
             The encrypted algorithm you have to use is just to take each prisoner mail description and reverse it.
            Sort the prisoners by their name (ascending), then by their id (ascending).*/

            /*<Prisoner>
                           <Id>3</Id>
                           <Name>Binni Cornhill</Name>
                           <IncarcerationDate>1967-04-29</IncarcerationDate>
                           <EncryptedMessages>
                           <Message>
                           <Description>!?sdnasuoht evif-ytnewt rof deksa uoy ro orez artxe na ereht sI</Description>
                           </Message>
                           </EncryptedMessages>
                           </Prisoner>
                           */
            var prisoners = context
                .Prisoners
                .ToList() // just in case
                .Where(p => prisonersNames.Contains(p.FullName))
                .Select(p => new ExportPrisonerDto()
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = p.Mails
                        .ToList() // just in case
                        .Select(m => new ExportPrisonerMailDto()
                        {
                            Description = ReverseString(m.Description)
                        })
                        .ToList()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToList();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportPrisonerDto>), new XmlRootAttribute("Prisoners"));


            using (StringWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, prisoners, namespaces);
            }

            return sb.ToString().TrimEnd();

        }
        public static string ReverseString(string s)
        {
            char[] array = s.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }
    }
}