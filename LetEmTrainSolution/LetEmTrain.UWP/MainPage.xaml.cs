using LetEmTrain.Domain.Models;
using LetEmTrain.UWP.ViewModels;
using LetEmTrain.UWP.Views;
using LetEmTrain.UWP.Views.Admin;
using LetEmTrain.UWP.Views.ProgressPages;
using LetEmTrain.UWP.Views.WorkoutTemplates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace LetEmTrain.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public AdminViewModel AdminViewModel { get; set; }
        public UserViewModel UserViewModel { get; set; }
        public string nvSectionTitle;
        private NavigationViewItem currentPage = null;
        bool isAdmin = false;
        bool isUser = false;
        public MainPage()
        {
            this.InitializeComponent();
            this.AdminViewModel = App.AdminViewModel;
            this.UserViewModel = App.UserViewModel;
            if (this.AdminViewModel.LoggedAdmin != null)
            {
                nvSectionTitle = "Admin Panel";
                frmMain.Navigate(typeof(AdminHomePage));
                isAdmin = true;
            }
            else
            {
                nvSectionTitle = "Workout Plans";
                isUser = true;
                frmMain.Navigate(typeof(HomePage));
            }

        }

        private void nvMain_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            
            var selectedItem = args.InvokedItemContainer as NavigationViewItem;
            if ((selectedItem != null && currentPage != selectedItem))
            {
                switch (selectedItem.Tag)
                {
                    case "home":
                        if(isUser)
                            frmMain.Navigate(typeof (HomePage));
                        else frmMain.Navigate(typeof (AdminHomePage));
                        break;
                    case "acc":
                        frmMain.Navigate(typeof(AccountPage));
                        break;
                    case "chart":
                        frmMain.Navigate(typeof(ProgressChart));
                        break;
                    case "showTemplate":
                        frmMain.Navigate(typeof(MyTemplatesPage));
                        break;
                    case "findExercise":
                        frmMain.Navigate(typeof(FindExercisePage));
                        break;

                    case "liftCalc":
                        frmMain.Navigate(typeof(LiftCalculatorPage));
                        break;
                    case "kcalCalc":
                        frmMain.Navigate(typeof(DietManagerPage));
                        break;
                    case "mngUsers":
                        frmMain.Navigate(typeof(ManageUsersPage));
                        break;
                    case "mngExercises":
                        frmMain.Navigate(typeof(ManageExercisesPage));
                        break;

                }
                currentPage = selectedItem;
                nvMain.IsBackEnabled = true;
     
            }
        }

        private void nvMain_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (isUser)
            {
                frmMain.Navigate(typeof(HomePage));
            }
            else frmMain.Navigate(typeof(AdminHomePage));
            nvMain.IsBackEnabled = false;
            currentPage = null;
        
        }

        private void nvLogout_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(this.AdminViewModel.LoggedAdmin != null )
                AdminViewModel.Logout();
            else UserViewModel.Logout();
            isAdmin = false;
            isUser = false;
            Frame.Navigate(typeof(WelcomePage));
        }
    }
}
