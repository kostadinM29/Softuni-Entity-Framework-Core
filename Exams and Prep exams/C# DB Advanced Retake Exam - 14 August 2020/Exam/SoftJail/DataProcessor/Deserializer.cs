using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using SoftJail.Data.Models;
using SoftJail.Data.Models.Enums;
using SoftJail.DataProcessor.ImportDto;

namespace SoftJail.DataProcessor
{

    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var departmentDtos = JsonConvert.DeserializeObject<List<ImportDepartmentDto>>(jsonString);

            var departments = new List<Department>();

            foreach (var departmentDto in departmentDtos)
            {
                if (!IsValid(departmentDto)) // check if not valid
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var department = new Department
                {
                    Name = departmentDto.Name
                };

                var cells = new List<Cell>();

                foreach (var cellDto in departmentDto.Cells)
                {
                    if (!IsValid(cellDto))
                    {
                        cells = new List<Cell>(); // to trigger the bottom check ?
                        break;
                    }
                    var cell = new Cell
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow

                    };
                    cells.Add(cell);
                }
                if (!cells.Any())
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                department.Cells = cells;
                departments.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }
            context.Departments.AddRange(departments);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            var prisonerDtos = JsonConvert.DeserializeObject<List<ImportPrisonersDto>>(jsonString);

            List<Prisoner> prisoners = new List<Prisoner>();

            foreach (var prisonerDto in prisonerDtos)
            {
                if (!IsValid(prisonerDto))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool isIncarcerationDateValid = DateTime.TryParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var incarcerationDate);

                if (!isIncarcerationDateValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime? releaseDate = null;
                if (!String.IsNullOrEmpty(prisonerDto.ReleaseDate))
                {
                    var isReleaseDateValid = DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDateValue);

                    if (!isReleaseDateValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    releaseDate = releaseDateValue;
                }

                var prisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId
                };
                foreach (var mailDto in prisonerDto.Mails)
                {
                    if (!IsValid(mailDto))
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    prisoner.Mails.Add(new Mail
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    });
                }
                prisoners.Add(prisoner);

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }
            context.Prisoners.AddRange(prisoners);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ImportOfficerDto>), new XmlRootAttribute("Officers"));

            var officers = new List<Officer>();

            using (StringReader reader = new StringReader(xmlString))
            {
                var officerDtos = (List<ImportOfficerDto>)serializer.Deserialize(reader);

                foreach (var officerDto in officerDtos)
                {
                    if (!IsValid(officerDto))
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    bool isValidPosition = Enum.TryParse(officerDto.Position, out Position position);
                    if (!isValidPosition)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    bool isValidWeapon = Enum.TryParse(officerDto.Weapon, out Weapon weapon);
                    if (!isValidWeapon)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    Officer officer = new Officer()
                    {
                        DepartmentId = officerDto.DepartmentId,
                        FullName = officerDto.FullName,
                        Position = position,
                        Weapon = weapon,
                        Salary = officerDto.Salary
                    };

                    foreach (var prisonerDto in officerDto.Prisoners)
                    {
                        // always valid
                        officer.OfficerPrisoners.Add(new OfficerPrisoner()
                        {
                            PrisonerId = prisonerDto.PrisonerId
                        });
                    }

                    officers.Add(officer);
                    sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
                }
                context.Officers.AddRange(officers);

                context.SaveChanges();

                return sb.ToString().TrimEnd();
            }
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}