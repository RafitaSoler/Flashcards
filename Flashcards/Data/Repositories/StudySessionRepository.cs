using Dapper;
using Flashcards.Dtos.StudySession;
using Microsoft.Data.SqlClient;

namespace Flashcards.Data.Repositories
{
    internal static class StudySessionRepository
    {
        internal static List<StudySessionSummaryDto> GetAllStudySessions()
        {
            using SqlConnection connection = DatabaseManager.CreateConnection();
            string query =
                """
                SELECT ss.Id, s.Name AS StackName, ss.Date, ss.CorrectCount, ss.TotalCount
                FROM StudySessions ss
                LEFT JOIN Stacks s ON s.Id = ss.StackId
                """;
            return connection.Query<StudySessionSummaryDto>(query).ToList();
        }

        internal static List<MonthlySessionCountDto> GetMonthlySessionCount(int stackId, int year)
        {
            using SqlConnection connection = DatabaseManager.CreateConnection();
            string query =
                """
                SELECT MONTH(Date) AS Month, COUNT(*) AS SessionCount
                FROM StudySessions
                WHERE StackId = @StackId AND YEAR(Date) = @Year
                GROUP BY MONTH(Date)
                ORDER BY MONTH(Date)
                """;
            return connection.Query<MonthlySessionCountDto>(query, new { StackId = stackId, Year = year }).ToList();
        }

        internal static List<MonthlyAverageScoreDto> GetAverageScoresByMonth(int stackId, int year)
        {
            using SqlConnection connection = DatabaseManager.CreateConnection();
            string query =
                """
                SELECT MONTH(Date) AS Month, AVG(CAST(CorrectCount AS FLOAT) / TotalCount) * 100 AS AverageScore
                FROM StudySessions
                WHERE StackId = @StackId AND YEAR(Date) = @Year
                GROUP BY MONTH(Date)
                ORDER BY MONTH(Date)
                """;
            return connection.Query<MonthlyAverageScoreDto>(query, new { StackId = stackId, Year = year }).ToList();
        }

        internal static int InsertStudySession(CreateStudySessionDto dto)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query = "INSERT INTO StudySessions (StackId, Date, CorrectCount, TotalCount) OUTPUT INSERTED.Id VALUES (@StackId, @Date, @CorrectCount, @TotalCount)";
                int id = connection.ExecuteScalar<int>(query, dto);
                Logger.Log($"Executing query: {query}");
                return id;
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static void DeleteStudySession(int studySessionId)
        {
            try
            {
                using SqlConnection connection = DatabaseManager.CreateConnection();
                string query = "DELETE FROM StudySessions WHERE Id = @Id";
                connection.Execute(query, new { Id = studySessionId });
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
