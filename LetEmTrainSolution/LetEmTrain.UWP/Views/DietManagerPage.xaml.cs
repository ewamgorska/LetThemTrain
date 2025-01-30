using LetEmTrain.Domain.Models;
using LetEmTrain.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace LetEmTrain.UWP.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class DietManagerPage : Page
    {
        public int goal = 0;
        public string dietType = null;
        public UserViewModel UserViewModel { get; set; }
        public ProgressViewModel ProgressViewModel { get; set; }

        public DietManagerPage()
        { 
            this.InitializeComponent();
            this.UserViewModel = App.UserViewModel;
            this.ProgressViewModel = App.ProgressViewModel;
        }

        private async void calculate_Click(object sender, RoutedEventArgs e)
        {

            int height = int.TryParse(heighttxt.Text, out var h) ? h : 0;
            float weight = float.TryParse(weighttxt.Text, out var w) ? w : 0;
            int age = int.TryParse(agetxt.Text, out int a) ? a : 0;
            int activityLvl = activitylvl.SelectedIndex + 1;
           

            if (age <= 0 || weight <= 0 || height <= 0)
            {
                await ShowContentDialogAsync("Invalid inputs", "Please enter valid numbers for height, weight and age");
                return;

            }

            if(activityLvl == 0)
            {
                await ShowContentDialogAsync("Activity level not selected", "Please select your activity level");
                return;
            }

            if (goal == 0)
            {
                await ShowContentDialogAsync("Goal not selected", "Please select your fitness goal");
                return;
            }

            if (string.IsNullOrEmpty(dietType))
            {
                await ShowContentDialogAsync("Diet Type not selected", "Please select your preferred diet type");
                return;
            }
            calories.Text = Utilities.CalorieCalc.CalculateCalories(age, weight, height, UserViewModel.LoggedUser.Gender, activityLvl, goal, dietType);   

        }

        private void import_Click(object sender, RoutedEventArgs e)
        {
            agetxt.Text = UserViewModel.LoggedUser.Age.ToString();
            heighttxt.Text = UserViewModel.LoggedUser.Height.ToString();
            activitylvl.SelectedIndex = UserViewModel.LoggedUser.ActivityLevel - 1;

            var goalRadioButton = FindName($"goal{UserViewModel.LoggedUser.FitnessGoals}") as RadioButton;
                if (goalRadioButton != null)
                {
                    goalRadioButton.IsChecked = true;
                }

                var dietRadioButton = FindName(UserViewModel.LoggedUser.DietType) as RadioButton;
                if (dietRadioButton != null)
                {
                    dietRadioButton.IsChecked = true;
                }
                else
                {
                    dietRadioButton = FindName("highProtein") as RadioButton;
                    if (dietRadioButton != null) dietRadioButton.IsChecked = true;
                }
            var lastProgress = ProgressViewModel.Progresses?.OrderByDescending(p => p.Date).FirstOrDefault();
            if (lastProgress != null)
            {

                weighttxt.Text = lastProgress.Weight.ToString("F2");
            }
            else
            {
                weighttxt.Text = "0";
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

        private void RadioButtongoal_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                if (int.TryParse(radioButton.Tag.ToString(), out int value))
                {
                    goal = value;
                }
            }
        }

        private void RadioButtondiet_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                dietType = radioButton.Tag.ToString();
            }
        }
    }
}
