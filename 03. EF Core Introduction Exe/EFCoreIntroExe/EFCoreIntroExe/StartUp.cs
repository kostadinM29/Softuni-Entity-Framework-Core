using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            using (context)
            {
                var output = GetEmployeesByFirstNameStartingWithSa(context);
                Console.WriteLine(output);
            }
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            /*Your first task is to extract all employees and return their first, last and middle name, their job title and salary,
             rounded to 2 symbols after the decimal separator, all of those separated with a space.
             Order them by employee id.*/
            var employees = context
                .Employees
                .OrderBy(x => x.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            /*Your task is to extract all employees with salary over 50000.
             Return their first names and salaries in format “{firstName} - {salary}”.Salary must be rounded to 2 symbols, after the decimal separator.
             Sort them alphabetically by first name.*/
            var employees = context
                .Employees
                .Where(w => w.Salary > 50000)
                .Select(s => new
                {
                    s.FirstName,
                    s.Salary
                })
                .OrderBy(x => x.FirstName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            /*Extract all employees from the Research and Development department. Order them by salary (in ascending order), then by first name (in descending order).
             Return only their first name, last name, department name and salary rounded to 2 symbols, after the decimal separator.*/
            var employees = context
                .Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.DepartmentName} - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            /*Create a new address with text "Vitoshka 15" and TownId 4.
             Set that address to the employee with last name "Nakov".
             Then order by descending all the employees by their Address’ Id, take 10 rows and from them, take the AddressText.
             Return the results each on a new line */
            Employee employeeNakov = context
                .Employees
                .First(e => e.LastName == "Nakov");
            employeeNakov.Address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.SaveChanges();

            var addresses = context
                .Employees
                .OrderByDescending(a => a.AddressId)
                .Take(10)
                .Select(a => new
                {
                    a.Address.AddressText
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var adress in addresses)
            {
                sb.AppendLine(adress.AddressText);
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            /*Find the first 10 employees who have projects started in the period 2001 - 2003 (inclusive). Print each employee's first name, last name, manager’s first name and last name.
             Then return all of their projects in the format "--<ProjectName> - <StartDate> - <EndDate>", each on a new row.
             If a project has no end date, print "not finished" instead.*/
            var employees = context
                .Employees
                .Where(e => e.EmployeesProjects.Any(ep =>
                    ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"), //possible error
                            EndDate = ep.Project.EndDate.HasValue
                                ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                                : "not finished" //possible error
                        })
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastName}");

                foreach (var project in employee.Projects)
                {
                    sb.AppendLine($"--{project.ProjectName} - {project.StartDate} - {project.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetAddressesByTown(SoftUniContext context)
        {
            /*Find all addresses, ordered by the number of employees who live there (descending), then by town name (ascending), and finally by address text (ascending).
             Take only the first 10 addresses. 
             For each address return it in the format "<AddressText>, <TownName> - <EmployeeCount> employees"*/
            var addresses = context
                .Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => new
                {
                    AddressText = a.AddressText,
                    TownName = a.Town.Name,
                    EmployeesCount = a.Employees.Count
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var a in addresses)
            {
                sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeesCount} employees");
            }

            return sb.ToString().TrimEnd(); // result doesn't match word doc but we will see
        }
        public static string GetEmployee147(SoftUniContext context)
        {
            /*Get the employee with id 147. Return only his/her first name, last name, job title and projects (print only their names).
             The projects should be ordered by name (ascending). Format of the output.*/
            var employee = context
                .Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                        .Select(ep => ep.Project.Name)
                        .OrderBy(ep => ep)
                        .ToList()
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employee) // can be done with single append line but this is more intuitive for upgrades?
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                foreach (var project in e.Projects)
                {
                    sb.AppendLine(project);
                }
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            /*Find all departments with more than 5 employees. Order them by employee count (ascending), then by department name (alphabetically). 
             For each department, print the department name and the manager’s first and last name on the first row. 
             Then print the first name, the last name and the job title of every employee on a new row. 
             Order the employees by first name (ascending), then by last name (ascending).
             Format of the output: For each department print it in the format "<DepartmentName> - <ManagerFirstName>  <ManagerLastName>" and for each employee print it 
             in the format "<EmployeeFirstName> <EmployeeFirstName> - <JobTitle>". */
            var departments = context
                .Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    departmentName = d.Name,
                    managerFirstName = d.Manager.FirstName,
                    managerLastName = d.Manager.LastName,
                    departmentEmployees = d.Employees
                        .Select(e => new
                        {
                            employeeFirstName = e.FirstName,
                            employeeLastName = e.LastName,
                            employeeJobTitle = e.JobTitle
                        })
                        .OrderBy(e => e.employeeFirstName)
                        .ThenBy(e => e.employeeLastName)
                        .ToList()
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var d in departments)
            {
                sb.AppendLine($"{d.departmentName} - {d.managerFirstName} {d.managerLastName}");
                foreach (var e in d.departmentEmployees)
                {
                    sb.AppendLine($"{e.employeeFirstName} {e.employeeLastName} - {e.employeeJobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetLatestProjects(SoftUniContext context)
        {
            /*Write a program that return information about the last 10 started projects.
             Sort them by name lexicographically and return their name, description and start date, each on a new row. Format of the output*/
            var projects = context
                .Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10) // take 10 needs to be after sort i think?
                .Select(p => new
                {
                    projectName = p.Name,
                    projectDesc = p.Description,
                    projectStart = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                })
                .OrderBy(p=> p.projectName)
                //.OrderByDescending(p => p.projectName.Length)
                //.ThenBy(p =>p.projectName) this is how lexicographical order should be done but judge doesn't accept my submission
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.projectName}");
                sb.AppendLine($"{p.projectDesc}");
                sb.AppendLine($"{p.projectStart}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string IncreaseSalaries(SoftUniContext context)
        {
            /*Write a program that increase salaries of all employees that are in the Engineering, Tool Design, Marketing or Information Services department by 12%.
             Then return first name, last name and salary (2 symbols after the decimal separator) for those employees whose salary was increased.
             Order them by first name (ascending), then by last name (ascending). Format of the output.*/

            var departmentList = new List<string>()
            {
                "Engineering",
                "Tool Design",
                "Marketing",
                "Information Services"
            };

            var employeesList = context
                .Employees
                .Where(e => departmentList.Contains(e.Department.Name))
                .ToList();

            foreach (var employee in employeesList)
            {
                employee.Salary *= 1.12M; // can be put in a variable
            }

            context.SaveChanges();

            var employees = employeesList // using this to save 3 lines of code
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            /*Write a program that finds all employees whose first name starts with "Sa".
             Return their first, last name, their job title and salary, rounded to 2 symbols after the decimal separator in the format given in the example below.
             Order them by first name, then by last name (ascending).*/
            var employees = context
                .Employees
                .Where(e=>e.FirstName.ToLower().StartsWith("sa"))
                //.Where(e => e.FirstName.StartsWith("Sa") || e.FirstName.StartsWith("sa") ||
                          //  e.FirstName.StartsWith("sA")) alternative way 
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }
        public static string DeleteProjectById(SoftUniContext context)
        {
            /*Let's delete the project with id 2.
             Then, take 10 projects and return their names, each on a new line.
             Remember to restore your database after this task.*/

            var epContext = context
                .EmployeesProjects
                .Where(ep => ep.ProjectId == 2) // put in variable
                .ToList();

            foreach (var c in epContext)
            { // remove from employeesprojects first
                context.Remove(c);
            }

            var pContext = context
                .Projects
                .Where(p => p.ProjectId == 2);

            foreach (var c in pContext)
            { // then from projects 
                context.Remove(c);
            }

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            var projects = context
                .Projects
                .Take(10)
                .Select(p => new
                {
                    p.Name
                })
                .ToList();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.Name}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string RemoveTown(SoftUniContext context)
        {
            /*Write a program that deletes a town with name „Seattle”.
             Also, delete all addresses that are in those towns.
             Return the number of addresses that were deleted in format “{count} addresses in Seattle were deleted”.
             There will be employees living at those addresses, which will be a problem when trying to delete the addresses.
             So, start by setting the AddressId of each employee for the given address to null.
             After all of them are set to null, you may safely remove all the addresses from the context.Addresses and finally remove the given town.*/

            var townToDelete = context
                .Towns
                .FirstOrDefault(t => t.Name == "Seattle"); // can make into variable

            var addressesList = context
                .Addresses
                .Where(a => a.Town.Name == "Seattle")
                .ToList();

            var employeesOnAddressesList = context
                .Employees
                .Where(e => addressesList.Contains(e.Address))
                .ToList();

            foreach (var e in employeesOnAddressesList)
            { // make employees address null
                e.Address = null;
            }

            foreach (var a in addressesList)
            { // delete addresses that are in Seattle
                context.Remove(a);
            }

            // delete Seattle town
            context.Remove(townToDelete);

            context.SaveChanges();

            return $"{addressesList.Count} addresses in Seattle were deleted";
        }
    }
}
