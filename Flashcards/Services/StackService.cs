using Flashcards.Data.Repositories;
using Flashcards.Dtos.Stack;

namespace Flashcards.Services
{
    internal static class StackService
    {
        internal static List<StackSummaryDto> GetAllStacks()
        {
            return StackRepository.GetAllStacksWithCardCount();
        }


        internal static int CreateStack(CreateStackDto dto)
        {
            if(StackRepository.NameExists(dto.Name))
            {
                throw new InvalidOperationException($"A stack named '{dto.Name}' already exists.");
            }
            return StackRepository.InsertStack(dto);
        }

        internal static void UpdateStack(UpdateStackDto dto)
        {
            StackRepository.UpdateStack(dto);
        }

        internal static void DeleteStack(int stackId)
        {
            StackRepository.DeleteStack(stackId);
        }
    }
}
