using Microsoft.Data.Sqlite;

namespace HabitTracker
{
    class HabitTracker
    {
        static void Main(string[] args)
        {
            string connectionsString = @"Data Source=habit-tracker.db";

            using (var connection = new SqliteConnection(connectionsString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER
                )";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("Habit Tracker");
                Console.WriteLine("1. Add water intake");
                Console.WriteLine("2. View water intake");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option: ");
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        AddWaterIntake();
                        break;
                    case "2":
                        ViewWaterIntake();
                        break;
                    case "3":
                        closeApp = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}