using System;
using Microsoft.Data.SqlClient;

namespace _04._Add_Minion
{
    class Program
    {
        private static SqlCommand getMinionIdCommand;

        static void Main(string[] args)
        {
            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            connection.Open();

            var minionInfo = Console.ReadLine().Split(": ", StringSplitOptions.RemoveEmptyEntries)[1].Split(" ");

            var newMinionName = minionInfo[0];
            var newMinionAge = int.Parse(minionInfo[1]);
            var newMinionTown = minionInfo[2];

            var villainName = Console.ReadLine().Split(": ", StringSplitOptions.RemoveEmptyEntries)[1];

            using (connection)
            {
                // select town
                int townId = GetTownId(connection, newMinionTown);
                //select villain
                int villainId = GetVillainId(connection, villainName);

                if (townId == 0) // if town doesn't exist make new town with id 5
                {
                    InsertNewTown(connection, newMinionTown);
                    //select town again
                    townId = GetTownId(connection, newMinionTown);
                }
                if (villainId == 0) // if villain doesn't exist make new villain with evilness factor 4
                {
                    InsertNewVillain(connection, villainName);
                    //select villain again
                    villainId = GetVillainId(connection, villainName);
                }

                // create minion with townid
                InsertNewMinion(connection, townId, newMinionName, newMinionAge);
                //select new minion
                int minionId = GetMinionId(connection, newMinionName);
                //assign minion
                CreateConnectionBetweenVillainAndMinion(connection, minionId, villainId, newMinionName, villainName);
            }

        }
        public static int GetVillainId(SqlConnection sqlConnection, string villainName)
        {
            using var getVillainIdCommand = new SqlCommand(@"SELECT Id FROM Villains WHERE [Name] = @villainName", sqlConnection);
            getVillainIdCommand.Parameters.AddWithValue("@villainName", villainName);

            return (int)getVillainIdCommand.ExecuteScalar();
        }
        public static int GetTownId(SqlConnection sqlConnection, string minionTown)
        {
            using var getTownIdCommand = new SqlCommand(@"SELECT Id FROM Towns WHERE [Name] = @townName", sqlConnection);
            getTownIdCommand.Parameters.AddWithValue("@townName", minionTown);

            return (int)getTownIdCommand.ExecuteScalar();
        }
        private static int GetMinionId(SqlConnection connection, string newMinionName)
        {
            getMinionIdCommand = new SqlCommand("SELECT * FROM Minions WHERE Name = @newMinionName", connection);
            getMinionIdCommand.Parameters.AddWithValue("@newMinionName", newMinionName);

            return (int)getMinionIdCommand.ExecuteScalar();
        }
        private static void InsertNewTown(SqlConnection connection, string newMinionTown)
        {
            var townCommand = new SqlCommand("INSERT INTO Towns (Name, CountryId) VALUES (@newMinionTown, 5)", connection); // new town Id
            townCommand.Parameters.AddWithValue("@newMinionTown", newMinionTown);

            townCommand.ExecuteNonQuery();
            Console.WriteLine($"Town {newMinionTown} was added to the database.");
        }
        private static void InsertNewVillain(SqlConnection connection, string villainName)
        {
            var villainCommand =
                new SqlCommand("INSERT INTO Villains (Name, EvilnessFactorId) VALUES (@villainName, 4)", connection); // default factor - evil
            villainCommand.Parameters.AddWithValue("@villainName", villainName);
            villainCommand.ExecuteNonQuery();
            Console.WriteLine($"Villain {villainName} was added to the database.");
        }
        private static void InsertNewMinion(SqlConnection connection, int townId, string newMinionName, int newMinionAge)
        {
            var minionCommand = new SqlCommand("INSERT INTO Minions (Name, Age, TownId) VALUES (@newMinionName, @newMinionAge, @townId)", connection);
            minionCommand.Parameters.AddWithValue("@townId", townId);
            minionCommand.Parameters.AddWithValue("@newMinionName", newMinionName);
            minionCommand.Parameters.AddWithValue("@newMinionAge", newMinionAge);

            minionCommand.ExecuteNonQuery();
        }
        private static void CreateConnectionBetweenVillainAndMinion(SqlConnection connection, int minionId, int villainId, string newMinionName, string villainName)
        {
            var command = new SqlCommand("INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)", connection);
            command.Parameters.AddWithValue("@minionId", minionId);
            command.Parameters.AddWithValue("@villainId", villainId);

            command.ExecuteNonQuery();

            Console.WriteLine($"Successfully added {newMinionName} to be minion of {villainName}.");
        }
    }
}
