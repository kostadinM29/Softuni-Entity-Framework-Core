using System;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace _08._Increase_Minion_Age
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            connection.Open();

            int[] ids = Console.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            using (connection)
            {
                foreach (int id in ids)
                {
                    // update the age of each minion individually 
                    SqlCommand command = new SqlCommand("UPDATE Minions SET Age += 1 WHERE Id = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }

                //read changed minions
                SqlCommand selectCommand = new SqlCommand("SELECT * FROM Minions", connection);
                SqlDataReader reader = selectCommand.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Name"]} - {reader["Age"]}");
                }
            }
        }
    }
}
