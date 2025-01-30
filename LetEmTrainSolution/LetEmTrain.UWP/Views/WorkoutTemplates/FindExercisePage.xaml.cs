using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using LetEmTrain.UWP.ViewModels;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LetEmTrain.Infrastructure;
using System.Threading.Tasks;
using LetEmTrain.Domain.Models;
using System.Collections.Generic;
using LetEmTrain.UWP.Utilities;
using System.Diagnostics;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace LetEmTrain.UWP.Views.WorkoutTemplates
{
    public sealed partial class FindExercisePage : Page
    {

        public ExerciseViewModel ExerciseViewModel { get; set; }

        public FindExercisePage()
        {
            this.InitializeComponent();
            ExerciseViewModel = new ExerciseViewModel();
            this.DataContext = ExerciseViewModel;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ExerciseViewModel.LoadAllAsync();
            base.OnNavigatedTo(e);
        }
           
        // Collection to represent the user's workout plan
        //private static ObservableCollection<LetEmTrain.UWP.ViewModels.ExerciseViewModel> WorkoutPlan = new ObservableCollection<LetEmTrain.UWP.ViewModels.ExerciseViewModel>();


        // Filter exercises by selected muscle group
        private async void FilterByGroup_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var mainBodyPart = button?.Content?.ToString();

            if (!string.IsNullOrEmpty(mainBodyPart))
            {
                await ExerciseViewModel.FilterExercisesByMuscleGroupAsync(mainBodyPart);
            }
        }

        // View detailed information about an exercise
        private async void SeeExercise_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var exerciseName = button?.Tag?.ToString();
            var selectedExercise = ExerciseViewModel.Exercises.FirstOrDefault(ex => ex.Name == exerciseName);

                // Create a dialog to display exercise details
                var exerciseDialog = new ContentDialog
                {

                    Title = selectedExercise.Name,
                    Content = new StackPanel
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Image
                            {
                                Height = 200,
                                Stretch = Stretch.Uniform,
                                Margin = new Thickness(0, 10, 0, 0),
                                Source = selectedExercise.Image != null && selectedExercise.Image.Length > 0
                                    ? (ImageSource)new BytesToImageConverter().Convert(selectedExercise.Image, typeof(ImageSource), null, string.Empty)
                                    : new BitmapImage(new Uri("ms-appx:///Assets/Placeholder.png"))
                            },
                            new TextBlock
                            {
                                 Text = "Main Body Part: ",
                                 FontWeight = Windows.UI.Text.FontWeights.Bold,
                                 Margin = new Thickness(0, 10, 0, 0)
                            },
                            new TextBlock
                            {
                                 Text = selectedExercise.MainBodyPart,
                                 TextWrapping = TextWrapping.Wrap,
                                 Margin = new Thickness(0, 5, 0, 10)
                            },
                            new TextBlock
                            {
                                 Text = "Description",
                                 FontWeight = Windows.UI.Text.FontWeights.Bold,
                                 Margin = new Thickness(0, 10, 0, 0)
                            },
                            new TextBlock
                            {
                                 Text = selectedExercise.Description,
                                 TextWrapping = TextWrapping.Wrap,
                                 Margin = new Thickness(0, 5, 0, 10)
                            },
                        }
                    },
                    CloseButtonText = "Close"
                };

                await exerciseDialog.ShowAsync();
        }

        private async void asbExercises_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                await ExerciseViewModel.SearchExercisesAsync(sender.Text);
            }
        }

        private async void asbExercises_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await ExerciseViewModel.SearchExercisesAsync(args.QueryText);
        }

        private void asbExercises_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem as string;
        }

    }
}
