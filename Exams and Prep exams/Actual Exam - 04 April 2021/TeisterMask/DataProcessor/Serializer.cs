using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using TeisterMask.Data.Models.Enums;
using TeisterMask.DataProcessor.ExportDto;
using TeisterMask.DataProcessor.ImportDto;

namespace TeisterMask.DataProcessor
{
    using System;

    using Data;

    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            /*Export all projects that have at least one task. For each project, export its name, tasks count, and if it has end (due) date which is represented like "Yes" and "No".
         For each task, export its name and label type. Order the tasks by name (ascending).
         Order the projects by tasks count (descending), then by name (ascending).*/
            var projects = context
                .Projects
                .ToList()
                .Where(p => p.Tasks.Any())
                .Select(p => new ExportProjectsDto()
                {
                    TasksCount = p.Tasks.Count,
                    ProjectName = p.Name,
                    HasEndDate = p.DueDate.HasValue ? "Yes" : "No",
                    Tasks = p.Tasks
                        .ToList()
                        .Select(t => new ExportProjectTasksDto()
                        {
                            Name = t.Name,
                            Label = t.LabelType.ToString()
                        })
                        .OrderBy(t => t.Name)
                        .ToList()
                })
                .OrderByDescending(p => p.TasksCount)
                .ThenBy(p => p.ProjectName)
                .ToList();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportProjectsDto>), new XmlRootAttribute("Projects"));

            using (StringWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, projects, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            /*Select the top 10 employees who have at least one task that its open date is after or equal to the given date with their tasks that meet the same requirement
            (to have their open date after or equal to the giver date). For each employee, export their username and their tasks.
           For each task, export its name and open date (must be in format "d"), due date (must be in format "d"), label and execution type.
           Order the tasks by due date (descending), then by name (ascending). Order the employees by all tasks (meeting above condition) count (descending), then by username (ascending).
           NOTE: Do not forget to use CultureInfo.InvariantCulture. You may need to call .ToArray() 
           function before the selection in order to detach entities from the database and avoid runtime errors (EF Core bug). 
*/
            var employees = context
                .Employees
                .ToList()
                .Where(e => e.EmployeesTasks.Any(et => et.Task.OpenDate >= date))
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks = e.EmployeesTasks
                        .Where(et => et.Task.OpenDate >= date)
                        .OrderByDescending(et => et.Task.DueDate)
                        .ThenBy(et => et.Task.Name)
                        .Select(et => new
                        {
                            TaskName = et.Task.Name,
                            OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                            DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                            LabelType = et.Task.LabelType.ToString(),
                            ExecutionType = et.Task.ExecutionType.ToString()
                        })
                        .ToList()
                })
                .OrderByDescending(e => e.Tasks.Count)
                .ThenBy(e => e.Username)
                .Take(10)
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return jsonResult;
        }
    }
}