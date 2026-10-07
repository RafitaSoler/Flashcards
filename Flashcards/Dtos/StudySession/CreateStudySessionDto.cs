namespace Flashcards.Dtos.StudySession
{
    internal class CreateStudySessionDto
    {
        public int StackId { get; set; } = 0;
        public DateTime Date { get; set; } = default;
        public int CorrectCount { get; set; } = 0;
        public int TotalCount { get; set; } = 0;
    }
}
