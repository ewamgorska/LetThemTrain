using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using LetEmTrain.UWP.ViewModels;
using Windows.UI.Xaml.Media.Animation;
using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using System.Threading.Tasks;

namespace LetEmTrain.UWP.Views.WorkoutTemplates
{
    public sealed partial class EditTemplatePage : Page
    {

        ExerciseSetViewModel ExerciseSetViewModel { get; set; }
        ExerciseViewModel ExerciseViewModel { get; set; }   
        WorkoutPlanViewModel WorkoutPlanViewModel { get; set; }

        public EditTemplatePage()
        {
            this.InitializeComponent();
            ExerciseViewModel = App.ExerciseViewModel;
            WorkoutPlanViewModel = App.WorkoutPlanViewModel;
            ExerciseSetViewModel = App.ExerciseSetViewModel;
            ExerciseViewModel.LoadAllAsync();
        }

        // 
        private void AddExerciseSet_Click(object sender, RoutedEventArgs e)
        {
            if (ExerciseViewModel.SelectedExercise == null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please select an exercise before adding a set.",
                    CloseButtonText = "OK"
                };
                _ = dialog.ShowAsync();
                return;
            }

            if (!int.TryParse(ExerciseSetViewModel.Reps, out int reps) || reps <= 0)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please enter a valid positive number for reps.",
                    CloseButtonText = "OK"
                };
                _ = dialog.ShowAsync();
                return;
            }

            if (!int.TryParse(ExerciseSetViewModel.Sets, out int sets) || sets <= 0)
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please enter a valid positive number for sets.",
                    CloseButtonText = "OK"
                };
                _ = dialog.ShowAsync();
                return;
            }

            var newExerciseSet = new ExerciseSet
            {
                Reps = reps,
                Sets = sets,
                ExerciseId = ExerciseViewModel.SelectedExercise.Id,
                Exercise = ExerciseViewModel.SelectedExercise,
                WorkoutPlanId = WorkoutPlanViewModel.SelectedWorkoutPlan.Id,
                WorkoutPlan = WorkoutPlanViewModel.SelectedWorkoutPlan
            };

            ExerciseSetViewModel.ExerciseSets.Add(newExerciseSet);
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MyTemplatesPage));
        }



        private void DeleteExerciseSet_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var exerciseSetToDelete = button.DataContext as ExerciseSet;
            ExerciseSetViewModel.ExerciseSets.Remove(exerciseSetToDelete);
        }



        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(ExerciseSetViewModel.ExerciseSets.Count != 0)
            {
                await ExerciseSetViewModel.SaveExerciseSets();
            }
            await WorkoutPlanViewModel.SaveEditedWorkoutPlan();
            Frame.Navigate(typeof(MyTemplatesPage));
        }
    }


}
