using System;
using Microsoft.Data.SqlClient;

namespace _03._Minion_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            var villainId = int.Parse(Console.ReadLine());

            connection.Open();


            using (connection)
            {

                var findVillainQuery = @"SELECT Name FROM Villains 
                                        WHERE Id = @Id";

                var findVillainCommand = new SqlCommand(findVillainQuery, connection);

                findVillainCommand.Parameters.AddWithValue("@Id", villainId);

                using (findVillainCommand)
                {
                    var villainName = findVillainCommand.ExecuteScalar();

                    try
                    {
                        if (villainName == null)
                        {
                            throw new ArgumentException($"No villain with ID {villainId} exists in the database.");
                        }
                    }
                    catch (ArgumentException ae)
                    {
                        Console.WriteLine(ae.Message);

                        return;
                    }

                    Console.WriteLine(villainName);
                }

                var findVillainMinionsQuery =
                    @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                         m.Name, 
                         m.Age
                            FROM MinionsVillains AS mv
                                    JOIN Minions As m 
                                        ON mv.MinionId = m.Id
                          WHERE mv.VillainId = @Id
                  ORDER BY m.Name";

                var findVillainMinionsCommand = new SqlCommand(findVillainMinionsQuery, connection);

                findVillainMinionsCommand.Parameters.AddWithValue("@Id", villainId);

                using (findVillainMinionsCommand)
                {
                    var minionsReader = findVillainMinionsCommand.ExecuteReader();

                    try
                    {
                        using (minionsReader)
                        {
                            if (!minionsReader.HasRows)
                            {
                                throw new ArgumentException("(no minions)");
                            }

                            while (minionsReader.Read())
                            {
                                Console.WriteLine($"{minionsReader["RowNum"]}. {minionsReader["Name"]}");
                            }
                        }
                    }
                    catch (ArgumentException ae)
                    {
                        Console.WriteLine(ae.Message);
                    }
                }

            }
        }
    }
}
