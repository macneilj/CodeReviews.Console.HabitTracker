using Microsoft.Data.Sqlite;
using System.Globalization;

namespace HabitTracker
{
    class HabitTracker
    {
        static bool closeApp = false;
        static string connectionsString = @"Data Source=habit-tracker.db";

        internal class DrinkingWater
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public int Quantity { get; set; }
        }

        static void Main(string[] args)
        {

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

            RunApp();
        }

        static void RunApp()
        {

            while (closeApp == false)
            {
                Console.Clear();
                Console.WriteLine("Habit Tracker");
                Console.WriteLine("1. Add water intake");
                Console.WriteLine("2. View water intake");
                Console.WriteLine("3. Update water intake");
                Console.WriteLine("4. Delete water intake");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");
                string userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "1":
                        InsertRecord();
                        break;
                    case "2":
                        ViewWaterIntake();
                        break;
                    case "3":
                        UpdateRecord();
                        break;
                    case "4":
                        DeleteRecord();
                        break;
                    case "5":
                        closeApp = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void UpdateRecord()
        {
            Console.Clear();
            GetRecords();

            Console.Write("Enter the Id of the record you want to delete, or type 0 to return to the menu: ");

            string idInput = Console.ReadLine();
            
            if (idInput == "0") return;


            using (var connection = new SqliteConnection(connectionsString))
            {
                connection.Open();
                
                using (SqliteCommand command = new("SELECT * FROM drinking_water WHERE Id = @idInput", connection))
                {
                    command.Parameters.Add("@idInput", SqliteType.Integer).Value = idInput;

                    SqliteDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        List<DrinkingWater> tableData = new();

                        Console.WriteLine("Please enter a new record to replace the old one:");

                        string date = GetDateInput();
                        if (date == null) { ReturnToMenu(); return; }

                        string quantity = GetQuantityInput();
                        if (quantity == null || quantity == "0") { ReturnToMenu(); return; }

                        using (SqliteCommand updateCmd = new("UPDATE drinking_water SET date = @Date, quantity = @Quantity WHERE Id = @idInput", connection))
                        {
                            //updateCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = '{quantity}' WHERE Id = {idInput}";

                            updateCmd.Parameters.Add("@Date", SqliteType.Integer).Value = date;
                            updateCmd.Parameters.Add("@Quantity", SqliteType.Integer).Value = quantity;
                            updateCmd.Parameters.Add("@idInput", SqliteType.Integer).Value = idInput;

                            if (updateCmd.ExecuteNonQuery() > 0)
                            {
                                Console.WriteLine("Record upddated successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to add record. Please try again.");
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine("No record found with the given Id.");

                    }

                }

            }

            ReturnToMenu();

        }

        private static void DeleteRecord()
        {
            Console.Clear();
            GetRecords();

            Console.Write("Enter the Id of the record you want to delete, or type 0 to return to the menu: ");

            string idInput = Console.ReadLine();

            if (idInput == "0") return;

            using (var connection = new SqliteConnection(connectionsString))
            {
                connection.Open();

                using (SqliteCommand command = new("DELETE FROM drinking_water WHERE Id = @idInput", connection))
                {
                    //tableCmd.CommandText = $"DELETE FROM drinking_water WHERE Id = {idInput}";

                    command.Parameters.Add("@idInput", SqliteType.Integer).Value = idInput;

                    if (command.ExecuteNonQuery() > 0)
                    {
                        Console.WriteLine("Record deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No record found with the given Id.");
                    }
                }
                
            }

            ReturnToMenu();

        }

        private static void GetRecords()
        {

            using (var connection = new SqliteConnection(connectionsString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yyyy", new CultureInfo("en-US")),
                                Quantity = reader.GetInt32(2)
                            });
                    }

                }
                else
                {
                    Console.WriteLine("No records found. Please hydrate yourself");
                }

                connection.Close();

                foreach (var record in tableData)
                {
                    Console.WriteLine($"Id: {record.Id} | Date: {record.Date.ToString("dd-MM-yyyy")} | Quantity: {record.Quantity} cups");
                }

            }
        }

        private static void ViewWaterIntake()
        {
            Console.Clear();

            Console.WriteLine("Water Intake Records:");

            GetRecords();

            ReturnToMenu();
        }

        private static void InsertRecord()
        {

            Console.Clear();

            string date = GetDateInput();
            if (date == null) { ReturnToMenu(); return; }

            string quantity =  GetQuantityInput();
            if (quantity == null || quantity == "0") { ReturnToMenu(); return; }

            using (var connection = new SqliteConnection(connectionsString))
            {
                connection.Open();

                using (SqliteCommand command = new("INSERT INTO drinking_water(date, quantity) VALUES (@Date, @Quantity)", connection))
                {

                    command.Parameters.Add("@Date", SqliteType.Text).Value = date;
                    command.Parameters.Add("@Quantity", SqliteType.Integer).Value = quantity;

                    if (command.ExecuteNonQuery() > 0)
                    {
                        Console.WriteLine("Record added successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to add record. Please try again.");
                    }
                }

            }

            ReturnToMenu();

        }

        private static void ReturnToMenu(string? message = null)
        {
            if(message is not null) Console.WriteLine(message);
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        private static string GetQuantityInput()
        {
            Console.Write("Enter the water you drank, or type 0 to return to the menu: ");

            string quantityInput = Console.ReadLine();

            if(int.TryParse(quantityInput, out int quantityValue))
            {
                if (quantityValue < 0)
                {
                    quantityInput = null;
                    Console.WriteLine("Quantity cannot be negative.");
                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey();
                }
            }
            else
            {
                quantityInput = null;
                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }

            return quantityInput;
        }

        internal static string GetDateInput()
        {
            Console.Write("Enter the date (dd-mm-yyyy), type 0 to return to the menu: ");

            string dateInput = Console.ReadLine();
            bool parsedDate = DateTime.TryParseExact(dateInput, "dd-MM-yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out DateTime dateValue);

            if (!parsedDate)
            {
                dateInput = null;
                Console.WriteLine("Invalid date format. Please use dd-mm-yyyy format.");
            }

            return dateInput;

        }
    }
}