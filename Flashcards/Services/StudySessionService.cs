using Flashcards.Data.Repositories;
using Flashcards.Dtos.StudySession;

namespace Flashcards.Services
{
    internal static class StudySessionService
    {
        internal static List<StudySessionSummaryDto> GetAllStudySessions()
        {
            return StudySessionRepository.GetAllStudySessions();
        }

        internal static List<MonthlySessionCountDto> GetMonthlySessionCount(int stackId, int year)
        {
            var results = StudySessionRepository.GetMonthlySessionCount(stackId, year).ToDictionary(r => r.Month);
            var full = new List<MonthlySessionCountDto>();
            for(int month = 1; month <= 12; month++)
            {
                full.Add(results.TryGetValue(month, out var found)
                    ? found
                    : new MonthlySessionCountDto { Month = month, SessionCount = 0}
                );
            }
            return full;
        }

        internal static List<MonthlyAverageScoreDto> GetMonthlyAverageScore(int stackId, int year)
        {
            var results = StudySessionRepository.GetAverageScoresByMonth(stackId, year).ToDictionary(r => r.Month);
            var full = new List<MonthlyAverageScoreDto>();
            for (int month = 1; month <= 12; month++)
            {
                full.Add(results.TryGetValue(month, out var found)
                    ? found
                    : new MonthlyAverageScoreDto { Month = month, AverageScore = 0 }
                );
            }
            return full;
        }

        internal static int CreateStudySession(CreateStudySessionDto dto)
        {
            return StudySessionRepository.InsertStudySession(dto);
        }

        internal static void DeleteStudySession(int studySessionId)
        {
            StudySessionRepository.DeleteStudySession(studySessionId);
        }
    }
}
