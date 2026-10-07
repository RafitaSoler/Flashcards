using Dapper;
using Flashcards.Dtos.Flashcard;
using Flashcards.Dtos.Stack;
using Flashcards.Dtos.StudySession;
using Flashcards.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Flashcards.Data
{
    internal static class DatabaseManager
    {
        private static IConfiguration _config = null!;
        private static string _connectionString = string.Empty;

        internal static void SetConfiguration(IConfiguration configuration)
        {
            _config = configuration;
            _connectionString = _config.GetConnectionString("DefaultConnection")!;
        }

        internal static SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        internal static void Start()
        {
            try
            {
                using SqlConnection connection = CreateConnection();
                string query =
                    """
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Stacks')
                    BEGIN
                    CREATE TABLE Stacks (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(100) UNIQUE NOT NULL
                    )
                    END;
                
                    """;
                connection.Execute(query);
                Logger.Log($"Executing query: {query}");

                query =
                    """
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Flashcards')
                    BEGIN
                    CREATE TABLE Flashcards (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        StackId INT NOT NULL,
                        Front NVARCHAR(100) NOT NULL,
                        Back NVARCHAR(100) NOT NULL,
                        FOREIGN KEY (StackId) REFERENCES Stacks(Id) ON DELETE CASCADE
                    )
                    END;
                    """;
                connection.Execute(query);
                Logger.Log($"Executing query: {query}");

                query =
                    """
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StudySessions')
                    BEGIN
                    CREATE TABLE StudySessions (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        StackId INT NOT NULL,
                        Date DATETIME2 NOT NULL,
                        CorrectCount INT NOT NULL,
                        TotalCount INT NOT NULL,
                        FOREIGN KEY (StackId) REFERENCES Stacks(Id) ON DELETE CASCADE
                    )
                    END;
                    """;
                connection.Execute(query);
                Logger.Log($"Executing query: {query}");
            }
            catch (Exception e)
            {
                Logger.Log($"Error: {e.Message}");
                throw;
            }
        }

        internal static void Initialize()
        {
            if(IsEmpty())
            {
                int spanishId = StackService.CreateStack(new CreateStackDto { Name = "Spanish" });

                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Dog", Back = "Perro" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Cat", Back = "Gato" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Horse", Back = "Caballo" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Cow", Back = "Vaca" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Pig", Back = "Cerdo" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Chicken", Back = "Gallina" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Bird", Back = "Pájaro" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Fish", Back = "Pez" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Rabbit", Back = "Conejo" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Mouse", Back = "Ratón" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Lion", Back = "León" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = spanishId, Front = "Tiger", Back = "Tigre" });

                int gameDevId = StackService.CreateStack(new CreateStackDto { Name = "Game Dev Terms" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = gameDevId, Front = "Delta time", Back = "Time elapsed since the last frame, used to make movement framerate-independent" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = gameDevId, Front = "Game loop", Back = "The core cycle: process input, update state, render, repeat" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = gameDevId, Front = "Draw call", Back = "A single instruction telling the GPU to render a batch of geometry" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = gameDevId, Front = "Culling", Back = "Skipping the rendering of objects not visible to the camera" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = gameDevId, Front = "Tick rate", Back = "How many times per second the game logic updates, independent of rendering" });

                int csharpId = StackService.CreateStack(new CreateStackDto { Name = "C# Basics" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = csharpId, Front = "What does 'static' mean on a method?", Back = "It belongs to the class itself, not to any instance" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = csharpId, Front = "What is boxing?", Back = "Wrapping a value type (like int) into an object on the heap" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = csharpId, Front = "What does 'using' do with IDisposable?", Back = "Guarantees Dispose() is called even if an exception is thrown" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = csharpId, Front = "Value type vs reference type?", Back = "Value types copy their data; reference types copy a pointer to shared data" });

                int mathId = StackService.CreateStack(new CreateStackDto { Name = "Math" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = mathId, Front = "7 + 5", Back = "12" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = mathId, Front = "9 - 4", Back = "5" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = mathId, Front = "6 x 3", Back = "18" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = mathId, Front = "20 / 4", Back = "5" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = mathId, Front = "8^2", Back = "64" });
                FlashcardService.CreateFlashcard(new CreateFlashcardDto { StackId = mathId, Front = "√81", Back = "9" });

                StudySessionService.CreateStudySession(new CreateStudySessionDto { StackId = 1, Date = DateTime.Now, CorrectCount = 12, TotalCount = 12 });
                StudySessionService.CreateStudySession(new CreateStudySessionDto { StackId = 2, Date = DateTime.Now.AddDays(-2).AddSeconds(12345), CorrectCount = 3, TotalCount = 5 });
                StudySessionService.CreateStudySession(new CreateStudySessionDto { StackId = 3, Date = DateTime.Now.AddDays(-5).AddSeconds(3214), CorrectCount = 1, TotalCount = 4 });
                StudySessionService.CreateStudySession(new CreateStudySessionDto { StackId = 4, Date = DateTime.Now.AddDays(-13).AddSeconds(4321), CorrectCount = 6, TotalCount = 6 });
            }
        }

        private static bool IsEmpty()
        {
            using SqlConnection connection = CreateConnection();
            int count = connection.ExecuteScalar<int>("SELECT CASE WHEN EXISTS (SELECT 1 FROM Stacks) THEN 1 ELSE 0 END;");
            return count == 0;
        }
    }
}
