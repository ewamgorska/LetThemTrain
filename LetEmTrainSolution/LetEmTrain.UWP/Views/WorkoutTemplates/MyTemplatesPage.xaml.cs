using LetEmTrain.UWP.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using System.Linq;
using System;
using Windows.UI.Xaml.Media;
using LetEmTrain.Domain.Models;
using LetEmTrain.Infrastructure;
using LetEmTrain.UWP.Views.Logging;
using Windows.Storage.Pickers.Provider;
using System.Text;

namespace LetEmTrain.UWP.Views.WorkoutTemplates
{
    public sealed partial class MyTemplatesPage : Page
    {
        public WorkoutPlanViewModel WorkoutPlanViewModel { get; set; }
        public ExerciseSetViewModel ExerciseSetViewModel { get; set; }

        public MyTemplatesPage()
        {
            this.InitializeComponent();
            WorkoutPlanViewModel = App.WorkoutPlanViewModel;
            ExerciseSetViewModel = App.ExerciseSetViewModel;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            WorkoutPlanViewModel.LoadAllAsync();
            base.OnNavigatedTo(e);

        }
   

        private async void asbWorkOutPlans_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                await WorkoutPlanViewModel.SearchWorkoutPlansAsync(sender.Text);
            }
        }

        private async void asbWorkOutPlans_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await WorkoutPlanViewModel.SearchWorkoutPlansAsync(args.QueryText);
        }

        private void asbWorkOutPlans_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem as string;
        }


        private async void ButtonDeleteTemplate(object sender , RoutedEventArgs e)
        {
            var button = sender as Button;
            var planToDelete = button.DataContext as WorkoutPlan;
            ContentDialog deleteDialog = new ContentDialog()
            {
                Title = "Do you want to delete this workout plan?",
                Content = "It's permament, meaning veeery long time. Are you sure ?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Close"
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await ExerciseSetViewModel.DeleteExerciseSets(planToDelete.Id);
                await WorkoutPlanViewModel.DeleteWorkoutPlanAsync(planToDelete);
            }
            if (WorkoutPlanViewModel.WorkoutPlans.Count == 0)
                WorkoutPlanViewModel.Placeholder = "U haven't added Workout Plan yet";
        }

        // Showing content of workoutPlan
        private async void ButtonViewSets(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var chosenPlan = button.DataContext as WorkoutPlan;
            WorkoutPlanViewModel.SelectedWorkoutPlan = chosenPlan;
            await ExerciseSetViewModel.LoadAllFromWorkoutPlanAsync();

            if (ExerciseSetViewModel.ExerciseSets == null || ExerciseSetViewModel.ExerciseSets.Count == 0)
            {
                var emptyDialog = new ContentDialog
                {
                    Title = "No Exercise Sets",
                    Content = "There are no exercise sets to display.",
                    CloseButtonText = "OK"
                };

                await emptyDialog.ShowAsync();
                return;
            }
                
            var contentBuilder = new StringBuilder();
            foreach (var exerciseSet in ExerciseSetViewModel.ExerciseSets)
            {
                contentBuilder.AppendLine($"- {exerciseSet.Exercise.Name} (Main Body Part: {exerciseSet.Exercise.MainBodyPart})");
                contentBuilder.AppendLine($"  Sets: {exerciseSet.Sets}, Reps: {exerciseSet.Reps}");
                contentBuilder.AppendLine();
            }

            var contentTextBlock = new TextBlock
            {
                Text = contentBuilder.ToString(),
                TextWrapping = TextWrapping.Wrap,
            };

            var scrollViewer = new ScrollViewer
            {
                Content = contentTextBlock,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            };


            // Display the workout plan details in a ContentDialog
            var workoutPlanContentDialog = new ContentDialog
            {
                Title = $"WorkoutPlan '{chosenPlan.Name}' contains:",
                Content = scrollViewer,
                CloseButtonText = "Exit"
            };

            await workoutPlanContentDialog.ShowAsync();
        }

        // Editing existing templates
        private async void ButtonEditTemplate(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var planToUpdate = button.DataContext as WorkoutPlan;
            WorkoutPlanViewModel.SelectedWorkoutPlan = planToUpdate;

            await ExerciseSetViewModel.LoadAllFromWorkoutPlanAsync();
           
            Frame.Navigate(typeof(EditTemplatePage));        
        }


        // Adding new template
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
           
            var Name = new TextBox
            {
                PlaceholderText = "Template Name",
                Width = 300,
                Margin = new Thickness(0, 5, 0, 5)
            };

            var Description = new TextBox
            {
                PlaceholderText = "Template Description",
                Width = 300,
                Margin = new Thickness(0, 5, 0, 5)
            };

            var dialog = new ContentDialog
            {
                Title = "Create New Template",
                PrimaryButtonText = "Create",
                CloseButtonText = "Cancel",
                Content = new StackPanel
                {
                    Children = {
                        new TextBlock { Text = "Template Name", Margin = new Thickness(0, 0, 0, 5) },Name,
                        new TextBlock { Text = "Template Description", Margin = new Thickness(0, 10, 0, 5) },Description
                    }
                }
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
               
                if (string.IsNullOrWhiteSpace(Name.Text) || string.IsNullOrWhiteSpace(Description.Text))
                {

                    await new ContentDialog
                    {
                        Title = "Error",
                        Content = "Please fill in all fields.",
                        CloseButtonText = "OK"
                    }.ShowAsync();
                    return;
                }

                await WorkoutPlanViewModel.CreateWorkoutPlanAsync(Name.Text, Description.Text);

                await new ContentDialog
                {
                    Title = "Success",
                    Content = $"Workout template '{Name.Text}' has been created.",
                    CloseButtonText = "OK"
                }.ShowAsync();

                WorkoutPlanViewModel.Placeholder = "";
            }
    
        }
    }
}
