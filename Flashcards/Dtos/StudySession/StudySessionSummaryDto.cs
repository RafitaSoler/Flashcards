namespace Flashcards.Dtos.StudySession
{
    internal class StudySessionSummaryDto
    {
        public int Id { get; set; } = 0;
        public string StackName { get; set; } = "";
        public DateTime Date { get; set; } = default;
        public int CorrectCount { get; set; } = 0;
        public int TotalCount { get; set; } = 0;
        public string Score => $"{CorrectCount}/{TotalCount}";
    }
}
