using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace MAUIPickFile;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        base.OnActivityResult(requestCode, resultCode, data);
        
        if (requestCode == SAFHelper.REQUEST_CODE_FOR_DIR)
        {
            Android.Net.Uri uri = data?.Data;
            if (uri != null)
            {
                ContentResolver.TakePersistableUriPermission(uri, data.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission));
                SAFHelper._result_1_bool = true;
            }
            else
            {
                SAFHelper._result_1_bool = false;
            }
        }
        else if (requestCode == SAFHelper.REQUEST_CODE_FOR_MANAGE)
        {
#pragma warning disable CA1416
            SAFHelper._result_1_bool = Android.OS.Environment.IsExternalStorageManager;
#pragma warning restore CA1416
        }
    }
}
