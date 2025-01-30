using System.Reflection.Metadata;
using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using System.Net.Mail;
using LetEmTrain.Domain;
using System.Reflection;
using LetEmTrain.Infrastructure.Repository;
using LetEmTrain.ConsoleApp;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Query;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using LetEmTrain.Infrastructure.Migrations;

namespace LetEmTrain.ConsoleApp
{
    internal class Program
    {
        public static User loggedUser = null;
        public static Admin loggedAdmin = null;
        public static Progress lastProgress = null;
        public static bool exit = false;

        static void Main(string[] args)
        {
            // If there is no database create one with added Admin and Exercises
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
            }

            while (!exit)
            {
                if (loggedUser == null && loggedAdmin == null)
                {
                    LoginMenu();
                }
                else if (loggedUser == null)
                {
                    AdminMenu();
                }
                else
                {
                    UserMenu();
                }
            }
        }

       
        async static Task AdminMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome Admin " + loggedAdmin.Login + " It's nice to see you again");
            Console.WriteLine("----------------------------------");
            Console.WriteLine(" - 1.Create New User");
            Console.WriteLine(" - 2.Print existing Users");
            Console.WriteLine(" - 3.Search User");
            Console.WriteLine(" - 4.Activate/Deactivate User");
            Console.WriteLine(" - 5.Create Exercise");
            Console.WriteLine(" - 6.Print Exercises");
            Console.WriteLine(" - 7.Edit Exercises");
            Console.WriteLine(" - 8.Delete Exercises");
            Console.WriteLine(" - 9.Log Out");
            Console.WriteLine(" - 0.Exit Application");

            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    await CreateUser();
                    Console.WriteLine(" Press any key to continue");
                    Console.ReadKey();
                    break;
                case "2":
                    await PrintUsers();
                    Console.WriteLine(" Press any key to continue");
                    Console.ReadKey();
                    break;
                case "3":
                    await SearchUser();
                    break;
                case "4":
                    await ToggleUserActivation();
                    break;
                case "5":
                    await CreateExercise();
                    Console.ReadKey();
                    break;
                case "6":
                    await PrintExercises();
                    break;
                case "7":
                    await EditExercise();
                    break;
                case "8":
                    await DeleteExercise();
                    break;
                case "9":
                    loggedAdmin = null;
                    Console.WriteLine("You have been logged out, press any key to continue");
                    Console.ReadKey();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Selected option is not correct, please try again :)");
                    Console.ReadKey();
                    break;
            }
        }

        async static Task UserMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome " + loggedUser.Username + "!!!");
            Console.WriteLine("----------------------------");
            Console.WriteLine(" - 1.Create Progress");
            Console.WriteLine(" - 2.Print Progress");
            Console.WriteLine(" - 3.Print Exercises");
            Console.WriteLine(" - 4.Calculate BodyScore");
            Console.WriteLine(" - 5.Create Dietary Recommendation");
            Console.WriteLine(" - 6.View Profile");
            Console.WriteLine(" - 7.Create Workout Plan");
            Console.WriteLine(" - 8.Show Workout Plans");
            Console.WriteLine(" - 9.Update existing Workout Plan");
            Console.WriteLine(" - 10.Delete Workout Plan");
            Console.WriteLine(" - 11.Log Out");
            Console.WriteLine(" - 0.Exit");

            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    await AddProgress();
                    Console.WriteLine(" Press any key to continue");
                    Console.ReadKey();
                    break;
                case "2":
                    await ShowProgress();
                    Console.WriteLine(" Press any key to continue");
                    Console.ReadKey();
                    break;
                case "3":
                    await PrintExercises();
                    break;
                case "4":
                    try
                    {
                        await BodyScoreCounter.CountAsync(loggedUser);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("We encountered a problem with calculations :((");
                    }

                    Console.WriteLine(" Press any key to continue");
                    Console.ReadKey();
                    break;
                case "5":
                    try
                    {
                        KcalCalculator.CalculateMakros(loggedUser, lastProgress);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("We encountered a problem with calculations :((");
                    }

                    Console.WriteLine(" Press any key to continue");
                    Console.ReadKey();
                    break;
                case "6":
                    await ViewProfile(loggedUser);
                    break;
                case "7":
                    await CreateWorkoutPlanAsync();
                    break;
                case "8":
                    await PrintWorkoutPlans();
                    Console.WriteLine("Press any key to continue");
                    Console.ReadLine();
                    break;
                case "9":
                    await UpdateWorkoutPlan();
                    break;
                case "10":
                    await DeleteWorkoutPlan();
                    break;
                case "11":
                    loggedUser = null;
                    Console.WriteLine("You have been logged out, press any key to continue");
                    Console.ReadKey();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Selected option is not correct, please try again :)");
                    Console.ReadKey();
                    break;
            }
        }

        async static Task LoginMenu()
        {
            Console.Clear();
            Console.WriteLine("Application Menu");
            Console.WriteLine("---------------");
            Console.WriteLine(" - 1.Register");
            Console.WriteLine(" - 2.Log In");
            Console.WriteLine(" - 0.Exit MENU");

            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    await CreateUser();
                    Console.WriteLine(" Press any key to continue");
                    Console.ReadKey();
                    break;
                case "2":
                    await LogIn();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Selected option is not correct, please try again :)");
                    Console.ReadKey();
                    break;
            }
        }

        async static Task CreateUser()
        {
            Console.WriteLine("write your username: ");
            var name = Console.ReadLine();

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            var email = "";
            while (!Regex.IsMatch(email, emailRegex))
            {
                Console.WriteLine("write your email: ");
                email = Console.ReadLine();

                if (!Regex.IsMatch(email, emailRegex))
                    Console.WriteLine("Invalid email. Please try again.");
            }

            var password = "";
            while (!IsValidPassword(password))
            {
                Console.WriteLine("write your password: ");
                password = Console.ReadLine();
                if (!IsValidPassword(password))
                    Console.WriteLine("Password must be at least 8 characters, " +
                        "contain small letter, big letter, number and special character");
            }

            string hashedPassword = PasswordHasher.HashPassword(password);


            var age = 0;
            while (age <= 0 || age > 100)
            {
                age = GetValidatedInput<int>("write your age: (0-99)");
            }

            var height = 0.0;
            while (height < 80 || height > 220)
            {
                height = GetValidatedInput<double>("write your height: ");
            }

            var diet = "";
            while (diet != "keto" && diet != "standard" && diet != "high-protein")
            {
                Console.WriteLine("write your preferenced diet: (keto/standard/high-protein)");
                diet = Console.ReadLine().ToLower();
            }

            var gender = 't';
            while (gender != 'm' && gender != 'f')
            {
                gender = GetValidatedInput<char>("write your gender m - male || f - female: ");
            }

            var activitylvl = 6;
            while (activitylvl < 0 || activitylvl > 5)
            {
                activitylvl = GetValidatedInput<int>("write your activity level: (1-5)");
            }

            var goals = 6;
            while (goals < 0 || goals > 5)
            {
                goals = GetValidatedInput<int>("write your calorie goals: " +
                    "\n 1 - big deficit 2 - small deficit 3 - maintainance 4 - small surplus 5 - big surplus");
            }
            using (var uow = new UnitOfWork())
            {
                var user1 = new User(name, email, hashedPassword, height, gender, age, activitylvl, goals, diet);
                try
                {
                    uow.UserRepository.Create(user1);
                    await uow.SaveAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine("There already exists account with this email, try to log in or try different email");
                    return;
                }
            }

            Console.WriteLine("User was created");
        }

        async static Task PrintUsers()
        {
            Console.WriteLine("This is a list of the users: ");

            using (var uow = new UnitOfWork())
            {
                var list = await uow.UserRepository.FindAllAsync();
                if (list.Count == 0)
                {
                    Console.WriteLine(" - no users in a database");
                }
                else
                {
                    foreach (var user in list)
                    {
                        Console.WriteLine(" " + user.Id + ". " + user.Username);
                    }
                }
            }
        }

        async static Task LogIn()
        {
            Console.WriteLine("Write your email: ");
            var email = Console.ReadLine();

            Console.WriteLine("Write your password: ");
            var password = Console.ReadLine();

            using (var uow = new UnitOfWork())
            {

                loggedAdmin = await uow.AdminRepository.FindEmailAndPasswordAsync(email, password);
                if (loggedAdmin == null)
                {
                    loggedUser = await uow.UserRepository.FindByEmailAndPasswordAsync(email, password);

                    if (loggedUser == null)
                    {
                        Console.WriteLine("Invalid email or password. Please try again.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    else if (!loggedUser.IsActive)
                    {
                        loggedUser = null;
                        Console.WriteLine("Your account is inactive. Please contact support or an administrator.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {

                        lastProgress = await uow.ProgressRepository.FindLastAsync(loggedUser.Id);
                        Console.WriteLine($"Welcome, {loggedUser.Username}! Press any key to go to the MENU...");
                        Console.ReadKey();
                    }
                }
                else
                {

                    Console.WriteLine($"Welcome, {loggedAdmin.Login}! Press any key to go to the admin panel...");
                    Console.ReadKey();
                }
            }
        }

        async static Task AddProgress()
        {

            var weight = 0.0f;
            var bench = 0.0f;
            var squat = 0.0f;
            var deadlift = 0.0f;

            while (weight <= 0.0 || weight > 500.0)
            {
                weight = GetValidatedInput<float>("Add your current weight (in kg): "); 
            }

            while ( bench <= 0.0 || bench > 400.0)
            {
                bench = GetValidatedInput<float>("Add your max bench (in kg): ");
            }
            
            while ( squat <= 0.0 || squat > 600.0)
            {
                squat = GetValidatedInput<float>("Add your max squat (in kg): ");
            }
           
            while (deadlift <= 0.0 || deadlift > 700)
            {
                deadlift = GetValidatedInput<float>("Add your max deadlift (in kg): ");

            }

            using (var uow = new UnitOfWork())
            {
                var last = await uow.ProgressRepository.FindLastAsync(loggedUser.Id);

                if (last != null)
                {
                    if (bench == null) bench = last.MaxBench;
                    if (squat == null) squat = last.MaxSquat;
                    if (deadlift == null) deadlift = last.MaxDeadlift;
                    if (weight == null) weight = last.Weight;

                }

                Progress progress = new Progress(loggedUser.Id, bench, squat, deadlift, weight);
                lastProgress = progress;
                uow.ProgressRepository.Create(progress);
                await uow.SaveAsync();
                Console.WriteLine("Progress record created");
            }
        }

        async static Task ShowProgress()
        {
            using (var uow = new UnitOfWork())
            {
                var list = await uow.ProgressRepository.FindAllUserProgressAsync(loggedUser.Id);
                if (list.Count == 0)
                {
                    Console.WriteLine(" - no progresses yet");
                }
                else
                {
                    int id = 1;
                    foreach (var progress in list)
                    {

                        Console.WriteLine(id + ". " + progress);
                        id++;
                    }
                }
            }

        }

        async static Task ViewProfile(User user)
        {
            string activity = user.ActivityLevel switch
            {
                1 => "Sedentary",   
                2 => "Lightly active", 
                3 => "Moderately active",  
                4 => "Very active", 
                5 => "Extra active",   
                _ => throw new ArgumentException("Invalid activity level")
            };


            string goal = user.FitnessGoals switch
            {
                1 => "Big deficit",
                2 => "Small deficit",
                3 => "Maintanance",
                4 => "Small surplus",
                5 => "Big surplus",
                _ => throw new ArgumentException("Invalid activity level")

            };

            Console.Clear();
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Age: {user.Age}");
            Console.WriteLine($"Height: {user.Height}");
            Console.WriteLine($"Activity Level: {activity}");
            Console.WriteLine($"Preferred Diet: {user.DietType}");
            Console.WriteLine($"Fitness Goals: {goal}");

            Console.WriteLine("Would you like to modify your profile? (y/n)");
            var option2 = Console.ReadLine();
            switch (option2)
            {
                case "y":
                    Console.Clear();
                    Console.WriteLine("\nWhich field would you like to modify?");
                    Console.WriteLine("1. Age");
                    Console.WriteLine("2. Height");
                    Console.WriteLine("3. Activity Level");
                    Console.WriteLine("4. Preferred Diet");
                    Console.WriteLine("5. Fitness Goals");

                    var fieldOption = Console.ReadLine();
                    using (var uow = new UnitOfWork())
                    {
                        switch (fieldOption)
                        {
                            case "1":
                                Console.WriteLine("Enter your new age:");
                                if (int.TryParse(Console.ReadLine(), out int newAge))
                                {
                                    user.Age = newAge;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid age input.");
                                }
                                break;

                            case "2":
                                Console.WriteLine("Enter your new height (in cm):");
                                if (float.TryParse(Console.ReadLine(), out float newHeight))
                                {
                                    user.Height = newHeight;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid height input.");
                                }
                                break;

                            case "3":
                                Console.WriteLine("Enter your new activity level: (1-5)");
                                if (int.TryParse(Console.ReadLine(), out int newActLevel))
                                {
                                    user.ActivityLevel = newActLevel;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid age input.");
                                }
                                break;

                            case "4":
                                Console.WriteLine("Enter your preferred diet (keto/standard/high-protein):");
                                user.DietType = Console.ReadLine();
                                break;

                            case "5":
                                Console.WriteLine("Enter your fitness goals:  " +
                                    "\n 1 - big deficit 2 - small deficit 3 - maintainance 4 - small surplus 5 - big surplus");
                                if (int.TryParse(Console.ReadLine(), out int newGoals))
                                {
                                    user.FitnessGoals = newGoals;
                                }
                                else
                                {
                                    Console.WriteLine("Invalid age input.");
                                }
                                break;

                            default:
                                Console.WriteLine("Invalid selection. No changes made.");
                                break;
                        }

                        // Save changes to the database
                        await uow.UserRepository.UpdateAsync(user);
                        await uow.SaveAsync();
                        Console.WriteLine("\nProfile updated successfully!");
                    }

                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    break;

                case "n":
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    break;

                default:
                    Console.WriteLine("Selected option is not correct, returning to menu...");
                    break;
            }
        }
        async static Task CreateExercise()
        {
            Console.Clear();
            Console.WriteLine("Create a New Exercise");

            // Get Exercise Name
            Console.WriteLine("Enter the exercise name: ");
            var name = Console.ReadLine();

            // Get Main Body Part
            Console.WriteLine("Enter the main body part targeted (e.g., Chest, Back, Legs): ");
            var mainBodyPart = Console.ReadLine();

            // Get Image URL
            Console.WriteLine("Enter the image URL (or leave blank if not available): ");
            var imageUrl = Console.ReadLine();

            // Get Description
            Console.WriteLine("Enter a brief description of the exercise: ");
            var description = Console.ReadLine();

            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                // Create a new Exercise object
                var exercise = new Exercise
                {
                    Name = name,
                    MainBodyPart = mainBodyPart,
                    ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl,
                    Description = description
                };

                try
                {
                    await uow.ExerciseRepository.CreateAsync(exercise);
                    await uow.SaveAsync();
                    Console.WriteLine("Exercise created successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Could not create the exercise. Details: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                }
            }

            Console.WriteLine("Press any key to return to the admin menu...");
            Console.ReadKey();
        }

        async static Task PrintExercises()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Exercise Library");
                Console.WriteLine("--------------------");
                Console.WriteLine("Choose a filter option:");
                Console.WriteLine("1. View All Exercises");
                Console.WriteLine("2. Search by Muscle Group");
                Console.WriteLine("3. Search by Exercise Name");
                Console.WriteLine("4. Search by Keyword");
                Console.WriteLine("0. Return to Menu");

                var filterOption = Console.ReadLine();

                using (var uow = new UnitOfWork())
                {
                    switch (filterOption)
                    {
                        case "1":
                            // View all exercises
                            DisplayExercises(await uow.ExerciseRepository.FindAllAsync());
                            break;

                        case "2":
                            // Search by Muscle Group
                            Console.WriteLine("Enter the muscle group (e.g., Chest, Back, Legs):");
                            var muscleGroup = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(muscleGroup))
                            {
                                var exercisesByGroup = await uow.ExerciseRepository.FindAllByBodyPartAsync(muscleGroup);
                                DisplayExercises(exercisesByGroup, $"Exercises for Muscle Group: {muscleGroup}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid muscle group input.");
                            }
                            break;

                        case "3":
                            // Search by Exercise Name
                            Console.WriteLine("Enter the exercise name or part of the name:");
                            var exerciseName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(exerciseName))
                            {
                                var exercisesByName = await uow.ExerciseRepository.SearchExercisesByNameAsync(exerciseName);
                                DisplayExercises(exercisesByName, $"Search Results for Name: {exerciseName}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid input.");
                            }
                            break;

                        case "4":
                            // Search by Keyword
                            Console.WriteLine("Enter a keyword to search for exercises:");
                            var keyword = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(keyword))
                            {
                                var exercisesByKeyword = await uow.ExerciseRepository.SearchExercisesByKeywordAsync(keyword);
                                DisplayExercises(exercisesByKeyword, $"Search Results for Keyword: {keyword}");
                            }
                            else
                            {
                                Console.WriteLine("Invalid input.");
                            }
                            break;

                        case "0":
                            // Return to menu
                            Console.WriteLine("Returning to the menu...");
                            return;

                        default:
                            Console.WriteLine("Invalid option. Try again. Press any key to continue");
                            Console.ReadKey();
                            break;
                    }
                }
            }

        }

        static void DisplayExercises(List<Exercise> exercises, string header = "All Exercises")
        {
            Console.Clear();
            Console.WriteLine(header);
            Console.WriteLine("-------------------");

            if (exercises.Count == 0)
            {
                Console.WriteLine("No exercises found.");
                return;
            }

            for (int i = 0; i < exercises.Count; i++)
            {
                Console.WriteLine($" - {i + 1}. {exercises[i].Name} (Body Part: {exercises[i].MainBodyPart})");
            }

            Console.WriteLine("\nChoose an exercise by number to view details, or type 0 to go back:");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= exercises.Count)
            {
                var selectedExercise = exercises[choice - 1];
                Console.Clear();
                Console.WriteLine($"Exercise Details for: {selectedExercise.Name}");
                Console.WriteLine($" - Main Body Part: {selectedExercise.MainBodyPart}");
                Console.WriteLine($" - Description: {selectedExercise.Description}");
                Console.WriteLine($" - Image URL: {selectedExercise.ImageUrl}");

                Console.WriteLine("Press any key to return to menu");
            }
            else if (choice == 0)
            {
                Console.WriteLine("Returning to the previous menu... Press any key to continue");
                Console.ReadKey();
                return;
            }
            else
            {
                Console.WriteLine("Invalid selection. Press any key to continue");
            }
            Console.ReadKey();
        }

        async static Task SearchUser()
        {
            Console.WriteLine("Write the username you wish to find:");
            var name = Console.ReadLine();

            using(var uow = new UnitOfWork())
            {
                try
                {
                    var user = await uow.UserRepository.FindWithUsernameAsync(name);

                    if (user == null)
                    {
                        Console.WriteLine($"User with username '{name}' not found.");
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("User Details:");
                        Console.WriteLine($" - Username: {user.Username}");
                        Console.WriteLine($" - Email: {user.Email}");
                        Console.WriteLine($" - Age: {user.Age}");
                        Console.WriteLine($" - Height: {user.Height}");
                        Console.WriteLine($" - Activity Level: {user.ActivityLevel}");
                        Console.WriteLine($" - Preferred Diet: {user.DietType}");
                        Console.WriteLine($" - Fitness Goals: {user.FitnessGoals}");
                        Console.WriteLine($" - Active: {(user.IsActive ? "Yes" : "No")}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while retrieving the user: {ex.Message}");
                }
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }

        async static Task EditExercise()
        {
            Console.Clear();
            Console.WriteLine("Edit an Existing Exercise");

            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                var exercises = await uow.ExerciseRepository.FindAllAsync();
                if (exercises.Count == 0)
                {
                    Console.WriteLine("No exercises available to edit.");
                    return;
                }

                Console.WriteLine("Available Exercises:");
                for (int i = 0; i < exercises.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {exercises[i].Name}");
                }

                Console.WriteLine("\nEnter the number of the exercise you want to edit:");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= exercises.Count)
                {
                    var selectedExercise = exercises[choice - 1];
                    Console.WriteLine($"Editing Exercise: {selectedExercise.Name}");

                    Console.WriteLine("Enter new name (leave blank to keep current):");
                    var name = Console.ReadLine();
                    selectedExercise.Name = string.IsNullOrWhiteSpace(name) ? selectedExercise.Name : name;

                    Console.WriteLine("Enter new main body part (leave blank to keep current):");
                    var mainBodyPart = Console.ReadLine();
                    selectedExercise.MainBodyPart = string.IsNullOrWhiteSpace(mainBodyPart) ? selectedExercise.MainBodyPart : mainBodyPart;

                    Console.WriteLine("Enter new image URL (leave blank to keep current):");
                    var imageUrl = Console.ReadLine();
                    selectedExercise.ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? selectedExercise.ImageUrl : imageUrl;

                    Console.WriteLine("Enter new description (leave blank to keep current):");
                    var description = Console.ReadLine();
                    selectedExercise.Description = string.IsNullOrWhiteSpace(description) ? selectedExercise.Description : description;

                    try
                    {
                        await uow.ExerciseRepository.EditAsync(selectedExercise);
                        Console.WriteLine("Exercise updated successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error updating exercise: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection. Returning to menu.");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        async static Task DeleteExercise()
        {
            Console.Clear();
            Console.WriteLine("Delete an Existing Exercise");

            using (var uow = new UnitOfWork(new AppDbContext()))
            {
                var exercises = await uow.ExerciseRepository.FindAllAsync();
                if (exercises.Count == 0)
                {
                    Console.WriteLine("No exercises available to delete.");
                    return;
                }

                Console.WriteLine("Available Exercises:");
                for (int i = 0; i < exercises.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {exercises[i].Name}");
                }

                Console.WriteLine("\nEnter the number of the exercise you want to delete:");
                if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= exercises.Count)
                {
                    var selectedExercise = exercises[choice - 1];
                    Console.WriteLine($"Are you sure you want to delete the exercise: {selectedExercise.Name}? (y/n)");

                    var confirmation = Console.ReadLine();
                    if (confirmation != null && confirmation.ToLower() == "y")
                    {
                        try
                        {
                            await uow.ExerciseRepository.DeleteAsync(selectedExercise);
                            Console.WriteLine("Exercise deleted successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting exercise: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Deletion canceled. Returning to menu...");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection. Returning to menu...");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        static T GetValidatedInput<T>(string message) where T : struct
        {
            T converted;
            while (true)
            {
                Console.WriteLine(message);
                var input = Console.ReadLine();
                if (TryParse<T>(input, out converted))
                    break;

                Console.WriteLine("Type of input is invalid, please try again!");
            }
            return converted;
        }

        async static Task ToggleUserActivation()
        {
            Console.Clear();
            Console.WriteLine("Activate/Deactivate a User");
            Console.WriteLine("--------------------------");

            Console.WriteLine("Type the email of the user you want to activate/deactivate, or type 'back' to return:");

            var input = Console.ReadLine();

            if (input.ToLower() == "back")
            {
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            using (var uow = new UnitOfWork())
            {
                try
                {

                    var selectedUser = await uow.UserRepository.FindByEmailAsync(input);

                    if (selectedUser == null)
                    {
                        Console.WriteLine("No user found with that email.");
                    }
                    else
                    {

                        if (selectedUser.IsActive)
                        {
                            await uow.UserRepository.DeactivateUserAsync(selectedUser.Id);
                            Console.WriteLine($"User {selectedUser.Username} ({selectedUser.Email}) has been deactivated.");
                        }
                        else
                        {
                            await uow.UserRepository.ActivateUserAsync(selectedUser.Id);
                            Console.WriteLine($"User {selectedUser.Username} ({selectedUser.Email}) has been activated.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey();
        }

        static bool TryParse<T>(string input, out T result) where T : struct
        {
            try
            {
                result = (T)Convert.ChangeType(input, typeof(T));
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));
            bool hasMinLength = password.Length >= 8;

            return hasUpper && hasLower && hasDigit && hasSpecial && hasMinLength;
        }


        static async Task CreateWorkoutPlanAsync()
        {
            Console.WriteLine("Welcome to WorkoutPlan creation menu !!!");
            var name = "";
            while(name == "")
            {
                Console.WriteLine("Name of your workout plan: ");
                name = Console.ReadLine();
            }

            Console.WriteLine("Describe this workout plan: ");
            var description = Console.ReadLine();


            using (var uow = new UnitOfWork())
            {
                var exercises = await uow.ExerciseRepository.FindAllAsync();
                if (exercises.Count == 0)
                {
                    Console.WriteLine(" - No exercises found in the database. Press any key to return");
                    Console.ReadLine();
                }
                else
                {

                    WorkoutPlan plan = new WorkoutPlan(loggedUser.Id, name, description);
                    uow.WorkoutPlanRepository.Create(plan);
                    await uow.SaveAsync();
                    var workoutPlan = await uow.WorkoutPlanRepository.FindUserLastAsync(loggedUser.Id);
                    var input = 0;
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("List of exercises:");
                        for (int i = 0; i < exercises.Count; i++)
                        {
                            Console.WriteLine($" - {i + 1}. {exercises[i].Name}");
                        }

                        input = GetValidatedInput<int>("Select an exercise you want to add, input 0 if you finished creating your Workout Plan");
                        if (input == 0) break;

                        ExerciseSet exerciseSet = CreateExerciseSet(exercises, workoutPlan,input);
                        if(exerciseSet != null)
                            await uow.WorkoutPlanRepository.UpdateExerciseSet(workoutPlan.Id, exerciseSet);
                      
                    }

                    // Deleting created workoutPlan if exercises wasn't added
                    var updatedWorkoutPlan = await uow.WorkoutPlanRepository.FindByIdAsync(workoutPlan.Id);
                    if (updatedWorkoutPlan.ExerciseSets == null || updatedWorkoutPlan.ExerciseSets.Count == 0)
                    {
                        uow.WorkoutPlanRepository.Delete(updatedWorkoutPlan);
                        await uow.SaveAsync();
                        return;
                    }


                    await uow.SaveAsync();
                    Console.WriteLine("Workout Plan has been created");
                    await ListWorkoutPlan(workoutPlan.Id);
                    Console.WriteLine("Such a nice workout plan :O :) Press any key to continue");
                    Console.ReadLine();
                }


            }
        }


        static ExerciseSet CreateExerciseSet(List<Exercise> exercises, WorkoutPlan plan,int input)
        {

            if (input > 0 && input <= exercises.Count)
            {
                var numberOfSets = 100;
                while (numberOfSets > 99 || numberOfSets <= 0)
                {
                    numberOfSets = GetValidatedInput<int>("Input number of sets");
                    if (numberOfSets > 99 || numberOfSets <= 0) Console.WriteLine("Number of sets needs to be in range 1-99");
                }

                var repetitions = 200;
                while (repetitions > 199 || repetitions <= 0)
                {
                    repetitions = GetValidatedInput<int>("Input number of repetitions per set");
                    if (repetitions > 199 || repetitions <= 0) Console.WriteLine("Number of repetitions needs to be in range 1-199");
                }

                return new ExerciseSet(repetitions, numberOfSets, exercises[input - 1].Id, plan.Id);
            }
            else
            {
                Console.WriteLine("This exercise number does not exist Press any key to try again");
                Console.ReadKey();
                return null;
            }
        }


        static async Task ListWorkoutPlan(int id)
        {
            using (var uow = new UnitOfWork())
            {
                WorkoutPlan plan = await uow.WorkoutPlanRepository.FindByIdAsync(id);
                List<ExerciseSet> sets = await uow.ExerciseSetRepository.FindWorkoutPlanAllAsync(id);
                Console.WriteLine($"Workout plan \"{plan.Name}\" contains: ");
                foreach (var set in sets)
                {
                    Exercise ex = await uow.ExerciseRepository.FindByIdAsync(set.ExerciseId);
                    Console.WriteLine($" - {ex.Name} {set.Sets}x{set.Reps}");
                }
            }
        }


        async static Task PrintWorkoutPlans()
        {
            using (var uow = new UnitOfWork())
            {
                var list = await uow.WorkoutPlanRepository.FindAllUserWorkoutPlans(loggedUser.Id);
                if (list.Count == 0)
                {
                    Console.WriteLine(" You don't have workout plans added :( Press any key to continue");
                    Console.ReadKey();
                    
                }
                else
                {
                    foreach (var plan in list)
                    {
                        await ListWorkoutPlan(plan.Id);     
                    }
                }
            }
        }

        async static Task DeleteWorkoutPlan()
        {
            Console.Clear();
            Console.WriteLine("Delete a Workout Plan");

            using (var uow = new UnitOfWork())
            {
                var list = await uow.WorkoutPlanRepository.FindAllUserWorkoutPlans(loggedUser.Id);
                if (list.Count == 0)
                {
                    Console.WriteLine("You don't have workout plans added :( Press any key to continue");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Available Workout Plans:");
                for (int i = 0; i < list.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {list[i].Name}");
                }

                int choice = GetValidatedInput<int>("Choose the number of the workout plan you want to delete. Input 0 to go back:");
                if (choice == 0) return;

                if (choice < 1 || choice > list.Count)
                {
                    Console.WriteLine("Invalid number. Press any key to return to the menu.");
                    Console.ReadKey();
                    return;
                }

                // Delete selected WorkoutPlan ExerciseSets cascade deletion.
                var plan = list[choice - 1];
                try
                {
                    uow.WorkoutPlanRepository.Delete(plan);
                    await uow.SaveAsync();
                    Console.WriteLine($"Successfully deleted workout plan \"{plan.Name}\". Press any key to continue.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting workout plan: {ex.Message}");
                }

                Console.ReadKey();
            }
        }


        async static Task UpdateWorkoutPlan()
        {
            Console.Clear();
            Console.WriteLine("Update a Workout Plan");

            using (var uow = new UnitOfWork())
            {
                var list = await uow.WorkoutPlanRepository.FindAllUserWorkoutPlans(loggedUser.Id);
                if (list.Count == 0)
                {
                    Console.WriteLine("You don't have any workout plans to update :( Press any key to continue");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("Available Workout Plans:");
                for (int i = 0; i < list.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {list[i].Name}");
                }

                int choice = GetValidatedInput<int>("Choose the number of the workout plan you want to update. Input 0 to go back:");
                if (choice == 0) return;

                if (choice < 1 || choice > list.Count)
                {
                    Console.WriteLine("Invalid number. Press any key to return to the menu.");
                    Console.ReadKey();
                    return;
                }

                var selectedPlan = list[choice - 1];
                Console.WriteLine($"Updating workout plan: \"{selectedPlan.Name}\"");


                Console.WriteLine("Enter new name for the workout plan (leave blank to keep current):");
                string newName = Console.ReadLine();

                Console.WriteLine("Enter new description for the workout plan (leave blank to keep current):");
                string newDescription = Console.ReadLine();

                var exerciseSets = await uow.ExerciseSetRepository.FindWorkoutPlanAllAsync(selectedPlan.Id);
                Console.WriteLine("Current Exercises in the Plan:");
                for (int i = 0; i < exerciseSets.Count; i++)
                {
                    var exercise = await uow.ExerciseRepository.FindByIdAsync(exerciseSets[i].ExerciseId);
                    Console.WriteLine($"{i + 1}. {exercise.Name} ({exerciseSets[i].Sets} sets x {exerciseSets[i].Reps} reps)");
                }

                while (true)
                {
                    Console.WriteLine("=== Exercise Set Menu ===");
                    Console.WriteLine("1. Add Exercise Set");
                    Console.WriteLine("2. Delete Exercise Set");
                    Console.WriteLine("0. Finish Editing");
                    int action = GetValidatedInput<int>("Choose an action:");

                    if (action == 0) break;

                    switch (action)
                    {
                        case 1:
                            var exercises = await uow.ExerciseRepository.FindAllAsync();
                            Console.Clear();
                            Console.WriteLine("List of exercises:");
                            for (int i = 0; i < exercises.Count; i++)
                            {
                                Console.WriteLine($" - {i + 1}. {exercises[i].Name}");
                            }

                            var input = GetValidatedInput<int>("Select an exercise you want to add, input 0 if you finished updating your Workout Plan");
                            if (input == 0) break;

                            ExerciseSet exerciseSet = CreateExerciseSet(exercises, selectedPlan, input);
                            if (exerciseSet != null)
                                await uow.WorkoutPlanRepository.UpdateExerciseSet(selectedPlan.Id, exerciseSet);
                            break;
                        case 2:
                            Console.WriteLine("There is no Deletion of ExerviseSet implemented yet");
                            break;
                        default:
                            Console.WriteLine("Invalid action. Please choose again.");
                            break;
                    }
                }




                selectedPlan.Description = string.IsNullOrWhiteSpace(newDescription) ? selectedPlan.Description : newDescription;
                selectedPlan.Name = string.IsNullOrWhiteSpace(newName) ? selectedPlan.Name : newName;
                try
                {
                    uow.WorkoutPlanRepository.Update(selectedPlan);
                    await uow.SaveAsync();
                    Console.WriteLine($"Workout plan \"{selectedPlan.Name}\" updated successfully. Press any key to continue.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating workout plan: {ex.Message}");
                }

                Console.ReadKey();
            }
        }






    }
}
