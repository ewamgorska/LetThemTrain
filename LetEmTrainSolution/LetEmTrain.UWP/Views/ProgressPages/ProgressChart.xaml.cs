using LetEmTrain.Domain.Models;
using LetEmTrain.UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Telerik.Charting;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LetEmTrain.UWP.Views.ProgressPages
{
    public sealed partial class ProgressChart : Page
    {
        public ProgressViewModel ProgressViewModel { get; set; }
        public ProgressChart()
        {
            this.InitializeComponent();
            ProgressViewModel = App.ProgressViewModel;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ProgressViewModel.LoadProgressesAsync();
        }


        private async void DeleteProgress_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var progressToDelete = button.DataContext as Progress;
            if (progressToDelete != null)
            {
                var dialog = new ContentDialog
                {
                    Title = "Confirm Deletion",
                    Content = "Are you sure you want to delete this progress?\nYou won't be able to bring it back",
                    PrimaryButtonText = "Yes",
                    SecondaryButtonText = "No"
                };
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    await ProgressViewModel.DeleteProgressAsync(progressToDelete);
                }
            }
        }

        private void CalendarView_StartDateChanged(object sender, CalendarViewSelectedDatesChangedEventArgs e)
        {
            if (e.AddedDates.Count > 0)
            {
                ProgressViewModel.StartDate = e.AddedDates[0].Date.AddDays(-1);
            }
            else
            {
                ProgressViewModel.StartDate = DateTime.MinValue;
            }
        }

        private void CalendarView_EndDateChanged(object sender, CalendarViewSelectedDatesChangedEventArgs e)
        {
            if (e.AddedDates.Count > 0)
            {
                ProgressViewModel.EndDate = e.AddedDates[0].Date.AddDays(1);
            }
            else
            {
                ProgressViewModel.EndDate = DateTime.MaxValue;
            }
        }
    }
}
