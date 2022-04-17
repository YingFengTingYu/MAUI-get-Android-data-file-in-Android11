# MAUIPickFile
Edit the file in /sdcard/Android/data by C#, dotnet6 and MAUI!

Reference articles: https://www.sohu.com/a/479611641_121124367

All the class is in \Platforms\Android
You need to change AndroidManifest.xml
Add android:preserveLegacyExternalStorage="true" android:requestLegacyExternalStorage="true"
Add <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" /> <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" /> <uses-permission android:name="android.permission.MANAGE_EXTERNAL_STORAGE" />
FileHelper.cs is to get, create or delete the file
MainActivity.cs is to get the result of requesting permission
SAFHelper.cs is to get the file from /sdcard/Android/obb(Untested) or /sdcard/Android/data by SAF
SimpleFile.cs is to describe a file
StreamCreater.cs is to convert java stream to c sharp stream
In fact, you just need to request the permission once. So you need a way to save the permission. 