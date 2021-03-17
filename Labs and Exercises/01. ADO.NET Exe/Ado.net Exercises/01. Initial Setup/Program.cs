using System;
using Microsoft.Data.SqlClient;

namespace _01._Initial_Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            var masterConnection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=master; Integrated Security=true");

            masterConnection.Open();
            // create database
            using (masterConnection)
            {
                var query = @"CREATE DATABASE MinionsDB";

                var createCommand = new SqlCommand(query, masterConnection);

                using (createCommand)
                {
                    createCommand.ExecuteNonQuery();

                    Console.WriteLine("DB created!");
                }
            }

            var connection = new SqlConnection(@"Server=.\SQLEXPRESS; Database=MinionsDB; Integrated Security=true");

            connection.Open();
            // create tables
            using (connection)
            {
                var tableCommand = new SqlCommand(
                    @"CREATE TABLE Countries
                            (
	                            Id INT PRIMARY KEY IDENTITY,
	                            Name NVARCHAR(50)
                            )
                            CREATE TABLE Towns
                            (
	                            Id INT PRIMARY KEY IDENTITY,
	                            Name NVARCHAR(50),
	                            CountryCode INT REFERENCES Countries(Id)
                            )
                            CREATE TABLE Minions
                            (
	                            Id INT PRIMARY KEY IDENTITY, 
	                            Name NVARCHAR(50), 
	                            Age INT, 
	                            TownId INT REFERENCES Towns(Id)
                            )
                            CREATE TABLE EvilnessFactors
                            (
	                            Id INT PRIMARY KEY IDENTITY, 
	                            Name NVARCHAR(50)
                            )
                             CREATE TABLE Villains 
                            (
	                            Id INT PRIMARY KEY IDENTITY, 
	                            Name NVARCHAR(50), 
	                            EvilnessFactorId INT REFERENCES EvilnessFactors(Id)
                            )
                            CREATE TABLE MinionsVillains 
                            (
	                            MinionId INT REFERENCES Minions(Id),
	                            VillainId INT REFERENCES Villains(Id),
	                            CONSTRAINT PK_MinionsVillains PRIMARY KEY (MinionId, VillainId)
                            )", connection);
                using (tableCommand)
                {
                    tableCommand.ExecuteNonQuery();

                    Console.WriteLine("Tables created!");
                }

                // insert data
                var insertCommand = new SqlCommand(@"INSERT INTO Countries ([Name]) 
                        VALUES ('Bulgaria'), 
                               ('England'), 
                               ('Cyprus'), 
                               ('Germany'), 
                               ('Norway')
                        INSERT INTO Towns ([Name], CountryCode) 
                        VALUES ('Plovdiv', 1),
                               ('Varna', 1),
                               ('Burgas', 1),
                               ('Sofia', 1),
                               ('London', 2),  
                               ('Southampton', 2),    
                               ('Bath', 2),  
                               ('Liverpool', 2),  
                               ('Berlin',  3),
                               ('Frankfurt', 3),   
                               ('Oslo', 4)
                
                        INSERT INTO Minions (Name,Age, TownId) 
                        VALUES ('Bob', 42, 3),
                               ('Kevin', 1, 1),        
                               ('Bob ', 32, 6),
                               ('Simon', 45, 3),    
                               ('Cathleen', 11, 2),  
                               ('Carry ', 50, 10), 
                               ('Becky', 125, 5),
                               ('Mars', 21, 1),
                               ('Misho', 5, 10),
                               ('Zoe', 125, 5),
                               ('Json', 21, 1)
                
                        INSERT INTO EvilnessFactors (Name) 
                        VALUES ('Super good'),  
                               ('Good'),      
                               ('Bad'),      
                               ('Evil'),
                               ('Super evil')
                        
                        INSERT INTO Villains (Name, EvilnessFactorId) 
                        VALUES ('Gru', 2),      
                               ('Victor', 1),       
                               ('Jilly', 3),
                               ('Miro', 4),
                               ('Rosen', 5),    
                               ('Dimityr', 1),   
                               ('Dobromir', 2)
                
                        INSERT INTO MinionsVillains (MinionId, VillainId) 
                        VALUES (4,2),
                               (1, 1),
                               (5, 7),
                               (3, 5),
                               (2, 6),
                               (11, 5),
                               (8, 4),
                               (9, 7),
                               (7, 1),
                               (1, 3),
                               (7, 3),
                               (5, 3),
                               (4, 3),
                               (1, 2),
                               (2, 1),
                               (2, 7)", connection);
                using (insertCommand)
                {
                    insertCommand.ExecuteNonQuery();

                    Console.WriteLine("Data inserted!");
                }
            }
        }
    }
}
