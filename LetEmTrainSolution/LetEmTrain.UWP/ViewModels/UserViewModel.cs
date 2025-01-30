using LetEmTrain.Domain;
using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.Repositories;
using LetEmTrain.Infrastructure;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using User = LetEmTrain.Domain.Models.User;

namespace LetEmTrain.UWP.ViewModels
{
    public class UserViewModel : BindableBase
    {
        public User User { get; set; }

        private User _loggedUser;

        public User LoggedUser
        {
            get { return _loggedUser; }
            set
            {
                _loggedUser = value;
                OnPropertyChanged();
            }
        }


        private string _height;
        public string Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    OnPropertyChanged();
                    System.Diagnostics.Debug.WriteLine($"Height set to: {_height}");
                }
            }
        }

        private char _gender;
        public char Gender
        {
            get { return _gender; }
            set { _gender = value;}

        }
        public ComboBox DietTypeComboBox { get; set; }
        public ComboBox ActivityLevelComboBox { get; set; }
        public ComboBox FitnessGoalsComboBox { get; set; }

        public string ActivityLevelDescription
        {
            get
            {
                switch (ActivityLvl)
                {
                    case 1:
                        return "1 - Sedentary";
                    case 2:
                        return "2 - Lightly active";
                    case 3:
                        return "3 - Moderately active";
                    case 4:
                        return "4 - Very active";
                    case 5:
                        return "5 - Extra active";
                    default:
                        return "Unknown";
                }
            }
        }

        public string FitnessGoalsDescription
        {
            get
            {
                switch (FitnessGoals)
                {
                    case 1:
                        return "Big deficit";
                    case 2:
                        return "Small deficit";
                    case 3:
                        return "Maintenance";
                    case 4:
                        return "Small surplus";
                    case 5:
                        return "Big surplus";
                    default:
                        return "Unknown";
                }
            }
        }

        public ObservableCollection<User> Users { get; set; }


        private string _age;
        public string Age
        {
            get { return _age; }
            set { _age = value; }
        }

        private int _activitylvl;
        public int ActivityLvl
        {
            get { return _activitylvl; }
            set { _activitylvl = value; }
        }

        private int _fitnessgoals;
        public int FitnessGoals
        {
            get { return _fitnessgoals; }
            set { _fitnessgoals = value; }
        }

        private string _diettype;
        public string DietType
        {
            get { return _diettype; }
            set
            {
                if (_diettype != value)
                {
                    _diettype = value;
                    OnPropertyChanged(); 
                }
            }
        }


        private byte[] _profilePicture;

        public byte[] ProfilePicture
        {
            get { return _profilePicture; }
            set { Set(ref _profilePicture, value); }
        }

        public UserViewModel()
        {
            User = new User();
            Users = new ObservableCollection<User>();
            
        } 

        public async Task<bool> CreateAccountAsync()
        { 

            if (string.IsNullOrWhiteSpace(User.Username) || string.IsNullOrWhiteSpace(User.Email) || string.IsNullOrWhiteSpace(User.Password))
            {
                await ShowContentDialogAsync("Blank inputs", "Fill all your account information");
                return false;
            }

            double height = double.TryParse(Height, out var parsedheight) ? parsedheight : 0;
            int age = int.TryParse(Age, out var parsedage) ? parsedage : 0;

            if (age < 0 || age > 99 || height < 80 || height > 220)
            {
                await ShowContentDialogAsync("Wrong Numbers", "Add a valid age and height numbers (age: 0-99, height: 80-220)");
                return false;

            }
            User.Age = age;
            User.Height = height;



            if (Gender == 'N' || ActivityLvl == 0 || FitnessGoals == 0 || string.IsNullOrEmpty(DietType))
            {
                await ShowContentDialogAsync("Items not selected", "Please select all options: Diet, Gender, Activity level and Fitness goals");
                return false;
            }
            User.Gender = Gender;
            User.ActivityLevel = ActivityLvl;
            User.FitnessGoals = FitnessGoals;
            User.DietType = DietType;

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(User.Email, emailRegex))
            {
                await ShowContentDialogAsync("Invalid Email", "Please enter a valid email address");
                return false;

            }


            if (!IsValidPassword(User.Password))
            {
                await ShowContentDialogAsync("Invalid Password", "Password must be at least 8 characters, contain small letter, big letter, number and special character");
                return false;
            }

            User.Password = PasswordHasher.HashPassword(User.Password);
            
            try
            {
                using (var uow = new UnitOfWork())
                {
                    {
                        uow.UserRepository.Create(User);
                        await uow.SaveAsync();
                    }
                    // await ShowContentDialogAsync("Success!", "Account created successfully");
                    LoggedUser = User;
                    return true;
                }
            }
            catch (Exception ex)
            {

                await ShowContentDialogAsync("Error", $"An error occurred while creating the account: {ex.Message}");
                return false;
            }
          
        }


        public async Task<bool> LoginAsync()
        {
            if (string.IsNullOrEmpty(User.Email) || string.IsNullOrEmpty(User.Password))
            {
                await ShowContentDialogAsync("Empty Inputs", "Please fill in all required fields.");
                return false;
            }

            try
            {
                using (var uow = new UnitOfWork())
                {
                    // Check if user exists in the database
                    var user = await uow.UserRepository.FindByEmailAndPasswordAsync(User.Email, User.Password);

                    if (user == null)
                    {
                        await ShowContentDialogAsync("Login Failed", "Invalid email or password.");
                        return false;
                    }
                    if (!user.IsActive)
                    {
                        await ShowContentDialogAsync("Login Failed", "User account is inactive. Please contact the Administrator");
                        return false;
                    }
                    // Populate UserViewModel with user data
                    LoggedUser = user;

                    // Map user properties to ViewModel properties
                    User = user;
                    Height = user.Height.ToString();
                    Age = user.Age.ToString();
                    Gender = user.Gender;
                    ActivityLvl = user.ActivityLevel;
                    FitnessGoals = user.FitnessGoals;
                    DietType = user.DietType;
                    ProfilePicture = user.ProfilePicture;
                    

                    System.Diagnostics.Debug.WriteLine($"[LoginAsync] Logged-in DietType: {DietType}");
                    System.Diagnostics.Debug.WriteLine($"[LoginAsync] Logged-in ActivityLevel: {ActivityLvl}");
                    System.Diagnostics.Debug.WriteLine($"[LoginAsync] Logged-in FitnessGoals: {FitnessGoals}");

                    return true;
                }
            }
            catch (Exception ex)
            {
                await ShowContentDialogAsync("Error", $"An error occurred during login: {ex.Message}");
                return false;
            }
        }



        private async Task ShowContentDialogAsync(string title, string content)
        {
            await new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK"
            }.ShowAsync();
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


        public async Task SaveProfileChangesAsync()
        {
            if (User == null) return;

            // Extract and assign dropdown values
            User.DietType = DietTypeComboBox?.SelectedItem is ComboBoxItem dietItem ? dietItem.Content.ToString() : null;
            User.ActivityLevel = ActivityLevelComboBox?.SelectedIndex + 1 ?? 0;
            User.FitnessGoals = FitnessGoalsComboBox?.SelectedIndex + 1 ?? 0;

            // Validate dropdown selections
            if (string.IsNullOrEmpty(User.DietType) || User.ActivityLevel <= 0 || User.FitnessGoals <= 0)
            {
                await ShowContentDialogAsync("Invalid Selection", "Please select valid options for Diet, Activity Level, and Fitness Goals.");
                return;
            }

            try
            {
                using (var uow = new UnitOfWork())
                {
                    uow.UserRepository.Update(User);
                    await uow.SaveAsync();
                }

                // Update the ViewModel properties with the saved User data
                DietType = User.DietType;
                ActivityLvl = User.ActivityLevel;
                FitnessGoals = User.FitnessGoals;

                await ShowContentDialogAsync("Success", "Your profile changes have been saved successfully.");
            }
            catch (Exception ex)
            {
                await ShowContentDialogAsync("Error", $"An error occurred while saving changes: {ex.Message}");
            }
        }

        public async Task SaveProfilePictureAsync()
        {
            LoggedUser.ProfilePicture = this.ProfilePicture;
            using (var uow = new UnitOfWork())
            {
                uow.UserRepository.Update(LoggedUser);
                await uow.SaveAsync();
            }

        }

        public async Task LoadAllUsersAsync()
        {
            using (var uow = new UnitOfWork())
            {
                var users = await uow.UserRepository.GetAllAsync(); // Updated to match method name in IUserRepository
                Users.Clear();
                foreach(var u in users)
                {
                    Users.Add(u);
                }
            }
        }

        public async Task SearchUsersAsync(string query)
        {
            using (var uow = new UnitOfWork())
            {
                var users = await uow.UserRepository.SearchUsersByEmailAsync(query);
                Users.Clear();
                foreach (var u in users)
                {
                    Users.Add(u);
                }
            }
        }

        public async Task ToggleUserStatusAsync(int userId, bool isActive)
        {
            using (var uow = new UnitOfWork())
            {
                var user = await uow.UserRepository.FindByIdAsync(userId); // Using the newly added method
                if (user != null)
                {
                    user.IsActive = isActive;
                    await uow.SaveAsync();
                    await LoadAllUsersAsync(); // Reload the list after updating
                }
            }
        }


        internal void Logout()
        {
            LoggedUser = User = new User();
            LoggedUser = null;
        }

    }
}
