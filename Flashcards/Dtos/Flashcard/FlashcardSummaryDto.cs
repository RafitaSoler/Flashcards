namespace Flashcards.Dtos.Flashcard
{
    internal class FlashcardSummaryDto
    {
        public int Id { get; set; } = 0;
        public string StackName { get; set; } = "";
        public string Front { get; set; } = "";
        public string Back { get; set; } = "";
    }
}
