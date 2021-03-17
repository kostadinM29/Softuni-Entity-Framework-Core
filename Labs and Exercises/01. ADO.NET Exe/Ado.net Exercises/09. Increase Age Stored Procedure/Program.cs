using System;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

namespace _09._Increase_Age_Stored_Procedure
{
    class Program
    {
        static void Main(string[] args)
        {
            // input ids
            int[] inputIds = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            connection.Open();

            using (connection)
            {
                foreach (int id in inputIds)
                {
                    //execute the stored procedure (we don't have to create it in SSMS)
                    SqlCommand command = new SqlCommand("EXEC usp_GetOlder @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }

                // output all minions
                SqlCommand selectCommand = new SqlCommand("SELECT * FROM Minions", connection);
                SqlDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Name"]} - {reader["Age"]} old years old");
                }

            }
        }
    }
}
