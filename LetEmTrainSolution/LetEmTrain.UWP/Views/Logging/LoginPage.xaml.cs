using LetEmTrain.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LetEmTrain.UWP.Views.Logging
{
    public sealed partial class LoginPage : Page
    {
        public AdminViewModel AdminViewModel { get; set; }
        public UserViewModel UserViewModel { get; set; }
        private bool IsUserSelected = true;
        private bool IsAdminSelected = false;
        public LoginPage()
        {
            this.InitializeComponent();
            this.UserViewModel = App.UserViewModel;
            this.AdminViewModel = App.AdminViewModel;

        }

        private void OnUserChecked(object sender, RoutedEventArgs e)
        {
            txtEmailUser.Visibility = Visibility.Visible;
            PasswordBoxUser.Visibility = Visibility.Visible;
            IsUserSelected = true;
            IsAdminSelected = false;
            txtEmailAdmin.Visibility = Visibility.Collapsed;
            PasswordBoxAdmin.Visibility = Visibility.Collapsed;
        }


        private void OnAdminChecked(object sender, RoutedEventArgs e)
        {
            txtEmailUser.Visibility = Visibility.Collapsed;
            PasswordBoxUser.Visibility = Visibility.Collapsed;
            IsUserSelected = false;
            IsAdminSelected = true;
            txtEmailAdmin.Visibility = Visibility.Visible;
            PasswordBoxAdmin.Visibility = Visibility.Visible;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            btnLogin.IsEnabled = false;
            progressRing.IsActive = true;
            btnLogin.Visibility = Visibility.Collapsed;
            progressRing.Visibility = Visibility.Visible;

            try
            {
                bool isLoggedIn = false;
                if (IsUserSelected)
                {
                    isLoggedIn = await UserViewModel.LoginAsync();
                }
                else
                {
                    isLoggedIn = await AdminViewModel.LoginAsync();
                }

                if (isLoggedIn)
                {
                    Frame.Navigate(typeof(MainPage));
                }
            }
            finally
            {
                btnLogin.IsEnabled = true;
                progressRing.IsActive = false;
                btnLogin.Visibility = Visibility.Visible;
                progressRing.Visibility = Visibility.Collapsed;
            }

        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                
                btnLogin_Click(btnLogin, null);
            }
        }
    }
}
