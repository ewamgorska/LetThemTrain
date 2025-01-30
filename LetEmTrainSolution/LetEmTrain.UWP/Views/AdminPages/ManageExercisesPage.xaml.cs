using LetEmTrain.Domain.Models;
using LetEmTrain.UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI.Xaml.Input;

namespace LetEmTrain.UWP.Views
{
    public sealed partial class ManageExercisesPage : Page
    {
        public AdminViewModel AdminViewModel { get; set; }
        public ExerciseViewModel ExerciseViewModel { get; set; }    
        public ManageExercisesPage()
        {
            this.InitializeComponent();
            AdminViewModel = App.AdminViewModel;
            this.DataContext = AdminViewModel;
            ExerciseViewModel = App.ExerciseViewModel;

            // Ensure exercises are loaded when the page is loaded
            _ = AdminViewModel.LoadExercisesAsync();  // Call the method directly in the constructor
        }

        private async void ManageExercisesPage_Loaded(object sender, RoutedEventArgs e)
        {
            await AdminViewModel.LoadExercisesAsync();  // Load exercises when the page is loaded
        }

        private async void btnUpload_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.ViewMode = PickerViewMode.Thumbnail;

            openPicker.FileTypeFilter.Clear();
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".jpg");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    byte[] bytes = new byte[stream.Length];
                    await stream.ReadAsync(bytes, 0, bytes.Length);
                    ExerciseViewModel.Image = bytes;
                }
            }
        }
        // Add a new exercise when the button is clicked
        private async void AddExercise_Click(object sender, RoutedEventArgs e)
        {
            // Open a dialog or navigate to the AddExercisePage for adding new exercise
            var addDialog = new ContentDialog
            {
                Title = "Add New Exercise",
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Cancel"
            };
            var stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Spacing = 10
                    };

                    var nameTextBox = new TextBox { PlaceholderText = "Enter Exercise Name" };
                    var descriptionTextBox = new TextBox { PlaceholderText = "Enter Exercise Description", AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
                    var uploadImageButton = new Button
                    { 
                        Content = "Choose Image...",
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    uploadImageButton.Tapped += btnUpload_Tapped;
                    
                    var mainBodyPartComboBox = new ComboBox
                    { 
                        PlaceholderText = "Select Main Body Part",
                        ItemsSource = new ObservableCollection<string>
                        {
                            "Chest",
                            "Back",
                            "Arms",
                            "Legs",
                            "Core",
                            "Glutes"
                        }
                    };
                    stackPanel.Children.Add(uploadImageButton);
                    stackPanel.Children.Add(mainBodyPartComboBox);
                    stackPanel.Children.Add(nameTextBox);
                    stackPanel.Children.Add(descriptionTextBox);

                    addDialog.Content = stackPanel;

            addDialog.PrimaryButtonClick += async (s, args) =>
            {
                string mainBodyPart = mainBodyPartComboBox.SelectedItem as string;

                if (!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrEmpty(descriptionTextBox.Text) && !string.IsNullOrEmpty(mainBodyPart))
                {
                    string exerciseName = nameTextBox.Text;
                    string exerciseDescription = descriptionTextBox.Text;

                    await AdminViewModel.AddExerciseAsync(exerciseName, exerciseDescription, mainBodyPart, ExerciseViewModel.Image);
                    await AdminViewModel.LoadExercisesAsync();
                }
            };

            await addDialog.ShowAsync();
        }

        private async void EditExercise_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var exerciseId = button?.Tag.ToString(); // Exercise ID

            if (!string.IsNullOrEmpty(exerciseId))
            {
                var exerciseToEdit = AdminViewModel.Exercises.FirstOrDefault(ex => ex.Id == int.Parse(exerciseId));
                if (exerciseToEdit != null)
                {
                    var editDialog = new ContentDialog
                    {
                        Title = "Edit Exercise",
                        PrimaryButtonText = "Save",
                        SecondaryButtonText = "Cancel"
                    };

                    var stackPanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Spacing = 10
                    };

                    var nameTextBox = new TextBox { Text = exerciseToEdit.Name, PlaceholderText = "Enter Exercise Name" };
                    var descriptionTextBox = new TextBox { Text = exerciseToEdit.Description, PlaceholderText = "Enter Exercise Description", AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
                    var uploadImageButton = new Button
                    {
                        Content = "Choose Image...",
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    uploadImageButton.Tapped += btnUpload_Tapped;

                    var mainBodyPartComboBox = new ComboBox
                    {
                        PlaceholderText = "Select Main Body Part",
                        ItemsSource = new ObservableCollection<string>
                        {
                            "Chest",
                            "Back",
                            "Arms",
                            "Legs",
                            "Core",
                            "Glutes"
                        }
                    };

                    if (!string.IsNullOrEmpty(exerciseToEdit.MainBodyPart))
                    {
                        mainBodyPartComboBox.SelectedItem = exerciseToEdit.MainBodyPart;
                    }

                    stackPanel.Children.Add(uploadImageButton);
                    stackPanel.Children.Add(mainBodyPartComboBox);
                    stackPanel.Children.Add(nameTextBox);
                    stackPanel.Children.Add(descriptionTextBox);

                    editDialog.Content = stackPanel;

                    editDialog.PrimaryButtonClick += async (s, args) =>
                    {
                        string mainBodyPart = mainBodyPartComboBox.SelectedItem as string;
                        // Verifique se o nome e a descrição não estão vazios
                        if (!string.IsNullOrEmpty(nameTextBox.Text) && !string.IsNullOrEmpty(descriptionTextBox.Text))
                        {
                            // Atualize o exercício com os novos dados
                            exerciseToEdit.Name = nameTextBox.Text;
                            exerciseToEdit.Description = descriptionTextBox.Text;
                            exerciseToEdit.MainBodyPart = mainBodyPart;
                            exerciseToEdit.Image = ExerciseViewModel.Image;

                            // Salve as alterações
                            await AdminViewModel.SaveExerciseAsync(exerciseToEdit);

                            // Recarregue os exercícios após a atualização
                            await AdminViewModel.LoadExercisesAsync();
                        }
                        else
                        {
                            // Mostre um erro caso algum campo esteja vazio
                            await ShowContentDialogAsync("Error", "Please fill in all fields.");
                        }
                    };


                    await editDialog.ShowAsync();
                }
            }
        }

        private async void DeleteExercise_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var exerciseId = button?.Tag.ToString();

            if (!string.IsNullOrEmpty(exerciseId))
            {
                var exerciseToDelete = AdminViewModel.Exercises.FirstOrDefault(ex => ex.Id == int.Parse(exerciseId));
                if (exerciseToDelete != null)
                {
                    var deleteDialog = new ContentDialog
                    {
                        Title = "Delete Exercise",
                        Content = $"Are you sure you want to delete {exerciseToDelete.Name}?",
                        PrimaryButtonText = "Yes",
                        SecondaryButtonText = "No"
                    };

                    deleteDialog.PrimaryButtonClick += async (s, args) =>
                    {
                        await AdminViewModel.DeleteExerciseAsync(exerciseToDelete);
                        await AdminViewModel.LoadExercisesAsync();
                    };

                    await deleteDialog.ShowAsync();
                }
            }
        }

        // The missing method to show content dialog
        private async Task ShowContentDialogAsync(string title, string content)
        {
            await new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK"
            }.ShowAsync();
        }

        private async void asbExercises_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                await AdminViewModel.SearchExercisesAsync(sender.Text);
            }
            
        }

        private async void asbExercises_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await AdminViewModel.SearchExercisesAsync(args.QueryText);
        }

        private void asbExercises_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem as string;
        }
    }
}
