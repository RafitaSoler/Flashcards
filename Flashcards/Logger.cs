using System.Runtime.CompilerServices;

namespace Flashcards
{
    internal class Logger
    {
        private static readonly string _path = $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

        public static void Log(string message,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            string fileName = Path.GetFileName(filePath);
            File.AppendAllText(_path, $"{DateTime.Now} [{fileName}:{lineNumber} {memberName}()]: {message}{Environment.NewLine}");
        }
    }
}