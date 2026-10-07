using Dapper;
using Flashcards.Dtos.Flashcard;
using Microsoft.Data.SqlClient;

namespace Flashcards.Data.Repositories
{
    internal static class FlashcardRepository
    {
        internal static List<FlashcardSummaryDto> GetAllFlashcardsWithStackName()
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query =
                    """
                    SELECT f.Id, s.Name AS StackName, f.Front, f.Back
                    FROM Flashcards f
                    LEFT JOIN Stacks s ON s.Id = f.StackId
                    """;
                return connection.Query<FlashcardSummaryDto>(query).ToList();
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static List<FlashcardSummaryDto> GetByStackId(int stackId)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query =
                    """
                    SELECT f.Id, s.Name AS StackName, f.Front, f.Back
                    FROM Flashcards f
                    LEFT JOIN Stacks s ON s.Id = f.StackId
                    WHERE f.StackId = @Id
                    """;
                return connection.Query<FlashcardSummaryDto>(query, new { Id = stackId }).ToList();
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static int InsertFlashcard(CreateFlashcardDto dto)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query = "INSERT INTO Flashcards (StackId, Front, Back) OUTPUT INSERTED.Id VALUES (@StackId, @Front, @Back)";
                int id = connection.Execute(query, dto);
                Logger.Log($"Executing query: {query}");
                return id;
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static void UpdateFlashcard(UpdateFlashcardDto dto)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query = "UPDATE Flashcards SET StackId = @StackId, Front = @Front, Back = @Back WHERE Id = @Id";
                int id = connection.ExecuteScalar<int>(query, dto);
                Logger.Log($"Executing query: {query}");
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static void DeleteFlashcard(int flashcardId)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query = "DELETE FROM Flashcards WHERE Id = @Id";
                connection.Execute(query, new { Id = flashcardId });
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
