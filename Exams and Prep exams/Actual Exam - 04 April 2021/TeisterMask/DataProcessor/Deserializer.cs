using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TeisterMask.Data.Models;
using TeisterMask.Data.Models.Enums;
using TeisterMask.DataProcessor.ImportDto;

namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;

    using Data;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            /*
             <Projects>
  <Project>
    <Name>S</Name>
    <OpenDate>25/01/2018</OpenDate>
    <DueDate>16/08/2019</DueDate>
    <Tasks>
      <Task>
        <Name>Australian</Name>
        <OpenDate>19/08/2018</OpenDate>
        <DueDate>13/07/2019</DueDate>
        <ExecutionType>2</ExecutionType>
        <LabelType>0</LabelType>
      </Task>
      <Task>
        <Name>Upland Boneset</Name>
        <OpenDate>24/10/2018</OpenDate>
        <DueDate>11/06/2019</DueDate>
        <ExecutionType>2</ExecutionType>
        <LabelType>3</LabelType>
      </Task>
    </Tasks>
  </Project>

            •	If there are any validation errors for the project entity (such as invalid name or open date), do not import any part of the entity and append an error message to the method output.
            •	If there are any validation errors for the task entity (such as invalid name, open or due date are missing,
            task open date is before project open date or task due date is after project due date), do not import it (only the task itself, not the whole project) and append an error message to the method output.
               NOTE: Dates will be in format dd/MM/yyyy, do not forget to use CultureInfo.InvariantCulture
               */
            StringBuilder sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(List<ImportProjectsDto>), new XmlRootAttribute("Projects"));

            var projectsToAdd = new List<Project>();

            using (StringReader reader = new StringReader(xmlString))
            {
                var projectDtos = (List<ImportProjectsDto>)serializer.Deserialize(reader);

                foreach (var projectDto in projectDtos)
                {
                    if (!IsValid(projectDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var isOpenDateValid = DateTime.TryParseExact(projectDto.OpenDate, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var openDate);
                    if (!isOpenDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime? dueDate = null;
                    if (!string.IsNullOrEmpty(projectDto.DueDate))
                    {
                        var isDueDateValid = DateTime.TryParseExact(projectDto.DueDate, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDateValue);

                        if (!isDueDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        dueDate = dueDateValue;
                    }


                    var projectToAdd = new Project()
                    {
                        Name = projectDto.Name,
                        OpenDate = openDate,
                        DueDate = dueDate
                    };

                    var tasks = new List<Task>();

                    foreach (var taskDto in projectDto.Tasks)
                    {
                        if (!IsValid(taskDto))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        var isTaskOpenDateValid = DateTime.TryParseExact(taskDto.OpenDate, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var taskOpenDate);

                        if (!isTaskOpenDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        var isTaskDueDateValid = DateTime.TryParseExact(taskDto.DueDate, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out var taskDueDate);

                        if (!isTaskDueDateValid)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                        //var isExecutionTypeValid = Enum.TryParse(taskDto.ExecutionType, out ExecutionType executionType);

                        //if (!isExecutionTypeValid)
                        //{
                        //    sb.AppendLine(ErrorMessage);
                        //    continue;
                        //}

                        //var isLabelTypeValid = Enum.TryParse(taskDto.LabelType, out LabelType labelType);

                        //if (!isLabelTypeValid)
                        //{
                        //    sb.AppendLine(ErrorMessage);
                        //    continue;
                        //}

                        var taskToAdd = new Task()
                        {
                            Name = taskDto.Name,
                            OpenDate = taskOpenDate,
                            DueDate = taskDueDate,
                            ExecutionType = Enum.Parse<ExecutionType>(taskDto.ExecutionType),
                            LabelType = Enum.Parse<LabelType>(taskDto.LabelType)
                        };

                        //task open date is before project open date or task due date is after project due date
                        if (taskToAdd.OpenDate < projectToAdd.OpenDate || taskDueDate > projectToAdd.DueDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }
                        projectToAdd.Tasks.Add(taskToAdd);
                    }
                    projectsToAdd.Add(projectToAdd);
                    sb.AppendLine(string.Format(SuccessfullyImportedProject, projectToAdd.Name,
                        projectToAdd.Tasks.Count));
                }
            }
            context.Projects.AddRange(projectsToAdd);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            /* •	If any validation errors occur (such as invalid username, email or phone), do not import any part of the entity and append an error message to the method output.
•	Take only the unique tasks.
•	If a task does not exist in the database, append an error message to the method output and continue with the next task.
*/
            StringBuilder sb = new StringBuilder();

            var employeesDto = JsonConvert.DeserializeObject<List<ImportEmployeesDto>>(jsonString);

            var employeesToAdd = new List<Employee>();

            foreach (var employeeDto in employeesDto)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var employee = new Employee()
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone
                };

                foreach (var taskId in employeeDto.Tasks.Distinct())
                {

                    var task = context.Tasks.FirstOrDefault(t => t.Id == taskId);

                    if (task == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var taskToAdd = new EmployeeTask()
                    {
                        TaskId = taskId
                    };
                    employee.EmployeesTasks.Add(taskToAdd);
                }
                employeesToAdd.Add(employee);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
            }
            context.Employees.AddRange(employeesToAdd);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}