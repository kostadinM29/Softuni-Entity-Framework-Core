using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace _05._Change_Town_Names_Casing
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the country: ");
            var inputCountry = Console.ReadLine();

            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            connection.Open();

            using (connection)
            {
                // select cities in country
                var command = new SqlCommand(@"SELECT t.Name FROM Countries c 
                                                     JOIN Towns AS t ON t.CountryCode = c.Id 
                                                     WHERE c.Name = @searchedCountry", connection);

                command.Parameters.AddWithValue("@searchedCountry", inputCountry);

                var reader = command.ExecuteReader();
                // create cities list to save an updated query search
                var towns = new List<string>();
                while (reader.Read())
                {
                    // add towns that were changed to a list
                    var town = (string)reader["Name"];
                    towns.Add(town.ToUpper());
                }

                if (towns.Count > 0)
                {
                    // if any cities were changed
                    Console.WriteLine($"{towns.Count} town names were affected.");
                    Console.WriteLine($"[{string.Join(", ", towns)}]");
                }
                else
                {
                    // if country doesn't exist or has no cities connected to it 
                    Console.WriteLine("No town names were affected.");
                }
            }
        }

    }
}
