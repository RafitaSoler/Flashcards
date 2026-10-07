namespace Flashcards.Models
{
    public class StudySession
    {
        public int Id { get; set; }
        public int StackId { get; set; }
        public DateTime Date { get; set; }
        public int CorrectCount { get; set; }
        public int TotalCount { get; set; }

        public StudySession(int id, int stackId, DateTime date, int correctCount, int totalCount)
        {
            Id = id;
            StackId = stackId;
            Date = date;
            CorrectCount = correctCount;
            TotalCount = totalCount;
        }
    }
}
