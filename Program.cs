//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Project:		Lab 2 - Advanced C# (Data Structures and Lambda Functions)
//	File Name:		Program.cs & VideoGame.cs
//
//	Description:	Reads data from a CSV file containing video game information and demonstrates
//	                various data structure functionalities using Lambda Functions.
//
//	Course:			CSCI 2910-800
//
//	Author:			Benjamin Wongmanee, wongmanee@etsu.edu, CSCI 2910 Student
//
//	Created:		Tuesday, September 12, 2023
//  Modified:       
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WongmaneeB_AdvancedCSharp;

public class Program
{
    private static string rootFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString();
    private static string fileName;

    private static List<VideoGame> vgList = new List<VideoGame>();

    private static Stack<string> userInputHistory = new Stack<string>();

    delegate KeyValuePair<string, int> RetrieveKeyValuePair(Dictionary<string, int> dictionary, string key);
    delegate int RetrieveIndex(Dictionary<string, int> dictionary, string key);
    delegate string RetrieveInputDescription(Stack<string> stack);
    delegate string RetrieveInputValue(Stack<string> stack);
    delegate IEnumerable<string> RetrieveTopFive(List<VideoGame> allVideoGames);
    private static Queue<Delegate> functionQueue = new Queue<Delegate>();

    public static void Main()
    {
        InputFile();
        EnqueueFunctions();

        Console.WriteLine("Mash <ENTER> to view all games in videogames.csv.");
        Console.ReadKey();
        SortByTitle();

        Console.WriteLine(Environment.NewLine);






        Console.WriteLine("Mash <ENTER> to view data based on a Publisher of your choice.");
        Console.ReadKey();
        Console.Clear();
        Dictionary<string, int> pubDictionary = DistinctSelect(true); // isPublisher == true.

        Console.Write(Environment.NewLine);

        Console.WriteLine("Mash <ENTER> to view data based on a Genre of your choice.");
        Console.ReadKey();
        Console.Clear();
        Dictionary<string, int> genDictionary = DistinctSelect(false); // isPublisher == false.

        Console.Write(Environment.NewLine);






        // ======================== BEHIND THE SCENES ======================== //
        Console.WriteLine("Mash <ENTER> to view BEHIND THE SCENES.");
        Console.ReadKey();
        Console.Clear();

        Console.Write(Environment.NewLine);

        string header = $"======================== BEHIND THE SCENES ========================";
        string footer = new string('=', header.Length);

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(header);
        Console.ResetColor();

        DictionaryImplementation(pubDictionary, header, footer);

        Console.WriteLine("Mash <ENTER> to view STACK IMPLEMENTATION.");
        Console.ReadKey();
        Console.Clear();
        StackImplementation(userInputHistory, header, footer);

        Console.WriteLine("Mash <ENTER> to view QUEUE IMPLEMENTATION.");
        Console.ReadKey();
        Console.Clear();
        QueueImplementation(functionQueue, header, footer);
    }






    // ======================== INPUT FILE ======================== //
    private static void InputFile()
    {
        // ========== DECLARE VARIABLES ========== //
        string filePath = $"{rootFolder}{Path.DirectorySeparatorChar}videogames.csv";

        fileName = Path.GetFileName(filePath);

        StreamReader sr = new StreamReader(filePath);

        List<string> lines = new List<string>();


        // ========== STREAMREADER ========== //
        using (sr)
        {
            sr.ReadLine(); // Cuts off the header

            while (!sr.EndOfStream)
            {
                lines.Add(sr.ReadLine());
            }

            for (int i = 0; i < lines.Count; i++)
            {
                // ========== BASIC PROPERTIES ========== //
                string vgName = lines[i].Split(',')[0];
                string vgPlatform = lines[i].Split(',')[1];
                int vgYear = ParseInt(lines[i].Split(',')[2]);
                string vgGenre = lines[i].Split(',')[3];
                string vgPublisher = lines[i].Split(',')[4];

                // ========== SALES PROPERTIES ========== //
                decimal vgNASales = ParseDecimal(lines[i].Split(',')[5]);
                decimal vgEUSales = ParseDecimal(lines[i].Split(',')[6]);
                decimal vgJPSales = ParseDecimal(lines[i].Split(',')[7]);
                decimal vgOtherSales = ParseDecimal(lines[i].Split(',')[8]);
                decimal vgGlobalSales = ParseDecimal(lines[i].Split(',')[9]);

                // ========== CREATE VG OBJECT ========== //
                VideoGame vg = new VideoGame(vgName, vgPlatform, vgYear, vgGenre, vgPublisher,
                    vgNASales, vgEUSales, vgJPSales, vgOtherSales, vgGlobalSales);

                vgList.Add(vg);
            }
        }

        sr.Close();
    }

    // ======================== PARSE METHODS ======================== //
    static int ParseInt(string valueAsString)
    {
        int valueAsInt;
        Int32.TryParse(valueAsString, out valueAsInt);
        return valueAsInt;
    }

    static decimal ParseDecimal(string valueAsString)
    {
        decimal valueAsDecimal;
        Decimal.TryParse(valueAsString, out valueAsDecimal);
        return valueAsDecimal;
    }






    // ======================== ENQUEUE LINQ FUNCTIONS ======================== //
    private static void EnqueueFunctions()
    {
        // ======================== DICTIONARY LINQ STATEMENTS ======================== //

        // ========== RETRIEVE KEY-VALUE PAIR FROM DICTIONARY ========== //
        functionQueue.Enqueue((RetrieveKeyValuePair)((dictionary, selectedKey) =>
        {
            var keyPair = dictionary
                .FirstOrDefault(kv => kv.Key == selectedKey);
            return keyPair;
        }));

        // ========== RETRIEVE KV PAIR INDEX FROM DICTIONARY ========== //
        functionQueue.Enqueue((RetrieveIndex)((dictionary, selectedKey) =>
        {
            int index = dictionary
                .ToList()
                .FindIndex(pair => pair.Key == selectedKey);
            return index;
        }));



        // ======================== STACK LINQ STATEMENTS ======================== //

        // ========== RETRIEVE INPUT DESCRIPTION FROM STACK ========== //
        functionQueue.Enqueue((RetrieveInputDescription)((stack) =>
        {
            string inputDescription = stack
                .Reverse()
                .FirstOrDefault();

            return inputDescription;
        }));

        // ========== RETRIEVE INPUT VALUE FROM STACK ========== //
        functionQueue.Enqueue((RetrieveInputValue)((stack) =>
        {
            string inputValue = stack
                .Reverse()
                .Skip(1)
                .FirstOrDefault();

            return inputValue;
        }));



        // ======================== QUEUE LINQ STATEMENTS ======================== //

        // ========== EXAMPLE LINQ STATEMENT (BEST-SELLING GAMES) ========== //
        functionQueue.Enqueue((RetrieveTopFive)((allVideoGames) =>
        {
            var topFiveBestSelling = vgList
                .OrderByDescending(vg => vg.GlobalSales)
                .Take(5)
                .Select(vg => vg.Name);

            return topFiveBestSelling;
        }));
    }






    // ======================== SORT BY TITLE ======================== //
    private static void SortByTitle()
    {
        vgList.Sort();
        foreach (VideoGame vg in vgList)
        {
            Console.WriteLine(vg);
        }
    }






    // ======================== DISPLAY ======================== //
    private static void DisplayData(bool isPublisher, string target)
    {
        List<VideoGame> targetVGList;

        if (isPublisher)
        {
            // ========== PUBLISHER LINQ FILTER & SORT ========== //
            targetVGList = vgList
            .Where(vg => vg.Publisher == target)
            .OrderBy(vg => vg.Name) // May be redundant if the entire list is already sorted A-Z.
            .ToList();
        }
        else
        {
            // ========== GENRE LINQ FILTER & SORT ========== //
            targetVGList = vgList
            .Where(vg => vg.Genre == target)
            .OrderBy(vg => vg.Name) // May be redundant if the entire list is already sorted A-Z.
            .ToList();
        }

        // ========== DISPLAY ========== //
        string header = $"======================== {target.ToUpper()} ========================";
        string footerText = $" END OF {target.ToUpper()} STREAM ";
        int borderPadding = (header.Length - footerText.Length) / 2;
        string footer = new string('=', borderPadding) + footerText + new string('=', borderPadding);

        Console.Write(Environment.NewLine);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(header);
        Console.ResetColor();
        Console.Write(Environment.NewLine);

        foreach (VideoGame vg in targetVGList)
        {
            Console.WriteLine(vg);
        }

        Console.Write(Environment.NewLine);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(footer);
        Console.ResetColor();

        Console.Write(Environment.NewLine);

        // ========== PERCENTAGE ========== //
        string percentage = (Math.Round(((decimal)targetVGList.Count / (decimal)vgList.Count) * 100, 2)).ToString("0.00");

        string singularPluralVerb = (targetVGList.Count == 1) ? "is" : "are";

        if (isPublisher)
        {
            Console.WriteLine($"Out of {vgList.Count} games, {targetVGList.Count} {singularPluralVerb} developed by {target}, which is {percentage}%.");
        }
        else
        {
            Console.WriteLine($"Out of {vgList.Count} games, {targetVGList.Count} are {target} games, which is {percentage}%.");
        }
    }






    // ======================== SELECT DISTINCT PUBLISHER/GENRE ======================== //
    private static Dictionary<string, int> DistinctSelect(bool isPublisher)
    {
        Dictionary<string, int> allDistinct = new Dictionary<string, int>();

        if (isPublisher)
        {
            // ========== LINQ RETRIEVE PULBISHERS ========== //
            allDistinct = vgList
                .GroupBy(vg => vg.Publisher)
                .ToDictionary(publishers => publishers.Key, publishers => publishers.Count());

            Console.WriteLine("Choose from one of the following PUBLISHERS:");
        }
        else
        {
            // ========== LINQ RETRIEVE GENRES ========== //
            allDistinct = vgList
                .GroupBy(vg => vg.Genre)
                .ToDictionary(genres => genres.Key, genres => genres.Count());

            Console.WriteLine("Choose from one of the following GENRES:");
        }

        // ========== INPUT PROMPT ========== //

        Console.Write(Environment.NewLine);

        int optionNumber = 1;

        foreach (var pair in allDistinct)
        {
            string singularPluralGames = (pair.Value == 1) ? "game" : "games";

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{optionNumber}] ");
            Console.ResetColor();
            Console.Write(pair.Key);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" ({pair.Value} {singularPluralGames})");
            Console.ResetColor();

            optionNumber++;
        }
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("[S] ");
        Console.ResetColor();
        Console.Write("SEARCH for a " + (isPublisher ? "publisher" : "genre") + ".");

        Console.WriteLine(Environment.NewLine);
        Console.Write($"Input your option (1-{allDistinct.Count}; S to Search) here: ");

        string selectedOption = Console.ReadLine();

        // ========== PUSH USER INPUT TO HISTORY ========== //
        userInputHistory.Push(isPublisher ? "[PublisherData] Selected Option: " : "[GenreData] Selected Option: ");
        userInputHistory.Push(selectedOption);

        // ========== INPUT VALIDATION ========== //
        bool invalidInput = true;
        string selectedKey = "";

        while (invalidInput)
        {
            if (selectedOption.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("Enter search query: ");
                Console.ResetColor();
                string searchQuery = Console.ReadLine();

                // ========== PUSH USER INPUT TO HISTORY ========== //
                userInputHistory.Push(isPublisher ? "[PublisherData] Search Query: " : "[GenreData] Search Query: ");
                userInputHistory.Push(searchQuery);


                selectedKey = allDistinct.Keys
                    .FirstOrDefault(key => key.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0);

                if (selectedKey == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR: No results found for '{searchQuery}'.");
                    Console.ResetColor();

                    Console.Write(Environment.NewLine);

                    Console.Write($"Please input an option (1-{allDistinct.Count} or S to Search again): ");

                    selectedOption = Console.ReadLine();

                    // ========== PUSH USER INPUT TO HISTORY ========== //
                    userInputHistory.Push(isPublisher ? "[PublisherData] Selected Option: " : "[GenreData] Selected Option: ");
                    userInputHistory.Push(searchQuery);
                }
                else
                {
                    invalidInput = false;
                }
            }
            else if (Int32.TryParse(selectedOption, out int selectedIndex)
                && selectedIndex > 0 && selectedIndex < allDistinct.Count + 1)
            {
                // ========== DISPLAY VIA INDEX ========== //

                selectedKey = allDistinct.Keys.ElementAt(selectedIndex - 1);

                invalidInput = false;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR: That was not a valid option. ");
                Console.ResetColor();
                Console.Write($"Please input an option (1-{allDistinct.Count}; S to Search): ");

                selectedOption = Console.ReadLine();

                // ========== PUSH USER INPUT TO HISTORY ========== //
                userInputHistory.Push(isPublisher ? "[PublisherData] Selected Option: " : "[GenreData] Selected Option: ");
                userInputHistory.Push(selectedOption);

                invalidInput = true;
            }
        }

        Console.Clear();

        if (isPublisher)
        {
            // ========== PUBLISHERDATA ========== //
            DisplayData(true, selectedKey); // isPublisher == true.
        }
        else
        {
            // ========== GENREDATA ========== //
            DisplayData(false, selectedKey); // isPublisher == false.
        }

        return allDistinct;
    }






    // ======================== DICTIONARY IMPLEMENTATION ======================== //
    private static void DictionaryImplementation(Dictionary<string, int> dictionary, string header, string footer)
    {
        // ========== DISPLAY ========== //
        string dictionaryImplementation = " DICTIONARY IMPLEMENTATION ";
        int dictImpBorderPadding = (header.Length - dictionaryImplementation.Length) / 2;
        string dictionaryHeader = new string('=', dictImpBorderPadding) + dictionaryImplementation + new string('=', dictImpBorderPadding);

        Console.Write(Environment.NewLine);
        Console.WriteLine(dictionaryHeader);
        Console.Write(Environment.NewLine);



        // ========== EXPLANATION ========== //
        Console.WriteLine("The first instance of DICTIONARY implementation in this project\n" +
            "occurs in the OPTION SELECT portion of the PublisherData and GenreData\n" +
            "displays. The Genre/Publisher and Number of Games constituted a key-value pair.\n");

        string pubSelectedKey = "Namco Bandai Games";



        // ========== RETRIEVE KEY-VALUE PAIR AND INDEX (DEQUEUE FUNCTIONS) ========== //
        RetrieveKeyValuePair retrieveKeyPair = (RetrieveKeyValuePair)functionQueue.Dequeue();
        KeyValuePair<string, int> keyPair = retrieveKeyPair(dictionary, pubSelectedKey);

        RetrieveIndex retrieveIndex = (RetrieveIndex)functionQueue.Dequeue();
        int pubOption = retrieveIndex(dictionary, pubSelectedKey);



        // ========== DISPLAY KEY-VALUE PAIR AND INDEX ========== //
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($"[{pubOption}] ");
        Console.ResetColor();
        Console.Write(keyPair.Key);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($" ({keyPair.Value} games)");
        Console.ResetColor();
        Console.Write(" --> ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(keyPair);
        Console.ResetColor();

        Console.Write(Environment.NewLine);
        Console.WriteLine(footer);
        Console.Write(Environment.NewLine);
    }






    // ======================== STACK IMPLEMENTATION ======================== //
    private static void StackImplementation(Stack<string> stack, string header, string footer)
    {
        // ========== DISPLAY ========== //
        string stackImplementation = " STACK IMPLEMENTATION ";
        int stackImpBorderPadding = (header.Length - stackImplementation.Length) / 2;
        string stackHeader = new string('=', stackImpBorderPadding) + stackImplementation + new string('=', stackImpBorderPadding);

        Console.WriteLine(stackHeader);

        Console.Write(Environment.NewLine);



        // ========== EXPLANATION ========== //
        Console.WriteLine("This program stores user input history in a STACK to be\n" +
            "stored later. Every instance of user input is recorded in pairs:\n" +
            "as a DESCRIPTION and, subsequently, the VALUE of the input itself.\n");



        // ========== RETRIEVE AND DISPLAY INPUT DESCRIPTION (DEQUEUE FUNCTIONS) ========== //
        RetrieveInputDescription retrieveInputDescription = (RetrieveInputDescription)functionQueue.Dequeue();
        string inputDescription = retrieveInputDescription(stack);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("DESCRIPTION: ");
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(inputDescription);
        Console.ResetColor();



        // ========== RETRIEVE AND DISPLAY INPUT VALUE (DEQUEUE FUNCTIONS) ========== //
        RetrieveInputValue retrieveInputValue = (RetrieveInputValue)functionQueue.Dequeue();
        string inputValue = retrieveInputValue(stack);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("VALUE: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(inputValue);
        Console.ResetColor();



        // ========== LIFO DEMONSTRATION ========== //
        Console.Write(Environment.NewLine);
        Console.WriteLine("Note that the order history goes from MOST RECENT to LEAST RECENT (LIFO).");
        Console.Write(Environment.NewLine);



        // ========== DISPLAY USERHISTORY MINUS EXAMPLE ========== //
        while (userInputHistory.Count > 2)
        {
            string userInput = userInputHistory.Pop();
            string description = userInputHistory.Pop();

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(description);
            Console.ResetColor();
            Console.WriteLine(userInput);
        }

        // ========== FINAL PAIR IN USERHISTORY ========== //
        string exampleInput = userInputHistory.Pop();
        string exampleDescription = userInputHistory.Pop();

        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write(exampleDescription);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(exampleInput);
        Console.ResetColor();

        Console.Write(Environment.NewLine);



        // ========== EXPLANATION ========== //
        Console.WriteLine("Now that we've iterated through the whole STACK, there are no\n" +
            "items leftover to be displayed (or popped). The history stack\n" +
            "has essentially been cleared and may be used again.\n");



        // ========== DISPLAY ========== //
        Console.Write(Environment.NewLine);
        Console.WriteLine(footer);
        Console.Write(Environment.NewLine);
    }






    // ======================== QUEUE IMPLEMENTATION ======================== //
    private static void QueueImplementation(Queue<Delegate> queue, string header, string footer)
    {
        // ========== DISPLAY ========== //
        string queueImplementation = " QUEUE IMPLEMENTATION ";
        int stackImpBorderPadding = (header.Length - queueImplementation.Length) / 2;
        string queueHeader = new string('=', stackImpBorderPadding) + queueImplementation + new string('=', stackImpBorderPadding);

        Console.WriteLine(queueHeader);

        Console.Write(Environment.NewLine);



        // ========== EXPLANATION ========== //
        Console.WriteLine("This program stores delegates (references to functions or methods)\n" +
            "in a QUEUE. These functions contain the various LINQ statements used\n" +
            "to retrieve data from each data structure (dictionaries, stacks, queues).\n");

        Console.WriteLine("Each of these functions have been pre-emptively stored in the queue.\n" +
            "Most functions can typically be called when they are DEQUEUED, for example:\n");



        // ========== RETRIEVE ========== //
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("RetrieveInputDescription ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("retrieveInputDescription ");
        Console.ResetColor();
        Console.Write("= (");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("RetrieveInputDescription");
        Console.ResetColor();
        Console.Write(")functionQueue.");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Dequeue");
        Console.ResetColor();
        Console.WriteLine("();");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("string ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("inputDescription ");
        Console.Write("= ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("retrieveInputDescription");
        Console.ResetColor();
        Console.Write("(");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("stack");
        Console.ResetColor();
        Console.WriteLine(");\n");

        Console.WriteLine("However, functions can also be retrieved using LINQ statements without affecting the queue.\n");

        // ========== RETRIEVE ========== //
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("var ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("exampleFunction ");
        Console.ResetColor();
        Console.Write("= ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("queue");
        Console.ResetColor();
        Console.Write(".");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("FirstOrDefault");
        Console.ResetColor();
        Console.WriteLine("();\n");

        var exampleFunction = queue
            .FirstOrDefault();

        RetrieveTopFive retrieveTopFive = (RetrieveTopFive)exampleFunction;
        IEnumerable<string> topFive = retrieveTopFive(vgList);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("Top 5 Best-Selling Games");
        Console.ResetColor();

        foreach (var vg in topFive)
        {
            Console.WriteLine(vg);
        }

        // ========== DISPLAY ========== //
        Console.Write(Environment.NewLine);
        Console.WriteLine(footer);
        Console.Write(Environment.NewLine);
    }
}