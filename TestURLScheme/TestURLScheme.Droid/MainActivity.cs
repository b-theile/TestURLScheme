using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Android.Content;
using Android.Net;

namespace TestURLScheme.Droid
{
    [Activity(Label = "TestURLScheme", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode =LaunchMode.SingleTop)]
    [IntentFilter(new[] { Android.Content.Intent.ActionView },
        DataScheme = "mru4uresponse",
        Categories = new[] { Android.Content.Intent.CategoryDefault, Android.Content.Intent.CategoryBrowsable })]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            if (Intent.Data != null)
            {
                MessagingCenter.Send<string>(Intent.Data.EncodedPath, "mru4uresponse");
            }

            MessagingCenter.Subscribe<string>(this, "mru4urequest", (urlString) => 
            {
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(urlString));
                //Intent intent = new Intent(Intent.ActionSend, Android.Net.Uri.Parse(urlString));
                //Forms.Context.StartActivity(Intent.CreateChooser(intent, "Messungen bereitstellen"));
                Forms.Context.StartActivity(intent);
                this.Finish();
            });
        }
    }
}
