using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CSharpEFAdo.netDemo
{
    class StartUp
    {
        static void Main(string[] args)
        {
            SqlConnection dbConnection =
                new SqlConnection(@"Server=.\SQLEXPRESS; Database=SoftUni; Integrated Security=true");
            dbConnection.Open();

            using (dbConnection)
            {
                SqlCommand command =
                    new SqlCommand("SELECT FirstName, LastName, d.Name FROM Employees e" + "JOIN Departments d ON e.DepartmentId = d.DepartmentId", dbConnection);
                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        string firstName = (string)reader["FirstName"];
                        string lastName = (string)reader["LastName"];
                        string departmentName = (string)reader["Name"];
                        Console.WriteLine($"{firstName} {lastName} - {departmentName}");
                    }

                }
            }
        }
    }
}
