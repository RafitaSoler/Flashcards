namespace Flashcards.Dtos.Flashcard
{
    internal class UpdateFlashcardDto
    {
        public int Id { get; set; } = 0;
        public int StackId { get; set; } = 0;
        public string Front { get; set; } = "";
        public string Back { get; set; } = "";
    }
}
