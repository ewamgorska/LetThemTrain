using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LetEmTrain.UWP.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace LetEmTrain.UWP.Views.Admin
{
    public sealed partial class ManageUsersPage : Page
    {
        public UserViewModel UserViewModel { get; set; }

        public ManageUsersPage()
        {
            this.InitializeComponent();
            UserViewModel = App.UserViewModel;
            this.DataContext = UserViewModel;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await UserViewModel.LoadAllUsersAsync();
            base.OnNavigatedTo(e);
        }

        private async void asbUsers_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                await UserViewModel.SearchUsersAsync(sender.Text);
            }
        }

        private async void asbUsers_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await UserViewModel.SearchUsersAsync(args.QueryText);
        }

        private void asbUsers_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = args.SelectedItem as string;
        }

        private async void ActivateUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int userId)
            {
                await UserViewModel.ToggleUserStatusAsync(userId, true);
            }
        }

        private async void DeactivateUser_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is int userId)
            {
                await UserViewModel.ToggleUserStatusAsync(userId, false);
            }
        }
    }
}
