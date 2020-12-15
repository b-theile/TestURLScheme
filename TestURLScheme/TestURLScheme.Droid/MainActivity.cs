using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Java.IO;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace TestURLScheme.Droid
{
    [Activity(Label = "TestURLScheme", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault },
        DataScheme = "content",
        DataMimeType = "*/*",
        DataPathPatterns = new[] { ".*\\.zivapp" },
        DataHost = "*")]

    public class MainActivity : FormsAppCompatActivity
    {
        private static readonly int MRU_REQUEST = 1543;

        MainActivity _context;

        protected override void OnCreate(Bundle bundle)
        {           
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            _context = this;
            
            Forms.Init(this, bundle);
            LoadApplication(new App());

            if (Intent.Data != null)
            {
                if (Intent.Data.LastPathSegment.Contains("zivapp"))
                {
                    string message = GetMessageFromUri(Intent.Data);
                    MessagingCenter.Send<string>(message, "fileresponse");
                }
                //else
                //{
                //    MessagingCenter.Send<string>(Intent.Data.EncodedPath, "mru4uresponse");
                //}                    
            }

            MessagingCenter.Subscribe<string>(this, "mru4urequest", urlString =>
            {
                Intent intent = new Intent(Intent.ActionView, Uri.Parse(urlString));
                StartActivityForResult(intent, MRU_REQUEST);
            });

            MessagingCenter.Subscribe<string>(this, "filerequest", urlString =>
            {
                File zivPath = new File(FilesDir.AbsolutePath, "ZIV");
                File file = new File(zivPath, "exchange.MRUAPP");

                if (!zivPath.IsDirectory)
                    zivPath.Mkdir();

                try
                {
                    FileOutputStream fos = new FileOutputStream(file);

                    if (!file.Exists())
                    {
                        file.CreateNewFile();
                    }

                    byte[] contentInBytes = System.Text.Encoding.ASCII.GetBytes(urlString);

                    fos.Write(contentInBytes);
                    fos.Flush();
                    fos.Close();
                }
                catch (IOException e)
                {
                    e.PrintStackTrace();
                }

                Uri apkURI = AndroidX.Core.Content.FileProvider.GetUriForFile(_context.ApplicationContext, "com.testexchange.provider", file);

                Intent intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(apkURI, "MRUAPP");
                intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                StartActivity(intent);
            });
        }

        private string GetMessageFromUri(Uri uri)
        {
            var crFileDescriptor = _context.ContentResolver.OpenFileDescriptor(uri, "r");
            FileInputStream fis = new FileInputStream(crFileDescriptor.FileDescriptor);
            StringBuilder message = new StringBuilder();
            int ch;
            while ((ch = fis.Read()) != -1)
            {
                message.Append((char)ch);
            }

            return message.ToString();
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
