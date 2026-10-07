using System.Globalization;
using Microsoft.Data.Sqlite;
using Spectre.Console;

namespace TCSA.HabitLogger;

internal class Program
{
    private static string connectionString = "Data Source=HabitLogger.db";
    private static CultureInfo _cultureInfo = new ("en-US");
    static void Main(string[] args)
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                
                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS Habits (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                NAME TEXT,
                UNIT TEXT
                                          )";
                tableCmd.ExecuteNonQuery();
                
                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS HabitRecords (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                HabitId INTEGER,
                Date TEXT,
                Quantity INTEGER,
                FOREIGN KEY (HabitId) REFERENCES Habits(ID)
                                          )";
                tableCmd.ExecuteNonQuery();

                tableCmd.CommandText =
                    @"SELECT EXISTS(SELECT 1 FROM Habits)";

                int result = Convert.ToInt32(tableCmd.ExecuteScalar());
                int[] habitIds = new int[3];
                
                if (result == 0)
                {
                    tableCmd.CommandText =
                        "INSERT INTO Habits(name, unit) VALUES ($name, $unit)";
                    
                    tableCmd.Parameters.AddWithValue("$name", "Reading");
                    tableCmd.Parameters.AddWithValue("$unit", "pages");
                    tableCmd.ExecuteNonQuery();
                    
                    tableCmd.CommandText =
                        "SELECT last_insert_rowid()";
                    
                    int firstId = Convert.ToInt32(tableCmd.ExecuteScalar());
                    habitIds[0] = firstId;

                    tableCmd.CommandText =
                        "INSERT INTO Habits(name, unit) VALUES ($name, $unit)";
                    tableCmd.Parameters.Clear();
                    tableCmd.Parameters.AddWithValue("$name", "Walking");
                    tableCmd.Parameters.AddWithValue("$unit", "steps");
                    tableCmd.ExecuteNonQuery();
                    
                    tableCmd.CommandText =
                        "SELECT last_insert_rowid()";
                    
                    int secondId = Convert.ToInt32(tableCmd.ExecuteScalar());
                    habitIds[1] = secondId;
                    
                    tableCmd.CommandText =
                        "INSERT INTO Habits(name, unit) VALUES ($name, $unit)";
                    tableCmd.Parameters.Clear();
                    tableCmd.Parameters.AddWithValue("$name", "Water");
                    tableCmd.Parameters.AddWithValue("$unit", "glasses");
                    tableCmd.ExecuteNonQuery();
                    
                    tableCmd.CommandText =
                        "SELECT last_insert_rowid()";
                    
                    int thirdId = Convert.ToInt32(tableCmd.ExecuteScalar());
                    habitIds[2] = thirdId;
                    
                    tableCmd.CommandText = 
                        @"INSERT INTO HabitRecords(HabitId, Date, Quantity) VALUES  ($habitId, $date, $quantity)";
                    
                    for (int i = 0; i < 100; i++)
                    {
                        tableCmd.Parameters.Clear();
                        tableCmd.Parameters.AddWithValue("$habitId", habitIds[Random.Shared.Next(3)]);
                        tableCmd.Parameters.AddWithValue("$date", (DateTime.Today - TimeSpan.FromDays(Random.Shared.Next(30))).ToString("dd-MM-yy"));
                        tableCmd.Parameters.AddWithValue("$quantity", Random.Shared.Next(1,1000));
                        
                        tableCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error creating the database");
                return;
            }
        }
        
        GetUserInput();
    }
    
    static void GetUserInput()
    {
        while (true)
        {
            Console.Clear();
            AnsiConsole.Write( 
                new FigletText("Habit Logger") 
                    .Centered() 
                    .Color(Color.Green)); 
            
            var command = AnsiConsole.Prompt(
                new SelectionPrompt<string>() 
                    .Title("[green]What would you like to do?[/]")
                    .AddChoices( 
                        "View All Records", 
                        "Insert Record", 
                        "Delete Record", 
                        "Update Record", 
                        "Create Habit", 
                        "Exit"));

            switch (command)
            {
                case "View All Records":
                    GetAllRecords();
                    PressAnyKey();
                    break;
                case "Insert Record":
                    Insert();
                    PressAnyKey();
                    break;
                case "Delete Record":
                    Delete();
                    PressAnyKey();
                    break;
                case "Update Record":
                    Update();
                    PressAnyKey();
                    break;
                case "Create Habit":
                    CreateHabit();
                    PressAnyKey();
                    break;
                case "Exit": 
                    AnsiConsole.MarkupLine("\n[green]Goodbye![/]\n"); 
                    return;
            }
        }
    }

    private static void PressAnyKey()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void GetAllRecords()
    {
        Console.Clear();

        using (var connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    @"SELECT HabitRecords.ID, Habits.NAME, HabitRecords.Date, HabitRecords.Quantity, Habits.UNIT 
                      FROM HabitRecords
                      JOIN Habits ON HabitRecords.HabitId = Habits.ID
                      ORDER BY HabitRecords.ID";

                using var reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var name = reader.GetString(1);
                        var date = DateTime.ParseExact(reader.GetString(2), "dd-MM-yy", _cultureInfo);
                        var quantity = reader.GetInt32(3);
                        var unit = reader.GetString(4);

                        Console.WriteLine($"{id} - {name} - {date:dd-MMM-yyyy} - Quantity: {quantity}  {unit}");
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("There was an error reading the database");
            }
        }
    }

    private static void GetAllHabits()
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    @"SELECT Id, Name, Unit FROM Habits";

                using var reader = tableCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string unit = reader.GetString(2);

                        Console.WriteLine($"{id} - {name} - {unit}");
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("There was an error reading the database");
        }
    }
    
    private static void CreateHabit()
    {
        string name;
        string unit;

        Console.Clear();
        Console.WriteLine("Create a new habit");
        Console.WriteLine("------------------");

        while (true)
        {
            Console.Write("Enter habit name: ");
            name = Console.ReadLine()!;
            if(string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name can not be empty."); continue; }
            
            Console.Write("Enter unit (e.g. glasses, steps, pages): ");
            unit = Console.ReadLine()!;
            if(string.IsNullOrWhiteSpace(unit)) { Console.WriteLine("Unit can not be empty."); continue; }
            
            break;
        }

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    "INSERT INTO Habits(name, unit) VALUES ($name, $unit)";

                tableCmd.Parameters.AddWithValue("$name", name);
                tableCmd.Parameters.AddWithValue("$unit", unit);

                tableCmd.ExecuteNonQuery();
            }
            
            Console.WriteLine();
            Console.WriteLine($"Habit '{name}' created successfully!");
        }
        catch (Exception)
        {
            Console.WriteLine("There was an error creating a new habit");
        }
    }

    private static void Delete()
    {
        GetAllRecords();

        while (true)
        {
            var recordId =
                GetNumberInput(
                    "\n\nPlease type the Id of the record you want to delete or type 0 to go back to Main Menu\n\n");
            
            if(recordId == 0) return;

            using (var connection = new SqliteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var tableCmd = connection.CreateCommand();
                    tableCmd.CommandText = "DELETE FROM HabitRecords WHERE Id = $id";
                    
                    tableCmd.Parameters.AddWithValue("$id", recordId);
                    
                    int rowCount = tableCmd.ExecuteNonQuery();

                    if (rowCount > 0)
                    {
                        Console.WriteLine($"\n\nRecord with Id {recordId} was deleted.");
                        return;
                    }

                    Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist. \n\n");
                }
                catch (Exception)
                {
                    Console.WriteLine("There was an error deleting the record");
                    return;
                }
            }
        }
    }

    private static void Insert()
    {
        Console.Clear();
        
        GetAllHabits();
        int habitId = GetNumberInput("\n\nEnter habit ID or type 0 to return to Main Menu:\n\n");
        if (habitId == 0) return;
        string unit = "";

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    "SELECT Id, Name, Unit FROM Habits  WHERE Id = $id";

                tableCmd.Parameters.AddWithValue("$id", habitId);

                using var reader = tableCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(1);
                        unit = reader.GetString(2);

                        Console.WriteLine($"{habitId} - {name} - {unit}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID entered.");
                    return;
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("There was an error inserting the record");
            return;
        }
        
        string date = GetDateInput();
        
        if(date == "0") return; 
        
        int quantity = GetNumberInput($"\n\nPlease enter quantity in {unit}:\n\n");
        if (quantity == 0) return;
        
        using (var connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                    "INSERT INTO HabitRecords(habitId, date, quantity) VALUES ($habitId, $date, $quantity)";

                tableCmd.Parameters.AddWithValue("$habitId", habitId);
                tableCmd.Parameters.AddWithValue("$date", date);
                tableCmd.Parameters.AddWithValue("$quantity", quantity);

                tableCmd.ExecuteNonQuery();
                Console.WriteLine("\n\nRecord was added.");
            }
            catch (Exception)
            {
                Console.WriteLine("The record could not be saved");
            }
        }
    }

    internal static void Update()
    {
        GetAllRecords();

        while (true)
        {
            var recordId =
                GetNumberInput(
                    "\n\nPlease type Id of the record would like to update. Type 0 to return to Main Menu.\n\n");
            
            if (recordId == 0) return;

            using (var connection = new SqliteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    var checkCmd = connection.CreateCommand();
                    checkCmd.CommandText = "SELECT EXISTS(SELECT 1 FROM HabitRecords WHERE Id = $id)";
                    
                    checkCmd.Parameters.AddWithValue("$id", recordId);
                    
                    int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (checkQuery == 0)
                    {
                        Console.WriteLine($"\n\nRecord with Id {recordId} doesn't exist.\n\n");
                        continue;
                    }
                    
                    var checkUnitCmd = connection.CreateCommand();
                    checkUnitCmd.CommandText =
                        @"SELECT Unit FROM HabitRecords
                          JOIN Habits ON HabitRecords.HabitId = Habits.ID
                          WHERE HabitRecords.ID = $id";
                    
                    checkUnitCmd.Parameters.AddWithValue("$id", recordId);

                    string unit = checkUnitCmd.ExecuteScalar()!.ToString()!;

                    string date = GetDateInput();
                    if (date == "0") return;

                    int quantity =
                        GetNumberInput(
                            $"\n\nPlease enter quantity in {unit}\n\n");
                    if (quantity == 0) return;

                    var tableCmd = connection.CreateCommand();
                    tableCmd.CommandText =
                        "UPDATE HabitRecords SET date = $date, quantity = $quantity WHERE Id = $id";
                    
                    tableCmd.Parameters.AddWithValue("$date", date);
                    tableCmd.Parameters.AddWithValue("$quantity", quantity);
                    tableCmd.Parameters.AddWithValue("$id", recordId);

                    tableCmd.ExecuteNonQuery();
                    Console.WriteLine($"\n\nRecord with Id {recordId} was updated.");
                    return;
                }
                catch (Exception)
                {
                    Console.WriteLine("There was an error updating the record");
                    return;
                }
            }
        }
    }
    internal static string GetDateInput()
    {
        while (true)
        {
            Console.WriteLine(
                "\n\nPlease insert the date: (Format: dd-mm-yy) or T for today. Type 0 to return to Main Menu.\n\n");
            
            string dateInput = Console.ReadLine() ?? "";
            dateInput = dateInput.Trim().ToLower();

            if (dateInput == "0") return "0";
            if (dateInput == "t") dateInput = DateTime.Today.ToString("dd-MM-yy");

            if (DateTime.TryParseExact(dateInput, "dd-MM-yy", _cultureInfo, DateTimeStyles.None, out _))
            {
                return dateInput;
            }
            
            Console.WriteLine(
                "\n\nInvalid date. (Format: dd-mm-yy). Type 0 to return to Main Menu or try again:\n\n");
        }
    }

    internal static int GetNumberInput(string message)
    {
        NumberValidator numberValidator = new NumberValidator();
        
        while (true)
        {
            Console.WriteLine(message);

            string? numberInput = Console.ReadLine();
            
            if (numberInput == "0") return 0;
            
            if (numberValidator.ValidateNumber(numberInput!, out int number))
            {
                return number;
            }
            
            Console.WriteLine("\n\nInvalid number. Try again.\n\n");
        }
    }
}
