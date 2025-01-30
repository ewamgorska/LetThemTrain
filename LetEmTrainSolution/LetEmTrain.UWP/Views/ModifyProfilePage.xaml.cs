using LetEmTrain.UWP.ViewModels;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LetEmTrain.UWP.Views
{
    public sealed partial class ModifyProfilePage : Page
    {
        public UserViewModel UserViewModel { get; set; }
        public ModifyProfilePage()
        {
            this.InitializeComponent();
            this.UserViewModel = App.UserViewModel;
            UserViewModel.DietTypeComboBox = cmbDiettype;
            UserViewModel.ActivityLevelComboBox = cmbActivityLevel;
            UserViewModel.FitnessGoalsComboBox = cmbFitnessgoals;

        }

        private async void btnSaveChanges_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var userViewModel = App.UserViewModel;

            // Save the changes
            btnSaveChanges.IsEnabled = false;
            progressRing.IsActive = true;

            await userViewModel.SaveProfileChangesAsync();

            progressRing.IsActive = false;
            btnSaveChanges.IsEnabled = true;

            Frame.GoBack();
        }

        private void InitializeDropdowns()
        {
            // Log current values in the ViewModel
            System.Diagnostics.Debug.WriteLine($"[InitializeDropdowns] DietType in ViewModel: {UserViewModel.DietType}");
            System.Diagnostics.Debug.WriteLine($"[InitializeDropdowns] ActivityLevel in ViewModel: {UserViewModel.ActivityLvl}");
            System.Diagnostics.Debug.WriteLine($"[InitializeDropdowns] FitnessGoals in ViewModel: {UserViewModel.FitnessGoals}");

            // Pre-select DietType
            cmbDiettype.SelectedItem = cmbDiettype.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == UserViewModel.DietType);

            // Pre-select ActivityLevel
            cmbActivityLevel.SelectedItem = cmbActivityLevel.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString().StartsWith(UserViewModel.ActivityLvl.ToString()));

            // Pre-select FitnessGoals
            cmbFitnessgoals.SelectedItem = cmbFitnessgoals.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString().StartsWith(UserViewModel.FitnessGoals.ToString()));

            // Log the selected items
            System.Diagnostics.Debug.WriteLine($"[InitializeDropdowns] Selected DietType: {(cmbDiettype.SelectedItem as ComboBoxItem)?.Content}");
            System.Diagnostics.Debug.WriteLine($"[InitializeDropdowns] Selected ActivityLevel: {(cmbActivityLevel.SelectedItem as ComboBoxItem)?.Content}");
            System.Diagnostics.Debug.WriteLine($"[InitializeDropdowns] Selected FitnessGoals: {(cmbFitnessgoals.SelectedItem as ComboBoxItem)?.Content}");

        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Ensure the ViewModel properties are up-to-date
            UserViewModel.DietType = UserViewModel.User.DietType;
            UserViewModel.ActivityLvl = UserViewModel.User.ActivityLevel;
            UserViewModel.FitnessGoals = UserViewModel.User.FitnessGoals;

            // Initialize dropdowns with updated values
            InitializeDropdowns();
        }


    }
}
