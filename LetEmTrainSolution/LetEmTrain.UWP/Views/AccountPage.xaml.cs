using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using LetEmTrain.Infrastructure;
using System.IO;
using LetEmTrain.UWP.ViewModels;

namespace LetEmTrain.UWP.Views
{
    public sealed partial class AccountPage : Page
    {
        public UserViewModel UserViewModel { get; set; }
   
        public ProgressViewModel ProgressViewModel { get; set; }
        
        public AccountPage()
        {
            this.InitializeComponent();
            this.DataContext = App.UserViewModel;
            UserViewModel = App.UserViewModel;
            ProgressViewModel = App.ProgressViewModel;  
                 
        }

        private async void ProfilePicture_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Create a FileOpenPicker to select an image
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.ViewMode = PickerViewMode.Thumbnail;

            picker.FileTypeFilter.Clear();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            // Open the picker
            StorageFile sourceFile = await picker.PickSingleFileAsync();
            if (sourceFile != null)
            {
                {
                    using (Stream stream = await sourceFile.OpenStreamForReadAsync())
                    {
                        byte[] bytes = new byte[stream.Length];
                        await stream.ReadAsync(bytes, 0, bytes.Length);
                        UserViewModel.ProfilePicture = bytes;
                    }
                }

                await UserViewModel.SaveProfilePictureAsync();
            }
        }






        private void ModifyProfile_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ModifyProfilePage));
        }

        private void nvLogout_Tapped(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WelcomePage));
            Frame.BackStack.Clear(); // Clear navigation stack
        }

        private void nvMain_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItemContainer is NavigationViewItem item)
            {
                switch (item.Name)
                {
                    case "nvMainPage":
                        Frame.Navigate(typeof(MainPage));
                        break;

                    case "nvWorkoutPlans":
                        //Frame.Navigate(typeof(WorkoutPlansPage));
                        break;

                    case "nvChart":
                        // Frame.Navigate(typeof(ProgressChartPage));
                        break;

                    case "nvMaxes":
                        //Frame.Navigate(typeof(MaxLiftsPage));
                        break;

                    case "nvMaxCalc":
                        //Frame.Navigate(typeof(OneRepMaxPage));
                        break;

                    case "nvCalorieCalc":
                        //Frame.Navigate(typeof(DietManagerPage));
                        break;

                    case "nvAccount":
                        // Already on AccountPage; do nothing
                        break;

                    case "nvLogout":
                        nvLogout_Tapped(sender, null);
                        break;

                    default:
                        break;
                }
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ProgressViewModel.LoadLastProgressAsync();
            ReloadUserData();
        }

        private async void ReloadUserData()
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    var user = await uow.UserRepository.FindByEmailAsync(App.UserViewModel.User.Email);
                    if (user != null)
                    {
                        App.UserViewModel.User = user;

                //        // Convert the relative path to URI
                //        if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                //        {
                //            if (!user.ProfilePicturePath.StartsWith("ms-appdata:///"))
                //            {
                //                App.UserViewModel.User.ProfilePicturePath = $"ms-appdata:///local/{user.ProfilePicturePath}";
                //            }
                //            else
                //            {
                //                App.UserViewModel.User.ProfilePicturePath = user.ProfilePicturePath;
                //            }
                //        }

                //        System.Diagnostics.Debug.WriteLine($"[ReloadUserData] Loaded ProfilePicturePath: {App.UserViewModel.User.ProfilePicturePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ReloadUserData] Error: {ex.Message}");
            }
        }


    }
}
