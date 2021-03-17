using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace CSharpEFAdo.netDemo
{
    class StartUp
    {
        static void Main(string[] args)
        {
            // sql injection test

            Console.Write("Enter email: ");
            var email = Console.ReadLine();
            Console.Write("Enter birth date: ");
            var birthDate = Console.ReadLine();


            var dbcon = new SqlConnection(@"Server=.\SQLEXPRESS; Database=TripService; Integrated Security=true");
            using (dbcon)
            {
                dbcon.Open();
                var sqlCommand =
                    new SqlCommand($"SELECT COUNT(*) FROM Accounts WHERE Email = @Email AND BirthDate = @BirthDate;",dbcon);
                sqlCommand.Parameters.Add(new SqlParameter("@Email", email));
                sqlCommand.Parameters.Add(new SqlParameter("@BirthDate", birthDate));
                var usersCount = (int) sqlCommand.ExecuteScalar();
               string message =  usersCount > 0 ? "Access granted!" : "Access denied!";
               Console.WriteLine(message);
            }


            // ado.net test
            SqlConnection dbConnection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=SoftUni; Integrated Security=true");
            dbConnection.Open();

            using (dbConnection)
            {
                SqlCommand command = new SqlCommand("SELECT FirstName, LastName, d.Name FROM Employees e" +
                                                    "JOIN Departments d ON e.DepartmentId = d.DepartmentId", dbConnection);
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
