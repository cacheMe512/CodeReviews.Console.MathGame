using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;

bool exit = false;
int gameCount = 0;
string[] operations = { "+", "-", "*", "/" };
List<string> games = new List<string>();
List<(string Difficulty, int Min, int Max)> difficultyLevels = new List<(string, int, int)>
{
    ("Easy", 1, 10),
    ("Normal", 5, 20),
    ("Hard", 10, 50)
};
(string Difficulty, int Min, int Max) currentDifficulty = difficultyLevels[1];

var menuOptions = new Dictionary<int, string>
{
    { 1, "Summation" },
    { 2, "Subtraction" },
    { 3, "Multiplication" },
    { 4, "Division" },
    { 5, "Random Mode" },
    { 6, "Show History" },
    { 7, "Change Difficulty" },
    { 8, "Exit" }
};

// Loop the game until user selects exit
while (!exit)
{
    string equation = "";
    string operation = "";
    int solution;

    try
    {
        int menuSelection = GetMenuSelection();

        switch (menuSelection)
        {
            case 1:
                operation = "+";
                break;
            case 2:
                operation = "-";
                break;
            case 3:
                operation = "*";
                break;
            case 4:
                operation = "/";
                break;
            case 5:
                break;
            case 6:
                Console.WriteLine("\t\tGame History\n-------------------------------------------\n");
                foreach (string game in games)
                    Console.WriteLine(game);
                break;
            case 7:
                SelectGameDifficulty();
                break;
            default:
                exit = true;
                break;
        }

        // Handle gameplay
        if (menuSelection > 0 && menuSelection < 6)
        {
            try
            {
                if(menuSelection == 5)
                    equation = GetRandomEquation();
                else
                    equation = GetEquation(operation);
                Console.WriteLine($"{equation}??");

                Stopwatch stopwatch = Stopwatch.StartNew();
                int guess = GetUserGuess();
                stopwatch.Stop();

                solution = SolveEquation(equation);

                TimeSpan timeTaken = stopwatch.Elapsed;

                games.Add($"Game {gameCount + 1}\nEquation: {equation}\nTime Taken: {timeTaken.TotalSeconds:F2} seconds\n");

                if (guess != solution)
                {
                    Console.WriteLine("Please Try Again.");
                    Console.WriteLine($"Correct answer is: {solution}");

                    games[gameCount] += $"Result: Game Lost\n";
                    games[gameCount] += $"You Guessed: {guess}\n";
                    games[gameCount] += $"The correct answer was: {solution}\n";
                }
                else
                {
                    Console.WriteLine("You guessed correctly!");

                    games[gameCount] += $"Result: Game Won\n";
                    games[gameCount] += $"You Guessed: {guess}\n";
                }
                gameCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during equation processing: {ex.Message}");
            }
        }
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error occurred: {ex.Message}");
    }
}

string GetEquation(string operation)
{
    try
    {
        Random random = new();
        int min = currentDifficulty.Min;
        int max = currentDifficulty.Max;

        return $"{random.Next(min, max)} {operation} {random.Next(min, max)}";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error generating equation: {ex.Message}");
        throw;
    }
}

string GetRandomEquation()
{
    try
    {
        Random randomOperand = new();
        Random randomOperation = new();
 
        int min = currentDifficulty.Min;
        int max = currentDifficulty.Max;

        int operand1 = randomOperand.Next(min, max);
        int operand2 = randomOperand.Next(min, max);
        string operation = operations[randomOperation.Next(4)];

        if(operation == "/" && operand2 == 0)
            operand2 = randomOperand.Next(1, max);

        return $"{operand1} {operation} {operand2}";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error generating equation: {ex.Message}");
        throw;
    }
}

int SolveEquation(string equation)
{
    try
    {
        string[] expressions = equation.Split();
        string operation = expressions[1];
        int operand1;
        int operand2;
        int result = 0;

        if (!int.TryParse(expressions[0], out operand1) || !int.TryParse(expressions[2], out operand2))
        {
            throw new FormatException("Invalid equation format.");
        }

        if (operation == "+")
            result = operand1 + operand2;
        else if (operation == "-")
            result = operand1 - operand2;
        else if (operation == "*")
            result = operand1 * operand2;
        else if (operation == "/")
        {
            if (operand2 == 0)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }
            result = operand1 / operand2;
        }

        return result;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error solving equation: {ex.Message}");
        throw;
    }
}

int GetMenuSelection()
{
    try
    {
        int choice = 0;
        do
        {
            string input = "";
            bool validNumber = false;

            foreach (var option in menuOptions)
                    Console.WriteLine($"{option.Key}. {option.Value}");

            input = Console.ReadLine();
            validNumber = int.TryParse(input, out choice);

            if (validNumber && choice > 0 && choice < 9)
                break;

        } while (true);

        return choice;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error getting menu selection: {ex.Message}");
        throw;
    }
}

int GetUserGuess()
{
    try
    {
        int guess = 0;
        while(true)
        {
            Console.Write("Please enter your guess: ");
            string input = Console.ReadLine()?.Trim();

            if (int.TryParse(input, out guess))
                return guess;
            Console.WriteLine("\nInvalid input. Please try again.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError getting user guess: {ex.Message}");
        throw;
    }
}

void SelectGameDifficulty()
{
    int selectedIndex = 1;
    ConsoleKey key;

    do
    {
        Console.Clear();
        Console.WriteLine("Use Up/Down arrows to select a difficulty level. Press Enter to confirm.\n");

        for(int i = 0; i < difficultyLevels.Count; i++)
        {
            if (i == selectedIndex)
            {
                Console.WriteLine($"> {difficultyLevels[i].Difficulty} <");
            }
            else
            {
                Console.WriteLine($"  {difficultyLevels[i].Difficulty}");
            }
        }

        key = Console.ReadKey(true).Key;

        if(key == ConsoleKey.UpArrow && selectedIndex > 0)
            selectedIndex--;
        else if(key == ConsoleKey.DownArrow && selectedIndex < difficultyLevels.Count - 1)
            selectedIndex++;
    } while(key != ConsoleKey.Enter);

    currentDifficulty = difficultyLevels[selectedIndex];
    Console.Clear();
}
