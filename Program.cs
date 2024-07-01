using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using MushroomPocket.NewFolder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SqlServer.Server;
using System.Net.Security;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MushroomPocket.Classes;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics.Metrics;
using MushroomPocket.Models;
using MushroomPocket.Model;
using System.Reflection.Metadata.Ecma335;
using System.Timers;
using System.Net;

namespace MushroomPocket
{
    class Program
    {

        private static Dictionary<string, string> mushroomSkillsDict = new Dictionary<string, string>()
{
                { "waluigi", "Agility" },
                { "daisy", "Leadership" },
                { "wario", "Strength" },
                { "luigi", "Precision and Accuracy" },
                { "peach", "Magic Abilities" },
                { "mario", "Combat Skills" }
            };

        private static List<MushroomMaster> mushroomMasters = new List<MushroomMaster>(){
            new MushroomMaster("Daisy", 2, "Peach"),
            new MushroomMaster("Wario", 3, "Mario"),
            new MushroomMaster("Waluigi", 1, "Luigi")
            };

        private static DbContextOptionsBuilder<CharacterContext> builder = new DbContextOptionsBuilder<CharacterContext>();

        private static Character choosenCharacter;
        static void Main(string[] args)
        {
            //MushroomMaster criteria list for checking character transformation availability.   
            /*************************************************************************
                PLEASE DO NOT CHANGE THE CODES FROM LINE 15-19
            *************************************************************************/
            //Use "Environment.Exit(0);" if you want to implement an exit of the console program
            //Start your assignment 1 requirements below;
            Console.WindowHeight = 30;
            Console.WindowWidth = 120;
            ConnectSQLServer();
            DisplayMenu();
        }

        private static void ConnectSQLServer()
        {
            string connection = @$"Server=.\SQLEXPRESS;Database=Character;Trusted_Connection=true;TrustServerCertificate=true;";
            string laptopConnection = @$"Server=ASUS_HW\SQLEXPRESS;Database=Character;Trusted_Connection=true;TrustServerCertificate=true;";
            try
            {
                Console.WriteLine("Connecting to SQL Express Database");
                Console.WriteLine("Connection String: " + connection);
                builder.UseSqlServer(connection);
                using (var context = new CharacterContext(builder.Options))
                {
                    // Try to open a connection to the database
                    context.Database.OpenConnection();
                    // If the connection is open, print a success message
                    Console.WriteLine("Connection successful!");
                    context.Database.EnsureCreated();
                    context.Database.Migrate();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection failed. Error: " + e.Message);
                Console.WriteLine(e.InnerException.Message);
            }
        }


        private static void DisplayMenu()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("***************************");
                Console.WriteLine("Welcome to Mushroom Pocket!");
                Console.WriteLine("***************************");
                Console.WriteLine("(1). Add Mushroom's character to my packet");
                Console.WriteLine("(2). List character(s) in my Pocket");
                Console.WriteLine("(3). Check if I can transform my characters");
                Console.WriteLine("(4). Transform character(s)");
                Console.WriteLine("(5). Delete character(s) by Name");
                Console.WriteLine("(6). Delete all characters");
                Console.WriteLine("(7). Start Mario Cart Game.");
                Console.WriteLine("(8). View Leaderboard");
                Console.Write("Please only enter [1-8] or Q to quit: ");
                string choice = Console.ReadLine().ToUpper();

                if (choice == "Q" || choice == "q")
                {
                    Console.WriteLine("Exiting Mushroom Pocket. Goodbye!");
                    Environment.Exit(0);
                }

                switch (choice)
                {
                    case "1":
                        option1();
                        break;

                    case "2":
                        // List down the characters that the user entered and sort the list by HP in descending order
                        option2();
                        break;
                    case "3":
                        // List character(s) that can be transformed
                        option3();
                        break;
                    case "4":
                        // Transform character(s). The newly transformed character will have its HP=100, EXP=0, and Skill=[based on the newly transformed abilities]
                        option4();
                        break;
                    case "5":
                        // Delete character(s) from the list
                        option5();
                        break;
                    case "6":
                        // Delete all characters from the list
                        option6();
                        break;
                    case "7":
                        using (var context = new CharacterContext(builder.Options))
                        {
                            if (context.Character.Count() == 0)
                            {
                                Console.WriteLine("No characters in the database.");
                                Console.WriteLine("Please add a character first.");
                                break;
                            }
                        }
                        Console.WriteLine("(1) Single Player");
                        Console.WriteLine("(2) Multiplayer");
                        Console.Write("Please select a game mode(1/2): ");
                        var selection = Console.ReadLine();


                        if (selection == "1")
                        {
                            option7(selection);
                            if (choosenCharacter.HP < 0)
                            {
                                Console.WriteLine("Character does not have sufficient HP to play. Please select another character.");
                                break;
                            }
                            marioCartGame();
                        }
                        else if (selection == "2")
                        {
                            option7(selection);
                            if (choosenCharacter.HP < 0)
                            {
                                Console.WriteLine("Character does not have sufficient HP to play. Please select another character.");
                                break;
                            }
                            marioCartGameMultiplayer();
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Please try again.");
                        }
                        break;
                    case "8":
                        // View the leaderboard
                        viewLeaderBoard();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void option7(string selection)
        {
            using (var context = new CharacterContext(builder.Options))
            {
                foreach (var character in context.Character)
                {
                    Console.WriteLine($"Id: {character.Id}, Name: {character.Name}, HP: {character.HP}, EXP: {character.EXP}, Level: {character.Level}, Skills: {character.Skill}");
                }
                int choosenID;
                while (true)
                {
                    // Prompt the user to enter a number
                    Console.Write("Please select a character by ID: ");
                    // Read the input from the user
                    // Try to convert the input to an integer
                    if (int.TryParse(Console.ReadLine(), out choosenID))
                    {
                        if (context.Character.Where(x => x.Id == choosenID).Count() == 0)
                        {
                            Console.WriteLine("Character does not exist.");
                            Console.WriteLine("Please try again");
                        }
                        else
                        {
                            break;

                        }
                    }
                    else
                    {
                        // The conversion failed, handle the error accordingly
                        Console.WriteLine("Invalid input. Please enter a valid Id.");
                    }
                }

                choosenCharacter = context.Character.Where(x => x.Id == choosenID).FirstOrDefault();

            }
            if (selection == "1")
            {
                Console.WriteLine("You have chosen Single Player Mode\n");
                Console.WriteLine($"You have chosen {choosenCharacter.Name} as your character");
                Console.WriteLine("-----Stats-----");
                if (choosenCharacter.Name == "Daisy")
                {
                    Console.WriteLine("------------");
                    Console.WriteLine("Speed: 15");
                    Console.WriteLine("Size: 2");
                    Console.WriteLine("------------");
                }
                else if (choosenCharacter.Name == "Wario")
                {
                    Console.WriteLine("------------");
                    Console.WriteLine("Speed: 24");
                    Console.WriteLine("Size: 2");
                    Console.WriteLine("------------");
                }
                else if (choosenCharacter.Name == "Waluigi")
                {
                    Console.WriteLine("------------");
                    Console.WriteLine("Speed: 22");
                    Console.WriteLine("Size: 2");
                    Console.WriteLine("------------");
                }
                else if (choosenCharacter.Name == "Peach")
                {
                    Console.WriteLine("------------");
                    Console.WriteLine("Speed: 15");
                    Console.WriteLine("Size: 1");
                    Console.WriteLine("------------");
                }
                else if (choosenCharacter.Name == "Mario")
                {
                    Console.WriteLine("------------");
                    Console.WriteLine("Speed: 20");
                    Console.WriteLine("Size: 1");
                    Console.WriteLine("------------");
                }
                else if (choosenCharacter.Name == "Luigi")
                {
                    Console.WriteLine("------------");
                    Console.WriteLine("Speed: 20");
                    Console.WriteLine("Size: 1");
                    Console.WriteLine("------------");

                }
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }
            else if (selection == "2")
            {
                Console.WriteLine("You have chosen Multiplayer Mode");
                Console.WriteLine("All character stats will be set to default at 15 speed and 2 size");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }
        }

        private static void option6()
        {
            Console.Write("Are you sure you want to delete ALL the characters in the database? All characters will be deleted!(Y/N): ");
            string confirm = Console.ReadLine().ToUpper();
            if (confirm == "Y")
            {
                using (var context = new CharacterContext(builder.Options))
                {
                    context.Character.ExecuteDelete();
                    Console.WriteLine("All characters have been deleted.");
                }
            }
            else
            {
                Console.WriteLine("Deletion of all Characters Cancelled");
            }
        }

        private static void option5()
        {

            using (var context = new CharacterContext(builder.Options))
            {
                Console.Write("Input the character you would like to delete: ");
                string name = Console.ReadLine();
                Console.Write("Are you sure you want to delete this character? All characters with this name will be deleted!(Y/N)");
                string confirm = Console.ReadLine().ToUpper();
                if (confirm == "Y")
                {
                    if (context.Character.Where(x => x.Name.ToLower() == name.ToLower()).Count() == 0)
                    {
                        Console.WriteLine("Character does not exist.");
                        return;
                    }
                    else
                    {
                        context.Character.RemoveRange(context.Character.Where(x => x.Name.ToLower() == name.ToLower()));
                        context.SaveChanges();
                        Console.WriteLine($"All {capitalizeString(name)}(s) have been deleted.");
                    }

                }
                else
                {
                    Console.WriteLine("Character deletion cancelled.");
                }
            }
        }

        private static void option4()
        {
            using (var context = new CharacterContext(builder.Options))
            {
                foreach (var character in context.Character.ToList())
                {

                    var transformedInto = mushroomMasters.FirstOrDefault(x => x.Name.ToLower() == character.Name.ToLower())?.TransformTo;
                    var numberOfTransformations = mushroomMasters.FirstOrDefault(x => x.Name.ToLower() == character.Name.ToLower())?.NoToTransform;
                    var countCharacter = context.Character.Where(x => x.Name.ToLower().Equals(character.Name.ToLower())).Count();

                    if (countCharacter >= numberOfTransformations)
                    {
                        try
                        {
                            if (transformedInto == "Luigi")
                            {
                                Luigi luigi = new Luigi("Luigi", 100, 0, 1, "Precision and Accuracy");
                                context.Character.Add(luigi);
                                context.Character.RemoveRange(context.Character.Where(x => x.Name == character.Name).Take((int)numberOfTransformations));
                                context.SaveChanges();
                                Console.WriteLine($"{capitalizeString(character.Name)} has been transformed to {transformedInto}");
                            }
                            else if (transformedInto == "Mario")
                            {
                                Mario mario = new Mario("Mario", 100, 0, 1, "Combat Skills");
                                context.Character.Add(mario);
                                context.Character.RemoveRange(context.Character.Where(x => x.Name == character.Name).Take((int)numberOfTransformations));
                                context.SaveChanges();
                                Console.WriteLine($"{capitalizeString(character.Name)} has been transformed to {transformedInto}");
                            }
                            else if (transformedInto == "Peach")
                            {
                                Peach peach = new Peach("Peach", 100, 0, 1, "Magic Abilities");
                                context.Character.Add(peach);
                                context.Character.RemoveRange(context.Character.Where(x => x.Name == character.Name).Take((int)numberOfTransformations));
                                context.SaveChanges();
                                Console.WriteLine($"{capitalizeString(character.Name)} has been transformed to {transformedInto}");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                        }
                    }
                }
            }

        }

        private static void option3()
        {
            List<string> checkDisplayTransformed = [];

            using (var context = new CharacterContext(builder.Options))
                foreach (var character in context.Character.ToList())
                {
                    try
                    {
                        var noToTransform = mushroomMasters.FirstOrDefault(x => x.Name.ToLower() == character.Name.ToLower())?.NoToTransform;
                        var countCharacter = context.Character.Where(x => x.Name.ToLower().Equals(character.Name.ToLower())).Count();

                        if (countCharacter >= noToTransform)
                        {
                            if (!checkDisplayTransformed.Contains(character.Name.ToLower()))
                            {
                                var transformedInto = mushroomMasters.FirstOrDefault(x => x.Name.ToLower() == character.Name.ToLower()).TransformTo;
                                Console.WriteLine($"{capitalizeString(character.Name)} --> {transformedInto}");
                                checkDisplayTransformed.Add(character.Name.ToLower());
                            }

                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
        }

        private static void option2()
        {
            // Implementation of option 2: View characters and sort by HP
            // Sort characters by HP in descending order
            try
            {
                using (var context = new CharacterContext(builder.Options))
                {
                    context.Database.EnsureCreated();
                    var sortedCharacters = context.Character.OrderByDescending(m => m.HP).ToList();
                    foreach (var character in sortedCharacters)
                    {
                        Console.WriteLine("------------------------");
                        Console.WriteLine($"Name: {capitalizeString(character.Name)}\nHP: {character.HP}\nEXP: {character.EXP}\nLevel: {character.Level}\nSkill: {character.Skill}");
                        Console.WriteLine("------------------------");

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private static void option1()
        {
            int hp;
            int exp;


            Console.Write("Enter Character's Name: ");
            string name = Console.ReadLine();
            if (mushroomSkillsDict.ContainsKey(name.ToLower()) && (name.ToLower() == "daisy" || name.ToLower() == "wario" || name.ToLower() == "waluigi"))
            {
                try
                {
                    Console.Write("Enter Character's HP: ");
                    hp = int.Parse(Console.ReadLine());
                    {

                    }
                    while (hp <= 0 || hp > 100)
                    {
                        Console.WriteLine("Invalid HP. Please try again.");
                        Console.Write("Enter Character's HP: ");
                        hp = int.Parse(Console.ReadLine());
                    }

                    Console.Write("Enter Character's EXP: ");
                    exp = int.Parse(Console.ReadLine());
                    while (exp < 0 || exp > 199)
                    {
                        Console.WriteLine("Invalid EXP. Please try again.");
                        Console.Write("Enter Character's EXP: ");
                        exp = int.Parse(Console.ReadLine());
                    }


                    string skill = mushroomSkillsDict[name.ToLower()];
                    int level = 1;

                    using (var context = new CharacterContext(builder.Options))

                        if (name.ToLower() == "waluigi")
                        {
                            Waluigi waluigi = new Waluigi(capitalizeString(name), hp, exp, level, skill);
                            context.Character.Add(waluigi);
                            context.SaveChanges();

                            Console.WriteLine($"{capitalizeString(name)} has been added.");

                        }
                        else if (name.ToLower() == "wario")
                        {
                            Wario wario = new Wario(capitalizeString(name), hp, exp, level, skill);
                            context.Character.Add(wario);
                            context.SaveChanges();
                            Console.WriteLine($"{capitalizeString(name)} has been added.");

                        }
                        else if (name.ToLower() == "daisy")
                        {
                            Daisy daisy = new Daisy(capitalizeString(name), hp, exp, level, skill);
                            context.Character.Add(daisy);
                            context.SaveChanges();
                            Console.WriteLine($"{capitalizeString(name)} has been added.");

                        }
                        else
                        {
                            Console.WriteLine("An error has occured when adding character to database.");
                        }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }


            }
            else
            {
                Console.WriteLine("Invalid Character Name. Please try again.");
            }
        }

        public static string capitalizeString(string input)
        {
            return (char.ToUpper(input[0]) + input.Substring(1)).ToString();
        }

        public static void viewLeaderBoard()
        {
            using (var context = new CharacterContext(builder.Options))
            {
                Console.WriteLine("\n------------Leaderboard------------");
                var placement = 1;
                foreach (var ranking in context.Leaderboard.OrderByDescending(x => x.Score).Take(5).ToList())
                {
                    Console.WriteLine($"Rank: {placement}, Name: {ranking.Name}, Character: {ranking.Character}, High Score: {ranking.Score}");
                    placement++;
                }
            }
        }

        public static void marioCartGame()
        {
            //https://github.com/dotnet/dotnet-console-games/blob/main/Projects/Drive/Program.cs
            //credits to ZacharyPatten

            int windowWidth;
            int windowHeight;
            char[,] scene;
            int score = 0;
            int carPosition;
            int carVelocity;
            bool gameRunning;
            bool keepPlaying = true;
            bool consoleSizeError = false;
            int previousRoadUpdate = 0;
            var speed = 50;
            int setRoadWidth = 15;
           


            if (choosenCharacter.Name == "Daisy")
            {
                setRoadWidth = 15;
                speed = 50;
            }
            else if (choosenCharacter.Name == "Wario")
            {
                setRoadWidth = 15;
                speed = 36;


            }
            else if (choosenCharacter.Name == "Waluigi")
            {
                setRoadWidth = 15;
                speed = 38;


            }
            else if (choosenCharacter.Name == "Peach")
            {
                setRoadWidth = 20;

            }
            else if (choosenCharacter.Name == "Mario")
            {
                setRoadWidth = 20;
                speed = 40;

            }
            else if (choosenCharacter.Name == "Luigi")
            {
                setRoadWidth = 20;
                speed = 40;

            }

            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            Console.SetWindowSize(width, height);
            Console.CursorVisible = false;

            try
            {
                Initialize();
                LaunchScreen();

                while (keepPlaying)
                {
                    InitializeScene();
                    var timer = new System.Timers.Timer(20000);
                    timer.Elapsed += onTimedEvent;
                    timer.AutoReset = true;
                    timer.Enabled = true;
                    while (gameRunning)
                    {
                        if (Console.WindowHeight < height || Console.WindowWidth < width)
                        {
                            consoleSizeError = true;
                            keepPlaying = false;
                            break;
                        }
                        

                        HandleInput();
                        Update();
                        Render();
                       
                        if (speed < 20)
                        {
                            timer.Stop();
                        }

                        if (gameRunning)
                        {
                            Thread.Sleep(TimeSpan.FromMilliseconds(speed));
                        }
                    }
                    if (keepPlaying)
                    {
                        GameOverScreen();
                    }
                }
                Console.Clear();
                if (consoleSizeError)
                {
                    Console.WriteLine("Console/Terminal window is too small.");
                    Console.WriteLine($"Minimum size is {width} width x {height} height.");
                    Console.WriteLine("Increase the size of the console window.");
                }
                Console.WriteLine("The Mario Cart Game was closed.");
            }
            finally
            {
                Console.CursorVisible = true;
            }

            void Initialize()
            {
                windowWidth = Console.WindowWidth;
                windowHeight = Console.WindowHeight;
                if (true)
                {
                    if (windowWidth < width)
                    {
                        windowWidth = Console.WindowWidth = width + 1;
                    }
                    if (windowHeight < height)
                    {
                        windowHeight = Console.WindowHeight = height + 1;
                    }
                    Console.WindowWidth = windowWidth;
                    Console.WindowHeight = windowHeight;
                    //Console.BufferHeight = windowHeight;
                    //Console.BufferWidth = windowWidth;


                }
            }

            void LaunchScreen()
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Mario Cart Game");
                Console.WriteLine();
                Console.WriteLine("");
                Console.WriteLine();
                Console.WriteLine("Use the arrow keys to control your velocity.");
                Console.WriteLine();
                Console.Write("Press [enter] to start...");
                PressEnterToContinue();
            }
            //start here
            void InitializeScene()
            {
                int roadWidth = setRoadWidth;
                gameRunning = true;
                carPosition = width / 2;
                carVelocity = 0;
                int leftEdge = (width - roadWidth) / 2;
                int rightEdge = leftEdge + roadWidth + 1;
                scene = new char[height, width];
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (j < leftEdge || j > rightEdge)
                        {
                            scene[i, j] = '.';
                        }
                        else
                        {
                            scene[i, j] = ' ';
                        }
                    }
                }
            }

            void HandleInput()
            {
                while (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case (ConsoleKey.LeftArrow):
                            carVelocity = -1;
                            break;
                        case ConsoleKey.RightArrow:
                            carVelocity = +1;
                            break;
                        case ConsoleKey.DownArrow:
                            carVelocity = 0;
                            break;
                        case ConsoleKey.Escape:
                            gameRunning = false;
                            keepPlaying = false;
                            break;
                        case ConsoleKey.Enter:
                            Console.ReadLine();
                            break;
                    }
                }
            }

            void Render()
            {

                StringBuilder stringBuilder = new StringBuilder(width * height);
                for (int i = height - 1; i >= 0; i--)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (i is 1 && j == carPosition)
                        {

                            stringBuilder.Append(
                                !gameRunning ? 'X' :
                                carVelocity < 0 ? '^' :
                                carVelocity > 0 ? '^' :
                                '^');

                        }
                        else
                        {
                            stringBuilder.Append(scene[i, j]);
                        }
                    }
                    if (i > 0)
                    {
                        stringBuilder.AppendLine();
                    }
                }
                Console.SetCursorPosition(0, 0);
                Console.Write(stringBuilder);
                //Console.ResetColor(); // Reset color to default after printing the character
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Score: {score} ");
            }

            void Update()
            {
                for (int i = 0; i < height - 1; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        scene[i, j] = scene[i + 1, j];
                    }
                }



                var rmd = new Random();
                int roadUpdate =
                    rmd.Next(5) < 4 ? previousRoadUpdate :
                    rmd.Next(3) - 1;
                if (roadUpdate is -1 && scene[height - 1, 0] is ' ') roadUpdate = 1;
                if (roadUpdate is 1 && scene[height - 1, width - 1] is ' ') roadUpdate = -1;
                switch (roadUpdate)
                {
                    case -1: // left
                        for (int i = 0; i < width - 1; i++)
                        {
                            scene[height - 1, i] = scene[height - 1, i + 1];
                        }
                        scene[height - 1, width - 1] = '.';
                        break;
                    case 1: // right
                        for (int i = width - 1; i > 0; i--)
                        {
                            scene[height - 1, i] = scene[height - 1, i - 1];
                        }
                        scene[height - 1, 0] = '.';
                        break;
                }
                previousRoadUpdate = roadUpdate;
                carPosition += carVelocity;
                if (carPosition < 0 || carPosition >= width || scene[1, carPosition] != ' ')
                {
                    gameRunning = false;
                }
                score++;

            }
            //end here
            void onTimedEvent(Object source, ElapsedEventArgs e)
            {
                speed -= 2;
     
            }

            void CheckLeaderboard()
            {
                using (var context = new CharacterContext(builder.Options))
                {
                    context.Database.EnsureCreated();
                    var leaderboard = context.Leaderboard.OrderByDescending(x => x.Score).Take(5).ToList();
                    try
                    {
                        if (context.Leaderboard.Count() == 0)
                        {
                            Console.WriteLine("You made it to the leaderboard!");
                            Console.Write("Enter your name: ");
                            string name = Console.ReadLine();
                            context.Leaderboard.Add(new Leaderboard(name, choosenCharacter.Name, score));
                            context.SaveChanges();
                            Console.WriteLine("You have been added to the leaderboard! Check the Leaderboard rankings by return to the main menu. ");
                        }
                        else if (leaderboard.Last().Score < score)
                        {
                            Console.WriteLine("You made it to the leaderboard!");
                            Console.Write("Enter your name: ");
                            string name = Console.ReadLine();
                            context.Leaderboard.Add(new Leaderboard(name, choosenCharacter.Name, score));
                            context.SaveChanges();
                            Console.WriteLine("You have been added to the leaderboard! Check the Leaderboard rankings by return to the main menu. ");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
            }

            void GameOverScreen()
            {
                Console.Clear();
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("Game Over");
                Console.WriteLine($"Score: {score}");
                Console.ForegroundColor = ConsoleColor.Green;
                choosenCharacter.HP -= 1;

                choosenCharacter.EXP += score / 10;
                Console.WriteLine($"EXP earned: {score / 10}");
                Console.WriteLine($"Current EXP: {choosenCharacter.EXP}");
                while (choosenCharacter.EXP >= 200)
                {
                    choosenCharacter.Level += 1;
                    choosenCharacter.EXP -= 200;
                    choosenCharacter.HP = 100;
                    Console.WriteLine("You have leveled up!");
                }

                Console.WriteLine($"Current Level: {choosenCharacter.Level}");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"HP remaining: {choosenCharacter.HP}");

                using (var context = new CharacterContext(builder.Options))
                {
                    context.Character.Update(choosenCharacter);
                    context.SaveChanges();
                }
                Console.ResetColor();
                CheckLeaderboard();
                if (choosenCharacter.HP <= 0)
                {
                    Console.WriteLine("You have no health points remaining.");
                    Console.WriteLine("Press ESC to Return to menu");
                    keepPlaying = false;
                }
                else
                {
                    Console.WriteLine($"Play Again (Y/N)?");
                }
            GetInput:
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Y:
                        keepPlaying = true;
                        Console.CursorVisible = false;
                        score = 0;
                        break;
                    case ConsoleKey.Escape:
                        keepPlaying = false;
                        break;
                    case ConsoleKey.N:
                        keepPlaying = false;
                        break;
                    default:
                        goto GetInput;
                }
            }

            void PressEnterToContinue()
            {
            GetInput:
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:
                        break;
                    case ConsoleKey.Escape:
                        keepPlaying = false;
                        break;
                    case ConsoleKey.N:
                        keepPlaying = false;
                        break;
                    default: goto GetInput;
                }
            }
        }
        public static void marioCartGameMultiplayer()
        {
            //https://github.com/dotnet/dotnet-console-games/blob/main/Projects/Drive/Program.cs
            //credits to ZacharyPatten

            int windowWidth;
            int windowHeight;
            char[,] scene;
            int score = 0;
            int carPosition;
            int carVelocity;
            bool gameRunning;
            bool keepPlaying = true;
            bool consoleSizeError = false;
            int previousRoadUpdate = 0;
            var speed = 50;
            int setRoadWidth = 15;
            var isNotConnected = false;
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            Console.SetWindowSize(width, height);
            string serverAddress;
            while (true)
            {
                Console.Write("Please enter an IP address:");
                serverAddress = Console.ReadLine();

                if (IPAddress.TryParse(serverAddress, out _))
                {
                    Console.WriteLine("You have entered a valid IP address: " + serverAddress);
                    break;
                }
                else
                {
                    Console.WriteLine("The IP address you entered is not valid.");
                }
            }

            Console.CursorVisible = false;


            //string serverAddress = "192.168.1.37";
            //string serverAddress = "172.26.186.26";

            int port = 11000;
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(serverAddress, port);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            if (!client.Connected)
            {
                Console.WriteLine("Failed to connect to server.");
                return;
            }
            else
            {
                Console.WriteLine("Connected to server!");
            }


            NetworkStream stream = client.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            try
            {
                Initialize();
                LaunchScreen();
                while (keepPlaying)
                {
                    InitializeScene();
                    var timer = new System.Timers.Timer(20000);
                    timer.Elapsed += onTimedEvent;
                    timer.AutoReset = true;
                    timer.Enabled = true;
                    while (gameRunning)
                    {
                        if (Console.WindowHeight < height || Console.WindowWidth < width)
                        {
                            consoleSizeError = true;
                            keepPlaying = false;
                            break;
                        }
                        HandleInput();
                        Update();
                        Render();

                        if (speed < 20)
                        {
                            timer.Stop();
                        }


                        byte[] data = Encoding.UTF8.GetBytes(score.ToString());
                        try
                        {
                            stream.Write(data, 0, data.Length);

                        }
                        catch (Exception)
                        {
                            client.Close();
                            Console.Clear();
                            Console.WriteLine("You Win");
                            Console.WriteLine($"Score: {score}");
                            Console.ForegroundColor = ConsoleColor.Green;
                            choosenCharacter.HP -= 1;

                            choosenCharacter.EXP += score / 10;
                            Console.WriteLine($"EXP earned: {score / 10}");
                            Console.WriteLine($"Current EXP: {choosenCharacter.EXP}");
                            while (choosenCharacter.EXP >= 200)
                            {
                                choosenCharacter.Level += 1;
                                choosenCharacter.EXP -= 200;
                                choosenCharacter.HP = 100;
                                Console.WriteLine("You have leveled up!");
                            }

                            Console.WriteLine($"Current Level: {choosenCharacter.Level}");

                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"HP remaining: {choosenCharacter.HP}");

                            using (var context = new CharacterContext(builder.Options))
                            {
                                context.Character.Update(choosenCharacter);
                                context.SaveChanges();
                            }
                            Console.ResetColor();
                            CheckLeaderboard();

                            Console.WriteLine("Press any key to go back to the main menu");
                            Console.ReadLine();


                            isNotConnected = true;
                            gameRunning = false;
                            return;
                        }

                        if (gameRunning)
                        {
                            timer.Elapsed += (sender, e) =>
                            {
                                speed-= 2;
                                if (speed < 10)
                                {
                                    timer.Stop();
                                    timer.AutoReset = true;
                                }
                            };
                            Thread.Sleep(TimeSpan.FromMilliseconds(speed));
                        }
                    }
                    byte[] buffer = new byte[1024];
                    if (isNotConnected)
                    {
                        Console.Clear();
                        keepPlaying = false;
                    }
                    if (keepPlaying)
                    {
                        GameOverScreen();
                        return;
                    }
                }
                Console.Clear();
                if (consoleSizeError)
                {
                    Console.WriteLine("Console/Terminal window is too small.");
                    Console.WriteLine($"Minimum size is {width} width x {height} height.");
                    Console.WriteLine("Increase the size of the console window.");
                }
                Console.WriteLine("Mario Cart Game was closed.");
            }
            finally
            {
                Console.CursorVisible = true;
            }

            void Initialize()
            {
                windowWidth = Console.WindowWidth;
                windowHeight = Console.WindowHeight;
                if (true)
                {
                    if (windowWidth < width)
                    {
                        windowWidth = Console.WindowWidth = width + 1;
                    }
                    if (windowHeight < height)
                    {
                        windowHeight = Console.WindowHeight = height + 1;
                    }
                    Console.WindowWidth = windowWidth;
                    Console.WindowHeight = windowHeight;
                }
            }

            void LaunchScreen()
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Mario Cart Game");
                Console.WriteLine();
                Console.WriteLine("");
                Console.WriteLine();
                Console.WriteLine("Use the arrow keys to control your velocity.");
                Console.WriteLine();
                Console.Write("Press [enter] to start the multiplayer Mario Cart Survival Game");
                PressEnterToContinue();
            }
            //start here
            void InitializeScene()
            {
                int roadWidth = setRoadWidth;
                gameRunning = true;
                carPosition = width / 2;
                carVelocity = 0;
                int leftEdge = (width - roadWidth) / 2;
                int rightEdge = leftEdge + roadWidth + 1;
                scene = new char[height, width];
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (j < leftEdge || j > rightEdge)
                        {
                            scene[i, j] = '.';
                        }
                        else
                        {
                            scene[i, j] = ' ';
                        }
                    }
                }


            }

            void HandleInput()
            {
                while (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case (ConsoleKey.LeftArrow):
                            carVelocity = -1;
                            break;
                        case ConsoleKey.RightArrow:
                            carVelocity = +1;
                            break;
                        case ConsoleKey.DownArrow:
                            carVelocity = 0;
                            break;
                        case ConsoleKey.Escape:
                            gameRunning = false;
                            keepPlaying = false;
                            break;
                        case ConsoleKey.Enter:
                            Console.ReadLine();
                            break;
                    }
                }
            }

            void Render()
            {

                StringBuilder stringBuilder = new StringBuilder(width * height);
                for (int i = height - 1; i >= 0; i--)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (i is 1 && j == carPosition)
                        {

                            stringBuilder.Append(
                                !gameRunning ? 'X' :
                                carVelocity < 0 ? '^' :
                                carVelocity > 0 ? '^' :
                                '^');

                        }
                        else
                        {
                            stringBuilder.Append(scene[i, j]);
                        }
                    }
                    if (i > 0)
                    {
                        stringBuilder.AppendLine();
                    }
                }
                Console.SetCursorPosition(0, 0);
                Console.Write(stringBuilder);



                string serializedMessage = stringBuilder.ToString();
                //Console.ResetColor(); // Reset color to default after printing the character
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Score: {score} ");
            }
            void Update()
            {
                for (int i = 0; i < height - 1; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        scene[i, j] = scene[i + 1, j];
                    }
                }



                var rmd = new Random();
                int roadUpdate =
                    rmd.Next(5) < 4 ? previousRoadUpdate :
                    rmd.Next(3) - 1;
                if (roadUpdate is -1 && scene[height - 1, 0] is ' ') roadUpdate = 1;
                if (roadUpdate is 1 && scene[height - 1, width - 1] is ' ') roadUpdate = -1;
                switch (roadUpdate)
                {
                    case -1: // left
                        for (int i = 0; i < width - 1; i++)
                        {
                            scene[height - 1, i] = scene[height - 1, i + 1];
                        }
                        scene[height - 1, width - 1] = '.';
                        break;
                    case 1: // right
                        for (int i = width - 1; i > 0; i--)
                        {
                            scene[height - 1, i] = scene[height - 1, i - 1];
                        }
                        scene[height - 1, 0] = '.';
                        break;
                }
                previousRoadUpdate = roadUpdate;
                carPosition += carVelocity;
                if (carPosition < 0 || carPosition >= width || scene[1, carPosition] != ' ')
                {
                    gameRunning = false;
                }
                score++;

            }
            //end here
            void onTimedEvent(Object source, ElapsedEventArgs e)
            {
                speed -= 2;

            }
            void CheckLeaderboard()
            {
                using (var context = new CharacterContext(builder.Options))
                {
                    context.Database.EnsureCreated();
                    var leaderboard = context.Leaderboard.OrderByDescending(x => x.Score).Take(5).ToList();
                    try
                    {
                        if (context.Leaderboard.Count() == 0)
                        {
                            Console.WriteLine("You made it to the leaderboard!");
                            Console.Write("Enter your name: ");
                            string name = Console.ReadLine();
                            context.Leaderboard.Add(new Leaderboard(name, choosenCharacter.Name, score));
                            context.SaveChanges();
                            Console.WriteLine("You have been added to the leaderboard! Check the Leaderboard rankings by return to the main menu. ");
                        }
                        else if (leaderboard.Last().Score < score)
                        {
                            Console.WriteLine("You made it to the leaderboard!");
                            Console.Write("Enter your name: ");
                            string name = Console.ReadLine();
                            context.Leaderboard.Add(new Leaderboard(name, choosenCharacter.Name, score));
                            context.SaveChanges();
                            Console.WriteLine("You have been added to the leaderboard! Check the Leaderboard rankings by return to the main menu. ");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }
            }

            void GameOverScreen()
            {

                client.Close();
                Console.Clear();
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("You lose");
                Console.WriteLine($"Score: {score}");
                Console.ForegroundColor = ConsoleColor.Green;
                choosenCharacter.HP -= 1;

                //choosenCharacter.EXP += score / 10;
                //Console.WriteLine($"EXP earned: {score / 10}");
                //Console.WriteLine($"Current EXP: {choosenCharacter.EXP}");
                //while (choosenCharacter.EXP >= 200)
                //{
                //    choosenCharacter.Level += 1;
                //    choosenCharacter.EXP -= 200;
                //    choosenCharacter.HP = 100;
                //    Console.WriteLine("You have leveled up!");
                //}

                //Console.WriteLine($"Current Level: {choosenCharacter.Level}");

                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.WriteLine($"HP remaining: {choosenCharacter.HP}");

                //using (var context = new CharacterContext(builder.Options))
                //{
                //    context.Character.Update(choosenCharacter);
                //    context.SaveChanges();
                //}
                Console.ResetColor();
                //CheckLeaderboard();

                if (choosenCharacter.HP <= 0)
                {
                    Console.WriteLine("You have no health points remaining.");
                    Console.WriteLine("Press any key to return to menu");
                    keepPlaying = false;
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine($"Press any key to return to menu");
                    Console.ReadLine();
                    return;
                }

            }

            void PressEnterToContinue()
            {
            GetInput:
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Enter:
                        break;
                    case ConsoleKey.Escape:
                        keepPlaying = false;
                        break;
                    case ConsoleKey.N:
                        keepPlaying = false;
                        break;
                    default: goto GetInput;
                }
            }
        }
    }
}


