using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Flashcards.Data
{
    internal static class DatabaseManager
    {
        private static IConfiguration _config = null!;
        private static string _connectionString = string.Empty;

        internal static void SetConfiguration(IConfiguration configuration)
        {
            _config = configuration;
            _connectionString = _config.GetConnectionString("DefaultConnection")!;
        }

        internal static SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        internal static void Start()
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                string query =
                    """
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stacks')
                    BEGIN
                    CREATE TABLE Stacks (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(100) UNIQUE NOT NULL
                    )
                    END;
                
                    """;
                connection.Execute(query);
                Logger.Log($"Executing query: {query}");

                query =
                    """
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Flashcards')
                    BEGIN
                    CREATE TABLE Flashcards (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        StackId INT NOT NULL,
                        Front NVARCHAR(100) NOT NULL,
                        Back NVARCHAR(100) NOT NULL,
                        FOREIGN KEY (StackId) REFERENCES Stacks(Id) ON DELETE CASCADE
                    )
                    END;
                    """;
                connection.Execute(query);
                Logger.Log($"Executing query: {query}");
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static void Initialize()
        {
            if(IsEmpty())
            {
                
            }
        }

        private static bool IsEmpty()
        {
            using SqlConnection connection = CreateConnection();
            int count = connection.ExecuteScalar<int>("SELECT EXISTS (SELECT 1 FROM Stacks)");
            return count == 0;
        }
    }
}
