namespace Flashcards.Dtos.Flashcard
{
    internal class CreateFlashcardDto
    {
        public int StackId { get; set; }
        public string Front { get; set; } = "";
        public string Back { get; set; } = "";
    }
}
