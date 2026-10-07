using Flashcards.Dtos.Flashcard;
using Flashcards.Dtos.Stack;
using Flashcards.Dtos.StudySession;
using Flashcards.Models;
using Flashcards.Services;
using Spectre.Console;
using System.Collections;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Flashcards
{
    internal static class UIController
    {
        internal enum MainMenuOption
        {
            StudySession,
            ViewStudySessions,
            ManageFlashcards,
            ExitApplication
        }

        internal enum ManageMenuOption
        {
            ViewStacks,
            ViewFlashcards,
            CreateStack,
            CreateFlashcard,
            UpdateStack,
            UpdateFlashcard,
            DeleteStack,
            DeleteFlashcard,
            Back
        }

        private static readonly int CANCEL_OPTION = -1;

        private static MainMenuOption SelectMainMenuOption()
        {
            bool optionSelected = false;
            MainMenuOption option = default;
            while (!optionSelected)
            {
                option = AnsiConsole.Prompt(
                    new SelectionPrompt<MainMenuOption>()
                    .HighlightStyle(Style.Parse("cyan"))
                    .Title("Main Menu")
                    .UseConverter(option => option switch
                    {
                        MainMenuOption.StudySession => "Study",
                        MainMenuOption.ViewStudySessions => "View Study Sessions",
                        MainMenuOption.ManageFlashcards => "Manage Flashcards and Stacks",
                        MainMenuOption.ExitApplication => "Exit",
                        _ => option.ToString()
                    })
                    .AddChoices(Enum.GetValues<MainMenuOption>())
                    .WrapAround()
                );
                optionSelected = true;
            }
            return option;
        }

        private static ManageMenuOption SelectManageMenuOption()
        {
            bool optionSelected = false;
            ManageMenuOption option = default;
            while (!optionSelected)
            {
                option = AnsiConsole.Prompt(
                    new SelectionPrompt<ManageMenuOption>()
                    .HighlightStyle(Style.Parse("cyan"))
                    .Title("Main Menu")
                    .UseConverter(option => option switch
                    {
                        ManageMenuOption.ViewStacks => "View Stacks",
                        ManageMenuOption.ViewFlashcards => "View Flashcards",
                        ManageMenuOption.CreateStack => "Create Stack",
                        ManageMenuOption.CreateFlashcard => "Create Flashcard",
                        ManageMenuOption.UpdateStack => "Update Stack",
                        ManageMenuOption.UpdateFlashcard => "Update Flashcard",
                        ManageMenuOption.DeleteStack => "Delete Stack",
                        ManageMenuOption.DeleteFlashcard => "Delete Flashcard",
                        ManageMenuOption.Back => "Go Back",
                        _ => option.ToString()
                    })
                    .AddChoices(Enum.GetValues<ManageMenuOption>())
                    .WrapAround()
                );
                optionSelected = true;
            }
            return option;
            
        }

        internal static void MainMenu()
        {
            bool exitApp = false;
            while(!exitApp)
            {
                AnsiConsole.Clear();
                MainMenuOption option = SelectMainMenuOption();
                switch(option)
                {
                    case MainMenuOption.StudySession:
                        StudySession();
                        break;
                    case MainMenuOption.ViewStudySessions:
                        ViewStudySessions();
                        break;
                    case MainMenuOption.ManageFlashcards:
                        ManageMenu();
                        break;
                    case MainMenuOption.ExitApplication:
                        exitApp = true;
                        AnsiConsole.Clear();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void ManageMenu()
        {
            bool back = false;
            while (!back)
            {
                AnsiConsole.Clear();
                ManageMenuOption option = SelectManageMenuOption();
                switch (option)
                {
                    case ManageMenuOption.ViewStacks:
                        ViewStacks();
                        break;
                    case ManageMenuOption.ViewFlashcards:
                        ViewFlashcards();
                        break;
                    case ManageMenuOption.CreateStack:
                        CreateStack();
                        break;
                    case ManageMenuOption.CreateFlashcard:
                        CreateFlashcard();
                        break;
                    case ManageMenuOption.UpdateStack:
                        UpdateStack();
                        break;
                    case ManageMenuOption.UpdateFlashcard:
                        UpdateFlashcard();
                        break;
                    case ManageMenuOption.DeleteStack:
                        DeleteStack();
                        break;
                    case ManageMenuOption.DeleteFlashcard:
                        DeleteFlashcard();
                        break;
                    case ManageMenuOption.Back:
                        back = true;
                        AnsiConsole.Clear();
                        break;
                    default:
                        break;
                }
            }
        }

        private static void StudySession()
        {
            StackSummaryDto stack = PromptStackSelection("Select a stack to study.");
            if (stack.Id != CANCEL_OPTION)
            {
                string method = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("Start Session.")
                    .HighlightStyle(Style.Parse("cyan"))
                    .AddChoices("Start", "Cancel")
                    .WrapAround()
                );
                if(method == "Cancel")
                {
                    return;
                }

                List<FlashcardSummaryDto> flashcards = FlashcardService.GetFlashcardsForStack(stack.Id);
                var shuffledCards = flashcards.Shuffle().ToList();
                DateTime date = DateTime.Now;
                int score = 0;
                int index = 0;
                foreach (FlashcardSummaryDto card in shuffledCards)
                {
                    AnsiConsole.WriteLine($"Cards left: {shuffledCards.Count - index}");
                    index++;
                    AnsiConsole.MarkupLine($"Score: {score}/{index}");
                    AnsiConsole.Write(new Panel(card.Front).Header("Front").Expand());
                    AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .HighlightStyle(Style.Parse("cyan"))
                        .AddChoices("Check answer")
                        .WrapAround()
                    );
                    AnsiConsole.Write(new Panel(card.Back).Header("Back").Expand());
                    bool correct = AnsiConsole.Confirm("Did you get it correct?");
                    if (correct)
                    {
                        score++;
                    }
                    AnsiConsole.Clear();
                }
                AnsiConsole.MarkupLine($"Final Score: [lime]{score}/{index}[/] correct");
                if(index != score)
                {
                    AnsiConsole.MarkupLine($"[red]{index - score} mistakes[/]");
                }
                bool save = AnsiConsole.Confirm("Save study session?");
                if(save)
                {
                    StudySessionService.CreateStudySession(new CreateStudySessionDto { StackId = stack.Id, Date = date, CorrectCount = score, TotalCount = index });
                    AnsiConsole.WriteLine("Study session saved");
                    PromptGoBack();
                }
            }
        }

        internal static void ViewStudySessions()
        {
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("What do you want to view?")
                .HighlightStyle(Style.Parse("cyan"))
                .AddChoices("View all sessions", "View number of sessions in a year", "View average score in a year", "Go Back")
                .WrapAround()
            );
            if(choice == "View all sessions")
            {
                ViewAllStudySessions();
            }
            if(choice == "View number of sessions in a year")
            {
                ViewMonthlySessionCount();
            }
            if(choice == "View average score in a year")
            {
                ViewMonthlyAverageScore();
            }
        }

        private static void ViewAllStudySessions()
        {
            List<StudySessionSummaryDto> stacks = StudySessionService.GetAllStudySessions();
            Table table = new Table().AddColumns("Id", "Stack Name", "Date", "Score");
            int fakeId = 1;
            foreach (StudySessionSummaryDto studySessionDto in stacks)
            {
                table.AddRow(fakeId.ToString(), studySessionDto.StackName, studySessionDto.Date.ToString(), studySessionDto.Score.ToString());
                fakeId++;
            }
            AnsiConsole.Clear();
            AnsiConsole.Write(table);
            PromptGoBack();
        }

        private static void ViewMonthlySessionCount()
        {
            StackSummaryDto stack = PromptStackSelection("Select a stack to view its sessions.");
            if (stack.Id != CANCEL_OPTION)
            {
                int year = AnsiConsole.Ask<int>("Write a year:");
                List<MonthlySessionCountDto> monthlyCounts = StudySessionService.GetMonthlySessionCount(stack.Id, year);
                Table table = new Table().Title($"Number of sessions per month for {year}").AddColumn("Stack Name");
                foreach (MonthlySessionCountDto dto in monthlyCounts)
                {
                    string monthName = new DateTime(year, dto.Month, 1).ToString("MMMM");
                    table.AddColumn(monthName);
                }
                string[] rowValues = monthlyCounts.Select(dto => dto.SessionCount.ToString()).Prepend(stack.Name).ToArray();
                table.AddRow(rowValues);

                AnsiConsole.Clear();
                AnsiConsole.Write(table);
                PromptGoBack();
            }
        }

        private static void ViewMonthlyAverageScore()
        {
            StackSummaryDto stack = PromptStackSelection("Select a stack to view its sessions.");
            if (stack.Id != CANCEL_OPTION)
            {
                int year = AnsiConsole.Ask<int>("Write a year:");
                List<MonthlyAverageScoreDto> monthlyAverages = StudySessionService.GetMonthlyAverageScore(stack.Id, year);

                Table table = new Table().Title($"Average score per month for {year}").AddColumn("Stack Name");
                foreach (MonthlyAverageScoreDto dto in monthlyAverages)
                {
                    string monthName = new DateTime(year, dto.Month, 1).ToString("MMMM");
                    table.AddColumn(monthName);
                }
                string[] rowValues = monthlyAverages.Select(dto => $"{dto.AverageScore:F1}%").Prepend(stack.Name).ToArray();
                table.AddRow(rowValues);

                AnsiConsole.Clear();
                AnsiConsole.Write(table);
                PromptGoBack();
            }
        }

        internal static void ViewStacks()
        {
            List<StackSummaryDto> stacks = StackService.GetAllStacks();
            Table table = new Table().AddColumns("Id", "Name", "Card Count");
            int fakeId = 1;
            foreach(StackSummaryDto stackDto in stacks)
            {
                table.AddRow(fakeId.ToString(), stackDto.Name, stackDto.CardCount.ToString());
                fakeId++;
            }
            AnsiConsole.Clear();
            AnsiConsole.Write(table);
            PromptGoBack();
        }

        internal static void ViewFlashcards()
        {
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("What do you want to view?")
                .HighlightStyle(Style.Parse("cyan"))
                .AddChoices("View all flashcards", "View flashcards in a stack", "Go Back")
                .WrapAround()
            );
            List<FlashcardSummaryDto> flashcards = null!;
            if(choice == "Go Back")
            {
                return;
            }
            if (choice == "View all flashcards")
            {
                flashcards = FlashcardService.GetAllFlashcards();
            }
            else if (choice == "View flashcards in a stack")
            {
                StackSummaryDto stack = PromptStackSelection("Select a stack to view its flashcards.");
                if (stack.Id != CANCEL_OPTION)
                {
                    flashcards = FlashcardService.GetFlashcardsForStack(stack.Id);
                }
                else
                {
                    return;
                }
            }
            Table table = new Table().AddColumns("Id", "Stack Name", "Front", "Back");
            int fakeId = 1;
            foreach (FlashcardSummaryDto flashcardDto in flashcards)
            {
                table.AddRow(fakeId.ToString(), flashcardDto.StackName, flashcardDto.Front, flashcardDto.Back);
                fakeId++;
            }
            AnsiConsole.Clear();
            AnsiConsole.Write(table);
            PromptGoBack();
        }

        internal static void CreateStack()
        {
            bool stackCreated = false;
            while (!stackCreated)
            {
                string stackName = AnsiConsole.Ask<string>("Write the name of the new stack");
                try
                {
                    CreateStackDto stackDto = new() { Name = stackName };
                    int newId = StackService.CreateStack(stackDto);
                    AnsiConsole.MarkupLine($"[green]Stack[/] [lime]{Markup.Escape(stackName)}[/] [green]created[/]");
                    stackCreated = true;
                }
                catch (InvalidOperationException e)
                {
                    AnsiConsole.MarkupLine($"[red]{e.Message}[/] Try a different name.");
                }
            }
            PromptGoBack();
        }

        internal static void CreateFlashcard()
        {
            StackSummaryDto stack = PromptStackSelection("Select a stack to insert the new flashcard.");
            if (stack.Id != CANCEL_OPTION)
            {
                bool flashcardCreated = false;
                while (!flashcardCreated)
                {
                    string front = AnsiConsole.Ask<string>("Write the question of the new flashcard");
                    string back = AnsiConsole.Ask<string>("Write the anwser of the new flashcard");
                    CreateFlashcardDto stackDto = new() { StackId = stack.Id, Front = front, Back = back };
                    int newId = FlashcardService.CreateFlashcard(stackDto);
                    AnsiConsole.MarkupLine($"[green]Flashcard[/] [lime]{Markup.Escape($"[{front} | {back}]")}[/] [green]created in Stack[/] [lime]{Markup.Escape(stack.Name)}[/]");
                    flashcardCreated = true;
                }
                PromptGoBack();
            }
        }

        internal static void UpdateStack()
        {
            StackSummaryDto stack = PromptStackSelection("Select a stack to change its name.");
            if (stack.Id != CANCEL_OPTION)
            {
                string newName = "";
                bool repeatedName = true;
                while (repeatedName)
                {
                    List<StackSummaryDto> stacks = StackService.GetAllStacks();
                    newName = AnsiConsole.Ask<string>("Write the new stack name:");
                    if (stacks.Exists(stack => stack.Name == newName))
                    {
                        AnsiConsole.MarkupLine("[red]A stack with that name already exists[/]");
                    }
                    else
                    {
                        repeatedName = false;
                    }
                }
                UpdateStackDto stackDto = new() { Id = stack.Id, Name = newName };
                StackService.UpdateStack(stackDto);
                AnsiConsole.MarkupLine($"[green]Stack updated[/]");
                PromptGoBack();
            }
        }

        internal static void UpdateFlashcard()
        {
            StackSummaryDto stack = PromptStackSelection("Select the stack of the flashcard to update.");
            if(stack.Id != CANCEL_OPTION)
            {
                FlashcardSummaryDto flashcard = PromptFlashcardSelection(stack.Id, "Select a flashcard to update.");
                if (flashcard.Id != CANCEL_OPTION)
                {
                    int stackToMoveId = stack.Id;
                    string newFront = flashcard.Front;
                    string newBack = flashcard.Back;

                    bool moveToStack = AnsiConsole.Confirm("Do you want to move the flashcard to another stack?");
                    if(moveToStack)
                    {
                        StackSummaryDto stackToMove = PromptStackSelection("Select a stack to move the flashcard.");
                        if (stackToMove.Id != CANCEL_OPTION)
                        {
                            stackToMoveId = stackToMove.Id;
                        }
                    }
                    bool changeFront = AnsiConsole.Confirm("Do you want to change the front of the flashcard?");
                    if(changeFront)
                    {
                        newFront = AnsiConsole.Ask<string>($"Write the new front: (Currently [cyan]{Markup.Escape(flashcard.Front)}[/])");
                    }
                    bool changeBack = AnsiConsole.Confirm("Do you want to change the back of the flashcard?");
                    if (changeBack)
                    {
                        newBack = AnsiConsole.Ask<string>($"Write the new back: (Currently [cyan]{Markup.Escape(flashcard.Back)}[/])");
                    }

                    UpdateFlashcardDto flashcardDto = new() { Id = flashcard.Id, StackId = stackToMoveId, Front = newFront, Back = newBack };
                    FlashcardService.UpdateFlashcard(flashcardDto);
                    AnsiConsole.MarkupLine($"[green]Flashcard updated[/]");
                    PromptGoBack();
                }
            }
        }

        internal static void DeleteStack()
        {
            StackSummaryDto stack = PromptStackSelection("Select a stack to delete.");
            if(stack.Id != CANCEL_OPTION)
            {
                bool delete = AnsiConsole.Confirm($"Do you want to remove Stack [cyan]{Markup.Escape(stack.Name)}[/] and all [cyan]{stack.CardCount}[/] cards in it? Study sessions of this stack will also get deleted.");
                if(delete)
                {
                    StackService.DeleteStack(stack.Id);
                    AnsiConsole.MarkupLine($"[red3]Stack[/] [red]{Markup.Escape(stack.Name)}[/] [red3]and[/] [red]{stack.CardCount}[/] [red3]cards removed. Study sessions deleted.[/]");
                }
                PromptGoBack();
            }
        }

        internal static void DeleteFlashcard()
        {
            StackSummaryDto stack = PromptStackSelection("Select the stack of the flashcard to delete.");
            if(stack.Id != CANCEL_OPTION)
            {
                FlashcardSummaryDto flashcard = PromptFlashcardSelection(stack.Id, "Select a flashcard to delete.");
                if(flashcard.Id != CANCEL_OPTION)
                {
                    bool delete = AnsiConsole.Confirm($"Do you want to remove Flashcard [cyan]{Markup.Escape(flashcard.Front)} | {Markup.Escape(flashcard.Back)}[/] in Stack [cyan]{Markup.Escape(stack.Name)}[/]?");
                    if (delete)
                    {
                        FlashcardService.DeleteFlashcard(flashcard.Id);
                        AnsiConsole.MarkupLine($"[red3]Flashcard[/] [red]{Markup.Escape(flashcard.Front)} | {Markup.Escape(flashcard.Back)}[/] [red3]removed.[/]");
                    }
                    PromptGoBack();
                }
            }
        }

        private static StackSummaryDto PromptStackSelection(string message)
        {
            List<StackSummaryDto> stacks = StackService.GetAllStacks();
            StackSummaryDto cancelOption = new() { Id = CANCEL_OPTION };
            stacks.Add(cancelOption);
            int maxLength = stacks.Max(s => s.Name.Length);
            StackSummaryDto stack = AnsiConsole.Prompt(
                new SelectionPrompt<StackSummaryDto>()
                .Title(message)
                .HighlightStyle(Style.Parse("cyan"))
                .UseConverter(stack => stack.Id == CANCEL_OPTION
                ? "[grey]Cancel[/]"
                : $"{stacks.IndexOf(stack) + 1, -4} | {stack.Name.PadRight(maxLength + 2)} | {stack.CardCount} cards")
                .AddChoices(stacks)
                .WrapAround()
            );
            stacks.Remove(cancelOption);
            return stack;
        }

        private static FlashcardSummaryDto PromptFlashcardSelection(int stackId, string message)
        {
            List<FlashcardSummaryDto> flashcards = FlashcardService.GetFlashcardsForStack(stackId);
            FlashcardSummaryDto cancelOption = new() { Id = CANCEL_OPTION };
            flashcards.Add(cancelOption);
            FlashcardSummaryDto flashcard = AnsiConsole.Prompt(
                new SelectionPrompt<FlashcardSummaryDto>()
                .Title(message)
                .HighlightStyle(Style.Parse("cyan"))
                .UseConverter(flashcard => flashcard.Id == CANCEL_OPTION
                ? "[grey]Cancel[/]"
                : $"{flashcards.IndexOf(flashcard) + 1} | {flashcard.Front} | {flashcard.Back}")
                .AddChoices(flashcards)
                .WrapAround()
            );
            flashcards.Remove(cancelOption);
            return flashcard;
        }

        private static void PromptGoBack()
        {
            AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .HighlightStyle(Style.Parse("cyan"))
                .AddChoices("Go Back")
            );
        }
    }
}
