using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LetEmTrain.UWP.Views;
using LetEmTrain.UWP.ViewModels;

namespace LetEmTrain.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static AdminViewModel AdminViewModel { get; set; }
        public static UserViewModel UserViewModel { get; set; }
        public static ProgressViewModel ProgressViewModel { get; set; }
        public static WorkoutPlanViewModel WorkoutPlanViewModel { get; set; }
        public static ExerciseViewModel ExerciseViewModel { get; set; }
        public static ExerciseSetViewModel ExerciseSetViewModel { get; set; }
        /// <summary>
        /// Initializes the application object. This is the first line of code that is executed
        /// and is the logical equivalent of the main() or WinMain() methods.
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            UserViewModel = new UserViewModel();
            ProgressViewModel = new ProgressViewModel();
            AdminViewModel = new AdminViewModel();
            WorkoutPlanViewModel = new WorkoutPlanViewModel();
            ExerciseViewModel = new ExerciseViewModel();
            ExerciseSetViewModel = new ExerciseSetViewModel();
        }

        /// <summary>
        /// Called when the application is launched normally by the end user. Other entry points
        /// will be used when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a frame that will act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from the previously suspended application
                }

                // Place the frame in the current window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored, navigate to the first page,
                    // configuring a new page by passing the required information as a parameter
                    //rootFrame.Navigate(typeof(LetEmTrain.UWP.Views.Logging.LoginPage), e.Arguments);
                    rootFrame.Navigate(typeof(WelcomePage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Called when navigation to a specific page fails
        /// </summary>
        /// <param name="sender">The frame where navigation failed</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Called when the application is suspended. Application state is saved
        /// without knowing whether the application will be terminated or resumed with unchanged memory content.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activities
            deferral.Complete();
        }
    }
}
