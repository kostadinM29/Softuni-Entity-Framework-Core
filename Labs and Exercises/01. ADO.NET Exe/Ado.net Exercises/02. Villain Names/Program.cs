using System;
using Microsoft.Data.SqlClient;

namespace _02._Villain_Names
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            connection.Open();

            using (connection)
            {
                var villainNameQuery = @"SELECT 
	                                        v.Name,
	                                        COUNT(VillainId) AS [MinionsCount]
                                        FROM Villains v
	                                        JOIN MinionsVillains mv ON v.Id = mv.MinionId
                                        GROUP BY v.Name
	                                        HAVING COUNT(VillainId) > 3
                                        ORDER BY MinionsCount DESC";

                var villainNamesCommand = new SqlCommand(villainNameQuery, connection);

                using (villainNamesCommand)
                {
                    var dataReader = villainNamesCommand.ExecuteReader();

                    using (dataReader)
                    {
                        while (dataReader.Read())
                        {
                            var villainName = dataReader["Name"];
                            var minionsCount = dataReader["MinionsCount"];
                            Console.WriteLine($"{villainName} - {minionsCount}");
                        }
                    }
                }
            }
        }
    }
}
