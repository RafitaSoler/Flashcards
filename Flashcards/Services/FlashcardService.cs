using Flashcards.Data.Repositories;
using Flashcards.Dtos.Flashcard;

namespace Flashcards.Services
{
    internal static class FlashcardService
    {
        internal static List<FlashcardSummaryDto> GetAllFlashcards()
        {
            return FlashcardRepository.GetAllFlashcardsWithStackName();
        }

        internal static List<FlashcardSummaryDto> GetFlashcardsForStack(int stackId)
        {
            return FlashcardRepository.GetByStackId(stackId);
        }

        internal static int CreateFlashcard(CreateFlashcardDto dto)
        {
            return FlashcardRepository.InsertFlashcard(dto);
        }

        internal static void UpdateFlashcard(UpdateFlashcardDto dto)
        {
            FlashcardRepository.UpdateFlashcard(dto);
        }

        internal static void DeleteFlashcard(int flashcardId)
        {
            FlashcardRepository.DeleteFlashcard(flashcardId);
        }
    }
}
