using System;
namespace TestURLScheme.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            LoadApplication(new TestURLScheme.App());

            Xamarin.Forms.MessagingCenter.Subscribe<string>(this, "mru4urequest", async (urlString) =>
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri($"mru4urequest:?data={urlString}"));
            });
        }
    }
}
