using LetEmTrain.Domain.Models;
using LetEmTrain.UWP.ViewModels;
using System;
using System.Collections.Generic;
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

namespace LetEmTrain.UWP.Views.ProgressPages
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class LiftCalculatorPage : Page
    {
        private ProgressViewModel ProgressViewModel;
        public LiftCalculatorPage()
        {
            this.InitializeComponent();
            this.ProgressViewModel = App.ProgressViewModel;
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }


        private async void calculate_ClickAsync(object sender, RoutedEventArgs e)
        {

            float BpMax = 0, SquatMax = 0, DlMax = 0, currentWeight = 0;
            int BpReps = 0, SquatReps = 0, DlReps = 0;

            bool isBpValid = float.TryParse(bp.Text, out BpMax);
            bool isSquatValid = float.TryParse(squat.Text, out SquatMax);
            bool isDlValid = float.TryParse(dl.Text, out DlMax);
            bool isWeightValid = float.TryParse(weight.Text, out currentWeight);

            bool isBpRepsValid = int.TryParse(bpReps.Text, out BpReps);
            bool isSquatRepsValid = int.TryParse(squatReps.Text, out SquatReps);
            bool isDlRepsValid = int.TryParse(dlReps.Text, out DlReps);

            if (BpMax >= 1000 || BpMax < 0 || SquatMax >= 1000 || SquatMax < 0 || DlMax >= 1000 || DlMax < 0)
            {
                await ShowContentDialogAsync("Invalid input", "Weight needs to be in range (1-999)");
                return;
            }

            if (BpReps >= 100 || BpReps < 0 || SquatReps >= 100 || SquatReps < 0 || DlReps >= 100 || DlReps < 0)
            {
                await ShowContentDialogAsync("Invalid input", "Number of reps needs to be in range (1-99)");
                return;
            }

            if ((BpMax > 0 && !isBpRepsValid) || (SquatMax > 0 && !isSquatRepsValid) || (DlMax > 0 && !isDlRepsValid))
            {
                await ShowContentDialogAsync("Invalid input", "Please enter valid numbers for reps.");
                return;
            }

            if ((BpReps > 0 && BpMax == 0) || (SquatReps > 0 && SquatMax == 0) || (DlReps > 0 && DlMax == 0))
            {
                await ShowContentDialogAsync("Incomplete input", "If you enter reps, you must also enter the weight for the lift.");
                return;
            }

            if (!isWeightValid || currentWeight <= 0)
            {
                await ShowContentDialogAsync("Wrong weight", "You need to add a valid weight");
                return;
            }

            if(BpMax == 0 && SquatMax == 0 &&  DlMax == 0)
            {
                await ShowContentDialogAsync("Wrong weight", "You need to enter at least one lift");
                return;
            }

            float maxBench = BpMax > 0 && isBpValid ? ProgressViewModel.CalculateOneRepMax(BpMax, BpReps, currentWeight, 0) : 0;
            float maxSquat = SquatMax > 0 && isSquatValid ? ProgressViewModel.CalculateOneRepMax(SquatMax, SquatReps, currentWeight, 1) : 0;
            float maxDeadlift = DlMax > 0 && isDlValid ? ProgressViewModel.CalculateOneRepMax(DlMax, DlReps, currentWeight, 2) : 0;

            if (maxBench > 0)
                MaxBenchFormatted.Text = $"{maxBench:F2} kg  ({(maxBench) / currentWeight:F2} times your bodyweight)";
            else 
                MaxBenchFormatted.Text = "";

            if (maxSquat > 0)
                MaxSquatFormatted.Text = $"{maxSquat:F2} kg  ({(maxSquat) / currentWeight:F2} times your bodyweight)";
            else 
                MaxSquatFormatted.Text = "";

            if (maxDeadlift > 0)
                MaxDeadliftFormatted.Text = $"{maxDeadlift:F2} kg  ({(maxDeadlift) / currentWeight:F2} times your bodyweight)";
            else 
                MaxDeadliftFormatted.Text = "";

            if (SaveToggle.IsOn)
            {
                Progress progress = await ProgressViewModel.AddProgress(maxBench, maxSquat, maxDeadlift, currentWeight);
                if (progress != null)
                {
                    await ShowContentDialogAsync("Progress added successfully!", "Nice maxes :)");
                }
            }
        }


        private void clear_Click(object sender, RoutedEventArgs e)
        {
            bp.Text = string.Empty;
            bpReps.Text = string.Empty;
            squat.Text = string.Empty;
            squatReps.Text = string.Empty;
            dl.Text = string.Empty;
            dlReps.Text = string.Empty;
            weight.Text = string.Empty;
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

        private void SaveToggle_Toggled(object sender, RoutedEventArgs e)
        {

        }
    }
}
