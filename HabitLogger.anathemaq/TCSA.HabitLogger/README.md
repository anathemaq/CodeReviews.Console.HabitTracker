# Habit Logger

A console application for tracking habits by quantity using SQLite and ADO.NET.

## Features

* Create custom habits with a name and unit of measurement.
* Add habit records with a date and quantity.
* View all logged habit records.
* Update existing records.
* Delete records.
* Store all data in a SQLite database.
* Automatically create the database and required tables if they do not exist.
* Seed the database with sample habits and 100 records on the first setup.
* Validate user input and handle database errors without crashing the application.
* Use parameterized SQL queries to prevent SQL injection.
* Unit tests for number input validation.
* Console menu built with Spectre.Console.

## Technologies

* C#
* .NET
* SQLite
* ADO.NET
* Microsoft.Data.Sqlite
* Spectre.Console
* xUnit

## Getting Started

### Requirements

* [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the application

```bash
git clone <repository-url>
cd TCSA.HabitLogger/TCSA.HabitLogger
dotnet run
```

On the first run the application creates `HabitLogger.db` in the current working directory and fills it with sample data. To start with a fresh database, close the application and delete `HabitLogger.db`.

### Run the tests

From the solution folder:

```bash
dotnet test
```

## How to Use

* Use the arrow keys and Enter to choose an option in the main menu.
* Enter dates in the `dd-mm-yy` format, or type `T` to use today's date.
* Type `0` at any prompt to return to the main menu.

## How It Works

The application stores habits in the `Habits` table and their logged occurrences in the `HabitRecords` table.

Each habit has:

* Name
* Unit

Each habit record has:

* Habit ID
* Date
* Quantity

The application uses ADO.NET to communicate with the SQLite database. Entity Framework and Dapper are not used.

## Architectural Choices

* **Two tables instead of one table per habit.** `Habits` stores the habit name and unit, `HabitRecords` stores the occurrences and points to a habit through `HabitId`. A new habit is just a new row, so the database structure never changes.
* **All code in one class.** Following the KISS principle and the project tips, the whole application lives in `Program.cs`. The only separate class is `NumberValidator`, so it can be unit tested without a database.
* **Dates stored as text.** SQLite has no dedicated date type, so dates are saved as text in the `dd-MM-yy` format and parsed back with `DateTime.ParseExact`.
* **A new connection for every operation.** Each method opens its own `SqliteConnection` inside a `using` block, so the connection is always closed, even after an error.
* **Errors are caught, not thrown.** Every database call is wrapped in `try/catch` and shows a message instead of crashing the application.

## Development Process

I started with the basic requirements and gradually added the functionality needed for the project.

During development I implemented:

1. SQLite database and table creation.
2. Habit and habit record operations.
3. Input validation.
4. Parameterized SQL queries.
5. Unit tests.
6. Custom habit creation and units.
7. Seed data for the database.
8. A console menu using Spectre.Console.

## What I Learned

The main thing I learned from this project was how to work with a real database from a C# application.

In particular, I learned how to:

* Create and work with a SQLite database.
* Create tables using SQL.
* Connect to SQLite from C# using ADO.NET.
* Execute SQL commands from C#.
* Read data using `SqliteDataReader`.
* Insert, update and delete records.
* Use parameters in SQL queries.
* Work with relationships between tables.
* Seed a database with initial data.
* Write basic unit tests.

## Difficulties

The biggest new area for me was working with the database and connecting SQLite to a C# application using ADO.NET.

The hardest problem was a bug that was not in the database at all. After seeding, "View All Records" showed only 83 records, while the database had 100. I spent a lot of time checking the SQL query, the `JOIN` between the tables and the date parsing, because I was sure some rows were being lost when reading them.

In the end the query returned all 100 records. The problem was in the console: after printing the list, the method returned to the main menu right away. The menu called `Console.Clear()` and drew the title and the menu options, and in my terminal they covered the first lines of the list. 17 lines of menu, 100 - 17 = 83.

The fix was a simple "Press any key to continue..." pause. Later I noticed that the same thing happened to other messages, like "Record was deleted", so I added a pause after every menu action. This taught me that when the data looks wrong, I should first check what the database really returns, and only then look for the problem in the code that shows it.

## What Was Easy

The basic console input and menu logic were familiar from previous projects. Creating and validating user input was also relatively straightforward.

## Conclusion

This project gave me practical experience working with a database from a C# console application and helped me understand how application code interacts with stored data.
