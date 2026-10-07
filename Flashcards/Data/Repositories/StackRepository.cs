using Dapper;
using Flashcards.Dtos.Stack;
using Microsoft.Data.SqlClient;

namespace Flashcards.Data.Repositories
{
    internal static class StackRepository
    {
        internal static List<StackSummaryDto> GetAllStacksWithCardCount()
        {
            using SqlConnection connection = DatabaseManager.CreateConnection();
            string query =
                """
                SELECT s.Id, s.Name, Count(f.Id) AS CardCount
                FROM Stacks s
                LEFT JOIN Flashcards f ON s.Id = f.StackId
                GROUP BY s.Id, s.Name
                """;
            return connection.Query<StackSummaryDto>(query).ToList();
        }

        internal static bool NameExists(string name)
        {
            using SqlConnection connection = DatabaseManager.CreateConnection();
            int count = connection.ExecuteScalar<int>(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM Stacks WHERE Name = @Name) THEN 1 ELSE 0 END;",
                new { Name = name }
            );
            return count > 0;
        }

        internal static int InsertStack(CreateStackDto dto)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query = "INSERT INTO Stacks (Name) OUTPUT INSERTED.Id VALUES (@Name)";
                int id = connection.ExecuteScalar<int>(query, dto);
                Logger.Log($"Executing query: {query}");
                return id;
            }
            catch(Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static void UpdateStack(UpdateStackDto dto)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query = "UPDATE Stacks SET Name = @Name WHERE Id = @Id";
                int id = connection.ExecuteScalar<int>(query, dto);
                Logger.Log($"Executing query: {query}");
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static void DeleteStack(int stackId)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query = "DELETE FROM Stacks WHERE Id = @Id";
                connection.Execute(query, new { Id = stackId });
                Logger.Log($"Executing query: {query}");
                return;
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }
    }
}
