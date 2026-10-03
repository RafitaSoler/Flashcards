using Flashcards;
using Flashcards.Data;
using Microsoft.Extensions.Configuration;

class Program
{
    static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        DatabaseManager.SetConfiguration(config);
        DatabaseManager.Start();

        UIController.MainMenu();
    }
}