using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DCCMobileController.Services;
using DCCMobileController.Views;

namespace DCCMobileController
{
    using DCCMobileController.ViewModels;

    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new MainPage() { BindingContext = new DccControllerBaseViewModel() };

            //DccControllerBaseViewModel model = new DccControllerBaseViewModel();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            MessagingCenter.Send<App>(this, "Sleep"); // When app sleep, send a message so I can "Cancel" the connection
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            MessagingCenter.Send<App>(this, "Resume"); // When app resume, send a message so I can "Resume" the connection
        }
    }
}
