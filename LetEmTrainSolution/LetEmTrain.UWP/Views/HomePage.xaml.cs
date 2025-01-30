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

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace LetEmTrain.UWP.Views
{
    public sealed partial class HomePage : Page
    {
        public UserViewModel UserViewModel { get; set; }
        public ProgressViewModel ProgressViewModel { get; set; }
        public string WelcomeMessage => $"WHAT DO WE HIT TODAY, \n{UserViewModel.LoggedUser.Username}?";
        public HomePage()
        {
            this.InitializeComponent();
            this.UserViewModel = App.UserViewModel;
            this.ProgressViewModel = App.ProgressViewModel;
            LoadData();
            
        }
        private async void LoadData()
        {
            await ProgressViewModel.LoadProgressesAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ProgressViewModel.RecalculateBodyScore();
        }
    }
}
