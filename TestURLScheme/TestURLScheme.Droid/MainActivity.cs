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
    [Activity(Label = "TestURLScheme", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    //[IntentFilter(new[] { Intent.ActionView },
    //    DataScheme = "mru4uresponse",
    //    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
    

    public class MainActivity : FormsAppCompatActivity
    {
        private static int MRU_REQUEST = 1543;

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

            MessagingCenter.Subscribe<string>(this, "mru4urequest", urlString =>
            {
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(urlString));
                StartActivityForResult(intent, MRU_REQUEST);
            });
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            if (intent.Data != null)
            {
                MessagingCenter.Send<string>(intent.Data.EncodedPath, "mru4uresponse");
            }            
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == MRU_REQUEST)
            {
                if (data?.Data != null)
                {
                    MessagingCenter.Send<string>(data.Data.EncodedSchemeSpecificPart, "mru4uresponse");
                }
            }
        }
    }
}

