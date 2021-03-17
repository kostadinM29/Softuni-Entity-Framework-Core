using System;
using Microsoft.Data.SqlClient;

namespace _06._Remove_Villain
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the VillainId: ");
            var inputVillain = Console.ReadLine();

            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            connection.Open();

            using (connection)
            {
                // check if villain exists
                try
                {
                    var villainName = GetVillainId(connection, inputVillain);

                    // if exception isn't thrown delete first from mapping table
                    SqlCommand minionsVillainsCommand = new SqlCommand("DELETE FROM MinionsVillains WHERE VillainId = @villainId", connection);
                    minionsVillainsCommand.Parameters.AddWithValue("@villainId", inputVillain);

                    int releasedMinions = minionsVillainsCommand.ExecuteNonQuery();

                    // then from villains table
                    SqlCommand villainsCommand = new SqlCommand("DELETE FROM Villains WHERE Id = @villainId", connection);
                    villainsCommand.Parameters.AddWithValue("@villainId", villainsCommand);

                    villainsCommand.ExecuteNonQuery();

                    // output text
                    Console.WriteLine($"{villainName} was deleted.");
                    Console.WriteLine($"{releasedMinions} minions were released.");
                }
                catch (ArgumentException ae)
                {
                    // probably could do this without throwing an error
                    Console.WriteLine(ae);
                }
            }
        }
        public static string GetVillainId(SqlConnection sqlConnection, string villainId)
        {
            using var getVillainIdCommand = new SqlCommand(@"SELECT Name FROM Villains WHERE Id = @villainId", sqlConnection);
            getVillainIdCommand.Parameters.AddWithValue("@villainId", villainId);

            string villainName = "";

            var nameReader = getVillainIdCommand.ExecuteReader();
            using (nameReader)
            {
                while (nameReader.Read())
                {
                    villainName = (string)nameReader["Name"];
                }

            }
            if (string.IsNullOrWhiteSpace(villainName))
            {
                throw new ArgumentException("No such villain was found.");
            }

            return villainName;
        }
    }
}
