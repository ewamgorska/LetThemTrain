using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LetEmTrain.UWP.ViewModels
{
    public class AdminViewModel : BindableBase
    {
        public Admin Admin { get; set; }

        private Admin _loggedAdmin;

        public Admin LoggedAdmin
        {
            get { return _loggedAdmin; }
            set
            {
                _loggedAdmin = value;
                OnPropertyChanged();

            }
        }

        public string Name { get; set; }

        public AdminViewModel()
        {
            Admin = new Admin();


        }

        public async Task<bool> LoginAsync()
        {
            if (string.IsNullOrEmpty(Admin.Email) || string.IsNullOrEmpty(Admin.Password))
            {
                await ShowContentDialogAsync("Empty Inputs", "Please fill in all required fields.");
                return false;
            }

            try
            {
                using (var uow = new UnitOfWork())
                {

                    var admin = await uow.AdminRepository.FindEmailAndPasswordAsync(Admin.Email, Admin.Password);

                    if (admin == null)
                    {
                        await ShowContentDialogAsync("Login Failed", "Invalid email or password.");
                        return false;
                    }
                    LoggedAdmin = admin;
                    return true;
                }
            }
            catch (Exception ex)
            {
                await ShowContentDialogAsync("Error", $"An error occurred during login: {ex.Message}");
                return false;
            }

        }
        public ObservableCollection<Exercise> Exercises { get; set; } = new ObservableCollection<Exercise>();

        public async Task LoadExercisesAsync()
        {
            using (var uow = new UnitOfWork())
            {
                var exercises = await uow.ExerciseRepository.FindAllAsync();
                Exercises.Clear();  // Clear existing exercises
                foreach (var exercise in exercises)
                {
                    Exercises.Add(exercise);  // Add the exercises to the collection
                }
            }
        }

        public async Task AddExerciseAsync(string exerciseName, string description, string mainBodyPart, byte[] image) 
        {
            using (var uow = new UnitOfWork())
            {
                var newExercise = new Exercise
                {
                    Name = exerciseName,
                    Description = description,
                    MainBodyPart = mainBodyPart,
                    Image = image
                };

                uow.ExerciseRepository.Create(newExercise);
                await uow.SaveAsync();
            }
        }

        public async Task SaveExerciseAsync(Exercise exercise)
        {
            using (var uow = new UnitOfWork())
            {
                if (exercise.Id == 0)
                {
                    // Create new exercise
                    uow.ExerciseRepository.Create(exercise);
                }
                else
                {
                    // Update existing exercise
                    uow.ExerciseRepository.Update(exercise);
                }

                await uow.SaveAsync();
            }
        }

        public async Task DeleteExerciseAsync(Exercise exerciseToDelete)
        {
            using (var uow = new UnitOfWork())
            {
                uow.ExerciseRepository.Delete(exerciseToDelete);
                await uow.SaveAsync();
            }
        }
        public async Task SearchExercisesAsync(string name)
        {
            using (var uow = new UnitOfWork())
            {
                var exercises = await uow.ExerciseRepository.SearchExercisesByNameAsync(name);

                Exercises.Clear();
                foreach (var exercise in exercises)
                {
                    Exercises.Add(exercise);
                }
            }
        }

        public void FilterExercises(string query)
        {
            var filtered = Exercises.Where(ex => ex.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).OrderBy(ex => ex.Name).ToList();
            Exercises = new ObservableCollection<Exercise>(filtered);
            OnPropertyChanged(nameof(Exercises));  // Notify that Exercises have been updated
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


        internal void Logout()
        {
            LoggedAdmin = Admin = new Admin();
            LoggedAdmin = null;
        }
    }

}