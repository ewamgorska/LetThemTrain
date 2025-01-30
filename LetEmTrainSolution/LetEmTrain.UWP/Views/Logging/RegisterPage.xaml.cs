using LetEmTrain.Infrastructure;
using LetEmTrain.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



namespace LetEmTrain.UWP.Views.Logging
{

    public sealed partial class RegisterPage : Page
    {

        public UserViewModel UserViewModel { get; set; }
        public RegisterPage()
        {
            this.InitializeComponent();
            this.UserViewModel = App.UserViewModel;
        }

        private async void btnCreateAccount_Click(object sender, RoutedEventArgs e)
        {

            string gender = (cmbGender.SelectedItem as ComboBoxItem)?.Content.ToString();
            string dietType = (cmbDiettype.SelectedItem as ComboBoxItem)?.Content.ToString();


            UserViewModel.Gender = !string.IsNullOrEmpty(gender) ? gender[0] : 'N';
            UserViewModel.ActivityLvl = cmbActivityLevel.SelectedIndex + 1;
            UserViewModel.FitnessGoals = cmbFitnessgoals.SelectedIndex + 1;
            UserViewModel.DietType = dietType;

            btnCreateAccount.IsEnabled = false;
            progressRing.IsActive = true;
            btnCreateAccount.Visibility = Visibility.Collapsed;
            progressRing.Visibility = Visibility.Visible;


            try
            {
                bool isAccountCreated = await UserViewModel.CreateAccountAsync();
                if (isAccountCreated)
                    Frame.Navigate(typeof(MainPage));

            }
            finally
            {
                btnCreateAccount.IsEnabled = true;
                progressRing.IsActive = false;
                btnCreateAccount.Visibility = Visibility.Visible;
                progressRing.Visibility = Visibility.Collapsed;
                   

            }

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LoginPage));
        }

    }
}

