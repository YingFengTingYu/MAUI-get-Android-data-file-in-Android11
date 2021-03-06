namespace MAUIPickFile;

public partial class MainPage : ContentPage
{
    public static MainPage s;

    public MainPage()
	{
		InitializeComponent();
        s = this;
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
        //I use pvz1 to check my App
        //All the class is in \Platforms\Android
        //You need to change AndroidManifest.xml
        //Add android:preserveLegacyExternalStorage="true" android:requestLegacyExternalStorage="true"
        //Add <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" /> <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" /> <uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" />
        //FileHelper.cs is to get, create or delete the file
        //MainActivity.cs is to get the result of requesting permission
        //SAFHelper.cs is to get the file from /sdcard/Android/obb(Untested) or /sdcard/Android/data by SAF
        //SimpleFile.cs is to describe a file
        //StreamCreater.cs is to convert java stream to c sharp stream
        string path = "/sdcard/Android/data/com.popcap.pvz_na/files/userdata/user1.dat";
        path = "/sdcard/Android/obb/com.popcap.pvz_na/main.60002398.com.popcap.pvz_na.obb";
        new Thread(async () =>
        {
            using (Stream ms = await StreamCreater.OpenOrCreate(path))
            {
                //Do whatever you want
                ms.WriteByte(100);
                ms.WriteByte(200);
                ms.WriteByte(3);
                ms.WriteByte(40);
                //Save the changes
                await StreamCreater.Save(ms, path);
            }
        })
        { IsBackground = true }.Start();
    }
}

