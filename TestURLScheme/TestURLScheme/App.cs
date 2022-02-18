using TestURLScheme.Views;
using Xamarin.Forms;

namespace TestURLScheme
{
    public class App : Application
    {
        public App()
        {
            MainPage = new NavigationPage(new SingleStartPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
