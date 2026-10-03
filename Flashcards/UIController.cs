using Spectre.Console;

namespace Flashcards
{
    internal static class UIController
    {
        internal enum MenuOption
        {
            StudySession,
            CreateFlashcard,
            CreateStack,
            UpdateFlashcard,
            DeleteFlashcard,
            DeleteStack,
            ExitApplication
        }

        private static Dictionary<string, MenuOption> options = new()
        {
            ["Study Session"] = MenuOption.StudySession,
            ["Create Flashcard"] = MenuOption.CreateFlashcard,
            ["Create Stack"] = MenuOption.CreateStack,
            ["Update Flashcard"] = MenuOption.UpdateFlashcard,
            ["Delete Flashcard"] = MenuOption.DeleteFlashcard,
            ["Delete Stack"] = MenuOption.DeleteStack,
            ["Exit Application"] = MenuOption.ExitApplication
        };

        private static MenuOption SelectMenuOption()
        {
            bool optionSelected = false;
            MenuOption option = default;
            while (!optionSelected)
            {
                string selected = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .HighlightStyle(Style.Parse("cyan"))
                    .Title("Main Menu")
                    .AddChoices(options.Keys)
                );
                optionSelected = options.TryGetValue(selected, out option);
            }
            return option;
        }

        internal static void MainMenu()
        {
            bool exitApp = false;
            while(!exitApp)
            {
                AnsiConsole.Clear();
                MenuOption option = SelectMenuOption();
                switch(option)
                {
                    case MenuOption.StudySession:
                        break;
                    case MenuOption.CreateFlashcard:
                        CreateFlashcard();
                        break;
                    case MenuOption.CreateStack:
                        CreateStack();
                        break;
                    case MenuOption.UpdateFlashcard:
                        UpdateFlashcard();
                        break;
                    case MenuOption.DeleteFlashcard:
                        DeleteFlashcard();
                        break;
                    case MenuOption.DeleteStack:
                        DeleteStack();
                        break;
                    case MenuOption.ExitApplication:
                        exitApp = true;
                        AnsiConsole.Clear();
                        break;
                    default:
                        break;
                }
            }
        }

        internal static void CreateFlashcard()
        {

        }

        internal static void CreateStack()
        {

        }

        internal static void UpdateFlashcard()
        {

        }

        internal static void DeleteFlashcard()
        {

        }

        internal static void DeleteStack()
        {

        }

    }
}
