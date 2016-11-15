using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace TestURLScheme.Droid
{
    [Activity(Label = "TestURLScheme", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, LaunchMode =LaunchMode.SingleTask)]

    [IntentFilter( new[] { Intent.ActionView },
        DataScheme = "mru4uresponse",
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]

    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            LoadApplication(new App());

            if (Intent.Data != null)
            {
                MessagingCenter.Send<string>(Intent.Data.EncodedPath, "mru4uresponse");
            }
        }

        protected override void OnNewIntent(Intent intent)
        {
            if (intent.Data != null)
            {
                MessagingCenter.Send<string>(intent.Data.EncodedPath, "mru4uresponse");
            }
            base.OnNewIntent(intent);
        }
    }
}

