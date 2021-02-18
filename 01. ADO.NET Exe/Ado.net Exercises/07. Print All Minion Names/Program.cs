using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace _07._Print_All_Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            connection.Open();


            using (connection)
            {
                // select all minions
                SqlCommand command = new SqlCommand("SELECT Name FROM Minions", connection);
                SqlDataReader reader = command.ExecuteReader();


                List<string> names = new List<string>();
                while (reader.Read())
                {
                    // put them in a list
                    string name = (string)reader["Name"];
                    names.Add(name);
                }


                for (int i = 0; i < names.Count / 2; i++)
                {
                    // print first
                    Console.WriteLine(names[i]);
                    // print last
                    Console.WriteLine(names[names.Count - 1 - i]);
                }

                // if odd number of minions print the other one because we always print in batches of two
                if (names.Count % 2 == 1)
                {
                    Console.WriteLine(names[names.Count / 2]);
                }
            }
        }
    }
}
